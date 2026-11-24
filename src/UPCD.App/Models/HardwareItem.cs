namespace UPCD.App.Models;

public sealed record HardwareItem(string Category, string Name, string Details, string Source = "WMI");
