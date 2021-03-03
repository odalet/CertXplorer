internal static class ThisAssembly
{
    public const string PluginVersion = "1.1.4.0";
    public const string Description = "Self-signed certificate generation plugin for CertXplorer";

    public const string Product = "Delta.CertXplorer";
    public const string Company = "Delta Apps";    
    public const string Name = "PluralSightSelfCertPlugin";
    public static string Version => GitVersionInformation.AssemblySemVer;
}
