using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace VirtualSmartCard
{
    public abstract class ReaderSettings
    {
        public String Name { get; set; }
        public String Host { get; set; }
        public bool IsRemote { get; set; }

        static public ReaderSettings GetSettings(RegistryKey regKey)
        {
            int type = (int)regKey.GetValue("Type", 0);
            string Host = regKey.GetValue("Host", ".").ToString();
            if (type == 1)
                return new TcpIpReaderSettings(regKey, Host);
            else if (type == 0)
                return new PipeReaderSettings(regKey, Host);
            else throw new Exception("Reader type not defined");
                
        }
        public override string ToString()
        {
            if (IsRemote)
                return Host + "\\" + Name;
            else
                return Name;
        }
    }

    public class TcpIpReaderSettings : ReaderSettings
    {
        internal TcpIpReaderSettings() { }
        internal TcpIpReaderSettings(RegistryKey regKey, string host) {
            IsRemote = true;
            Host = host;
            Name = regKey.Name.Substring(regKey.Name.LastIndexOf('\\')+1);
            Port = (int)regKey.GetValue("Port", 29500);
            EventPort = (int)regKey.GetValue("EventPort", 29501);
        }
        public int Port {get;set;}
        public int EventPort {get;set;}
    }

    public class PipeReaderSettings : ReaderSettings
    {
        internal PipeReaderSettings() { }
        internal PipeReaderSettings(RegistryKey regKey, string host)
        {
            IsRemote = true;
            Host = host;
            Name = regKey.Name.Substring(regKey.Name.LastIndexOf('\\') + 1);
            PipeName = regKey.GetValue("PipeName", "SCardSimulatorDriver0").ToString();
            EventPipeName = regKey.GetValue("EventPipeName", "SCardSimulatorDriverEvents0").ToString();
        }
        public string PipeName { get; set; }
        public string EventPipeName { get; set; }
    }
}
