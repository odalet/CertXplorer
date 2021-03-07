using System.Diagnostics.CodeAnalysis;

[SuppressMessage("Major Bug", "S3903:Types should be defined in named namespaces", Justification = "Will be removed later")]
internal static class ThisAssembly
{
    public const string PluginVersion = "1.3.0.0";
    public const string Description = "A set of cryptography related tools";
    public const string Product = "Delta.CertXplorer";
    public const string Company = "Delta Apps";
    public static string Version => GitVersionInformation.AssemblySemVer;
}
