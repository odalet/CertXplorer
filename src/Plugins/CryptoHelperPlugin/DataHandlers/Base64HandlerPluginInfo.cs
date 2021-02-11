using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Delta.CertXplorer.Extensibility;

namespace CryptoHelperPlugin.DataHandlers
{
    internal class Base64HandlerPluginInfo : IPluginInfo
    {
        private static readonly Guid guid = new Guid("{F017208B-06FC-4A4B-AC8E-387A9F06B7AE}");

        #region IPluginInfo Members

        public Guid Id { get { return guid; } }

        public string Name
        {
            get { return "Base64 Plugin"; }
        }

        public string Description
        {
            get { return ThisAssembly.Description; }
        }

        public string Author
        {
            get { return "O. DALET"; }
        }

        public string Company
        {
            get { return ThisAssembly.Company; }
        }

        public string Version
        {
            get { return ThisAssembly.PluginVersion; }
        }

        #endregion
    }
}
