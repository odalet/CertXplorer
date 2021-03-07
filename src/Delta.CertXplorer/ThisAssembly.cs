using System.Diagnostics.CodeAnalysis;

[SuppressMessage("Major Bug", "S3903:Types should be defined in named namespaces", Justification = "Will be removed later")]
internal static class ThisAssembly
{
    public const string Product = "Delta.CertXplorer";
    public const string Company = "Delta Apps";    
    public const string Name = "Delta.CertXplorer";
    public static string Version => GitVersionInformation.AssemblySemVer;
}
