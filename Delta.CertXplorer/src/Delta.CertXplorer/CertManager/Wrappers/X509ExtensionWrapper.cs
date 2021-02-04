using System;
using System.Linq;
using System.ComponentModel;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Diagnostics.CodeAnalysis;

namespace Delta.CertXplorer.CertManager.Wrappers
{
    [TypeConverter(typeof(CustomExpandableObjectConverter))]
    internal abstract class X509ExtensionWrapper : IDisplayTypeWrapper
    {
        private class X509SimpleExtensionWrapper : X509ExtensionWrapper
        {
            public X509SimpleExtensionWrapper(X509Extension extension) : base(extension) => Type = extension.GetType();

            [SuppressMessage("Major Code Smell", "S1144:Unused private types or members should be removed", Justification = "Used by the PropertyGrid")]
            public Type Type { get; }
        }

        private class X509BasicConstraintsExtensionWrapper : X509ExtensionWrapper
        {
            private readonly X509BasicConstraintsExtension x509;

            public X509BasicConstraintsExtensionWrapper(X509BasicConstraintsExtension extension) : base(extension) =>
                x509 = extension;

            [SuppressMessage("Major Code Smell", "S1144:Unused private types or members should be removed", Justification = "Used by the PropertyGrid")]
            public bool CertificateAuthority => x509.CertificateAuthority;

            [SuppressMessage("Major Code Smell", "S1144:Unused private types or members should be removed", Justification = "Used by the PropertyGrid")]
            public bool HasPathLengthConstraint => x509.HasPathLengthConstraint;

            [SuppressMessage("Major Code Smell", "S1144:Unused private types or members should be removed", Justification = "Used by the PropertyGrid")]
            public int PathLengthConstraint => x509.PathLengthConstraint;
        }

        private class X509KeyUsageExtensionWrapper : X509ExtensionWrapper
        {
            public X509KeyUsageExtensionWrapper(X509KeyUsageExtension extension) : base(extension) => 
                KeyUsages = extension.KeyUsages;

            [SuppressMessage("Major Code Smell", "S1144:Unused private types or members should be removed", Justification = "Used by the PropertyGrid")]
            public X509KeyUsageFlags KeyUsages { get; }
        }

        private class X509EnhancedKeyUsageExtensionWrapper : X509ExtensionWrapper
        {
            public X509EnhancedKeyUsageExtensionWrapper(X509EnhancedKeyUsageExtension extension) : base(extension) => 
                EnhancedKeyUsages = extension.EnhancedKeyUsages != null && extension.EnhancedKeyUsages.Count > 0 ?
                    extension.EnhancedKeyUsages.Cast<Oid>().Select(o => new OidWrapper(o)).ToArray() :
                    new OidWrapper[0];

            [SuppressMessage("Major Code Smell", "S1144:Unused private types or members should be removed", Justification = "Used by the PropertyGrid")]
            public OidWrapper[] EnhancedKeyUsages { get; }
        }

        private class X509SubjectKeyIdentifierExtensionWrapper : X509ExtensionWrapper
        {
            public X509SubjectKeyIdentifierExtensionWrapper(X509SubjectKeyIdentifierExtension extension) : base(extension) =>
                SubjectKeyIdentifier = extension.SubjectKeyIdentifier;

            [SuppressMessage("Major Code Smell", "S1144:Unused private types or members should be removed", Justification = "Used by the PropertyGrid")]
            public string SubjectKeyIdentifier { get; }
        }

        private readonly X509Extension x509ext;

        protected X509ExtensionWrapper(X509Extension extension)
        {
            x509ext = extension;
            Oid = new OidWrapper(x509ext.Oid);
        }

        public bool Critical => x509ext.Critical;
        public byte[] RawData => x509ext.RawData;
        private OidWrapper Oid { get; }

        public static X509ExtensionWrapper Create(X509Extension extension)
        {
            if (extension == null) return null;
            else if (extension is X509BasicConstraintsExtension bc)
                return new X509BasicConstraintsExtensionWrapper(bc);
            else if (extension is X509EnhancedKeyUsageExtension eku)
                return new X509EnhancedKeyUsageExtensionWrapper(eku);
            else if (extension is X509KeyUsageExtension ku)
                return new X509KeyUsageExtensionWrapper(ku);
            else if (extension is X509SubjectKeyIdentifierExtension ski)
                return new X509SubjectKeyIdentifierExtensionWrapper(ski);
            
            return new X509SimpleExtensionWrapper(extension);
        }

        [Browsable(false)]
        public virtual string DisplayType => Oid.DisplayType;
    }
}
