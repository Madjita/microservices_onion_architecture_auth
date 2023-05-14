using System.Reflection;

namespace AuthDomain.Helpers;

public static class AppVersion {
    public static string Version => _version + _versionSuffix; // Теперь версия берётся автоматически
    private static readonly string _version = LoadVersion();
    private static readonly string _versionSuffix = "";
    static string LoadVersion()
    {
        var version = Assembly
                        //.GetExecutingAssembly()
                        .GetCallingAssembly()
                        .GetName()
                        .Version;
        return version!.ToString();
    }
}