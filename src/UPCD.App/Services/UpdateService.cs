using System.Net.NetworkInformation;

namespace UPCD.App.Services;

public sealed class UpdateNetworkStatus
{
    public bool HasInternet { get; init; }
    public bool HasWifiConnection { get; init; }
}

public sealed class UpdateService
{
    // Configure only after a trusted HTTPS release feed and verifiable packages exist.
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
        var status = GetNetworkStatus();

        if (!status.HasInternet)
            return isEnglish
                ? "No active network connection detected. Update source is not configured."
                : "Активное сетевое подключение не обнаружено. Источник обновлений не настроен.";

        if (!status.HasWifiConnection)
            return isEnglish
                ? "Internet connection detected; Wi-Fi interface is not active. Update source is not configured."
                : "Сетевое подключение обнаружено, но Wi-Fi не активен. Источник обновлений не настроен.";

        if (!IsUpdateFeedConfigured)
            return isEnglish
                ? "Wi-Fi detected. Automatic update checks and downloads are not available yet because no trusted update source is configured."
                : "Wi-Fi обнаружен. Автопроверка и загрузка обновлений пока недоступны: доверенный источник обновлений ещё не настроен.";

        return isEnglish
            ? "Wi-Fi detected. The update source is configured and ready for a check."
            : "Wi-Fi обнаружен. Источник обновлений настроен и готов к проверке.";
    }
}
