using System.Management;
using UPCD.App.Models;

namespace UPCD.App.Services;

/// <summary>
/// Read-only Windows hardware inventory. Values are reported only when Windows exposes them through WMI.
/// </summary>
public sealed class HardwareInventoryService
{
    public Task<IReadOnlyList<HardwareItem>> ScanAsync(CancellationToken cancellationToken = default) =>
        Task.Run<IReadOnlyList<HardwareItem>>(() => Scan(cancellationToken), cancellationToken);

    private static IReadOnlyList<HardwareItem> Scan(CancellationToken cancellationToken)
    {
        var items = new List<HardwareItem>();
        Add(items, "CPU", "SELECT Name, NumberOfCores, NumberOfLogicalProcessors, MaxClockSpeed FROM Win32_Processor",
            o => $"{Text(o, "NumberOfCores")} cores · {Text(o, "NumberOfLogicalProcessors")} threads · max {Text(o, "MaxClockSpeed")} MHz");
        Add(items, "GPU", "SELECT Name, DriverVersion, VideoProcessor FROM Win32_VideoController",
            o => $"driver {Text(o, "DriverVersion")} · {Text(o, "VideoProcessor")}");
        Add(items, "RAM", "SELECT Capacity, Speed, Manufacturer, PartNumber, DeviceLocator FROM Win32_PhysicalMemory",
            o => $"{FormatBytes(Number(o, "Capacity"))} · {Text(o, "Speed")} MHz · {Text(o, "Manufacturer")} {Text(o, "PartNumber")} · {Text(o, "DeviceLocator")}");
        Add(items, "Motherboard", "SELECT Manufacturer, Product, Version FROM Win32_BaseBoard",
            o => $"{Text(o, "Manufacturer")} · {Text(o, "Product")} · {Text(o, "Version")}", "Product");
        Add(items, "BIOS", "SELECT Manufacturer, SMBIOSBIOSVersion, ReleaseDate FROM Win32_BIOS",
            o => $"{Text(o, "Manufacturer")} · {Text(o, "SMBIOSBIOSVersion")} · {Text(o, "ReleaseDate")}", "SMBIOSBIOSVersion");
        Add(items, "Storage", "SELECT Model, Size, InterfaceType, MediaType FROM Win32_DiskDrive",
            o => $"{FormatBytes(Number(o, "Size"))} · {Text(o, "InterfaceType")} · {Text(o, "MediaType")}", "Model");
        Add(items, "Windows", "SELECT Caption, Version, BuildNumber, OSArchitecture FROM Win32_OperatingSystem",
            o => $"{Text(o, "Version")} (build {Text(o, "BuildNumber")}) · {Text(o, "OSArchitecture")}", "Caption");
        cancellationToken.ThrowIfCancellationRequested();
        return items;
    }

    private static void Add(List<HardwareItem> items, string category, string query, Func<ManagementBaseObject, string> details, string nameProperty = "Name")
    {
        try
        {
            using var searcher = new ManagementObjectSearcher(query);
            using var results = searcher.Get();
            foreach (ManagementBaseObject item in results)
            {
                using (item)
                {
                    var name = Text(item, nameProperty);
                    if (string.IsNullOrWhiteSpace(name) || name == "Unknown")
                        name = category;
                    var detail = details(item);
                    items.Add(new HardwareItem(category, name, string.IsNullOrWhiteSpace(detail) ? "Some properties are not exposed by this device." : detail));
                }
            }
        }
        catch (Exception ex) when (ex is ManagementException or UnauthorizedAccessException or System.Runtime.InteropServices.COMException or InvalidOperationException)
        {
            items.Add(new HardwareItem(category, $"{category} information unavailable", ex.Message, "WMI error"));
        }
    }

    private static string Text(ManagementBaseObject item, string property) =>
        Convert.ToString(item[property], System.Globalization.CultureInfo.InvariantCulture)?.Trim() ?? "Unknown";

}
