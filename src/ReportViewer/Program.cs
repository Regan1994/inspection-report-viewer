using System.Diagnostics;
using System.Text.Json;

namespace InspectionReportViewer;

internal static class Program
{
    private const string DefaultServerUrl = "http://127.0.0.1:5000/manager";

    [STAThread]
    private static void Main(string[] args)
    {
        ApplicationConfiguration.Initialize();

        try
        {
            var settings = ViewerSettings.Load(AppContext.BaseDirectory, args, DefaultServerUrl);
            LaunchManagerPage(settings.ServerUrl);
        }
        catch (Exception exception)
        {
            MessageBox.Show(
                exception.Message,
                "校機報工報表閱讀器",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }
    }

    private static void LaunchManagerPage(Uri serverUrl)
    {
        var browser = BrowserLocator.Find();
        if (browser is null)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = serverUrl.AbsoluteUri,
                UseShellExecute = true,
            });
            return;
        }

        var startInfo = new ProcessStartInfo
        {
            FileName = browser,
            UseShellExecute = false,
        };
        startInfo.ArgumentList.Add($"--app={serverUrl.AbsoluteUri}");
        Process.Start(startInfo);
    }
}

internal sealed record ViewerSettings(Uri ServerUrl)
{
    public static ViewerSettings Load(string baseDirectory, string[] args, string defaultServerUrl)
    {
        var configuredUrl = ReadCommandLineUrl(args)
            ?? ReadConfigUrl(baseDirectory)
            ?? defaultServerUrl;

        if (!Uri.TryCreate(configuredUrl, UriKind.Absolute, out var serverUrl)
            || (serverUrl.Scheme != Uri.UriSchemeHttp && serverUrl.Scheme != Uri.UriSchemeHttps)
            || string.IsNullOrWhiteSpace(serverUrl.Host))
        {
            throw new InvalidOperationException(
                $"主系統網址格式不正確：{configuredUrl}\n請使用 http://主機IP:5000/manager。 ");
        }

        return new ViewerSettings(serverUrl);
    }

    private static string? ReadCommandLineUrl(string[] args)
    {
        for (var index = 0; index < args.Length; index++)
        {
            if (string.Equals(args[index], "--url", StringComparison.OrdinalIgnoreCase)
                && index + 1 < args.Length)
            {
                return args[index + 1];
            }
        }

        return null;
    }

    private static string? ReadConfigUrl(string baseDirectory)
    {
        var path = Path.Combine(baseDirectory, "ReportViewer.json");
        if (!File.Exists(path))
        {
            return null;
        }

        try
        {
            var json = File.ReadAllText(path);
            var config = JsonSerializer.Deserialize<ViewerConfig>(
                json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return config?.ServerUrl;
        }
        catch (JsonException exception)
        {
            throw new InvalidOperationException(
                $"ReportViewer.json 無法讀取：{exception.Message}",
                exception);
        }
    }

    private sealed record ViewerConfig(string? ServerUrl);
}

internal static class BrowserLocator
{
    public static string? Find()
    {
        var candidates = new[]
        {
            Combine(Environment.GetEnvironmentVariable("ProgramFiles(x86)"), "Microsoft", "Edge", "Application", "msedge.exe"),
            Combine(Environment.GetEnvironmentVariable("ProgramFiles"), "Microsoft", "Edge", "Application", "msedge.exe"),
            Combine(Environment.GetEnvironmentVariable("LOCALAPPDATA"), "Microsoft", "Edge", "Application", "msedge.exe"),
            Combine(Environment.GetEnvironmentVariable("ProgramFiles(x86)"), "Google", "Chrome", "Application", "chrome.exe"),
            Combine(Environment.GetEnvironmentVariable("ProgramFiles"), "Google", "Chrome", "Application", "chrome.exe"),
            Combine(Environment.GetEnvironmentVariable("LOCALAPPDATA"), "Google", "Chrome", "Application", "chrome.exe"),
        };

        return candidates.FirstOrDefault(path => path is not null && File.Exists(path));
    }

    private static string? Combine(string? root, params string[] parts)
    {
        return string.IsNullOrWhiteSpace(root)
            ? null
            : Path.Combine(new[] { root }.Concat(parts).ToArray());
    }
}

