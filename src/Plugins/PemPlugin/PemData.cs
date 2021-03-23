using System.Collections.Generic;
using System.ComponentModel;
using Delta.CapiNet.Pem;
using Delta.CertXplorer.ComponentModel;
using Delta.CertXplorer.Extensibility;

namespace PemPlugin
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    internal sealed class PemData : IData
    {
        [TypeConverter(typeof(ExpandableObjectConverter))]
        private class ComplexPemData
        {
            [TypeConverter(typeof(ExpandableObjectConverter))]
            public PemInfo Info { get; internal set; }
                        
            [TypeConverter(typeof(ReadOnlyDictionaryConverter))]            
            public IReadOnlyDictionary<string, string> AdditionalHeaders { get; internal set; }
        }

        public PemData(PemInfo info)
        {
            if (info == null) return;

            MainData = info.Workload;
            if (info.AdditionalHeaders == null || info.AdditionalHeaders.Count == 0)
                AdditionalData = info;

            AdditionalData = new ComplexPemData
            {
                Info = info,
                AdditionalHeaders = info.AdditionalHeaders
            };
        }

        public byte[] MainData { get; }
        public object AdditionalData { get; }
    }
}
