using System.Net.NetworkInformation;

namespace UPCD.App.Services;

public sealed class UpdateNetworkStatus
{
    public bool HasInternet { get; init; }
    public bool HasWifiConnection { get; init; }
}

public sealed class UpdateService
{
    // Configure only after a real HTTPS release feed and signed packages exist.
    private const string? UpdateFeedUri = null;

    public UpdateNetworkStatus GetNetworkStatus()
    {
        var interfaces = NetworkInterface.GetAllNetworkInterfaces()
            .Where(n => n.OperationalStatus == OperationalStatus.Up);
        return new UpdateNetworkStatus
        {
            HasInternet = NetworkInterface.GetIsNetworkAvailable(),
            HasWifiConnection = interfaces.Any(n => n.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)
        };
    }

    public bool IsUpdateFeedConfigured =>
        Uri.TryCreate(UpdateFeedUri, UriKind.Absolute, out var uri) &&
        uri.Scheme == Uri.UriSchemeHttps;

    public string GetReadinessMessage(bool isEnglish)
    {
        if (!IsUpdateFeedConfigured)
            return isEnglish
                ? "Update feed is not configured yet. This preview will not download or install updates."
                : "Источник обновлений пока не настроен. Эта версия не загружает и не устанавливает обновления.";

        var status = GetNetworkStatus();
        if (!status.HasInternet) return isEnglish ? "No internet connection." : "Нет подключения к интернету.";
        if (!status.HasWifiConnection) return isEnglish ? "Internet is available, but Wi-Fi is not connected." : "Интернет доступен, но Wi-Fi не подключён.";
        return isEnglish ? "Wi-Fi is available for update checks." : "Wi-Fi доступен для проверки обновлений.";
    }
}
