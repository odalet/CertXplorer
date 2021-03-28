using System;
using System.ComponentModel;
using System.Security.Cryptography.X509Certificates;

namespace Delta.CapiNet
{
    /// <summary>
    /// This class wraps a <see cref="X509Certificate2"/> object.
    /// </summary>
    public sealed class Certificate
    {
        public Certificate(X509Certificate2 certificate) => X509 = certificate;

        [Browsable(false)]
        public X509Certificate2 X509 { get; }

        /// <summary>
        /// Gets a value indicating whether this certificate is valid.
        /// </summary>
        /// <remarks>
        /// The validity of the certificate is only verified by examining its 
        /// validity period. No certification chain verification is done.
        /// </remarks>
        /// <value><c>true</c> if this certificate is valid; otherwise, <c>false</c>.</value>
        public bool IsValid
        {
            get
            {
                var now = DateTime.Now;
                return now >= X509.NotBefore && now <= X509.NotAfter;
            }
        }

        public X500DistinguishedName SubjectName => X509.SubjectName;
        public bool HasPrivateKey => X509.HasPrivateKey;
        public X500DistinguishedName IssuerName => X509.IssuerName;

        public string FriendlyName
        {
            get => X509.FriendlyName;
            set => X509.FriendlyName = value;
        }
    }
}
