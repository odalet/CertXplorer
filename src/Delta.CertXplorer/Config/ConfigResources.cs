using System.IO;
using System.Linq;
using System.Reflection;

namespace Delta.CertXplorer.Config
{
    internal static class ConfigResources 
    {
        static ConfigResources()
        {
            // This extracts this namespace + "." -> Delta.CertXplorer.Config.
            var type = typeof(ConfigResources);
            var full = type.FullName;
            Prefix = full.Substring(0, full.Length - type.Name.Length);
        }

        private static string Prefix { get; }

        public static byte[] Read(string name)
        {
            var resourceName = Prefix + name;
            var assembly = Assembly.GetExecutingAssembly();
            if (!assembly.GetManifestResourceNames().Contains(resourceName))
                return null;

            using var mstream = new MemoryStream();
            using var stream = assembly.GetManifestResourceStream(resourceName);
            stream.CopyTo(mstream);
            return mstream.ToArray();
        }
    }
}
