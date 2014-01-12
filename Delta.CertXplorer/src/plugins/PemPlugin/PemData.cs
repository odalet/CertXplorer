using System.ComponentModel;
using System.Collections.Generic;

using Delta.CapiNet.Pem;
using Delta.CertXplorer.Extensibility;
using Delta.CertXplorer.ComponentModel;

namespace PemPlugin
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    internal class PemData : IData
    {
        [TypeConverter(typeof(ExpandableObjectConverter))]
        private class ComplexPemData
        {
            [TypeConverter(typeof(ExpandableObjectConverter))]
            public PemInfo Info { get; internal set; }

            //public KeyValuePair<string, string>[] AdditionalHeaders { get; internal set; }
            
            // TODO: Find a means for PropertyGridEx to automatically apply this when encountering a doictionary
            // See http://stackoverflow.com/questions/2535647/insert-custom-typeconverter-on-a-property-at-runtime-from-inside-a-custom-uityp
            [TypeConverter(typeof(ReadOnlyDictionaryConverter))]            
            public IDictionary<string, string> AdditionalHeaders { get; internal set; }
        }

        public PemData(PemInfo info)
        {
            if (info == null) return;

            MainData = info.Workload;
            if (info.AdditionalHeaders == null || info.AdditionalHeaders.Count == 0)
                AdditionalData = info;

            var data = new ComplexPemData();
            data.Info = info;
            data.AdditionalHeaders = info.AdditionalHeaders;
            AdditionalData = data;
        }

        #region IData Members

        public byte[] MainData { get; private set; }

        public object AdditionalData { get; private set; }

        #endregion
    }
}
