using System.Reflection;

namespace suntadela.web;

public static class BuildInfo
{
    public static string CommitHash { get; }
    public static bool IsDevBuild { get; }
    public static string AppName => IsDevBuild ? "suntadela.web-dev" : "suntadela.web";

    static BuildInfo()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var metadata = assembly.GetCustomAttributes<AssemblyMetadataAttribute>();
        CommitHash = metadata.FirstOrDefault(a => a.Key == "GitCommitHash")?.Value ?? "unknown";
        IsDevBuild = metadata.FirstOrDefault(a => a.Key == "IsDevBuild")?.Value == "true";
    }
}
