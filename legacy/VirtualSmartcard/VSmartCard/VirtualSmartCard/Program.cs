using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using Microsoft.Win32;
using System.Text;

namespace VirtualSmartCard
{
    static class Program
    {
        public static Dictionary<string, ReaderSettings> SimulatedReaders { get; set; }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            SimulatedReaders = new Dictionary<string, ReaderSettings>();
            try
            {
                RegistryKey regKey = Registry.CurrentUser.OpenSubKey(@"Software\VirtualSmartCard\RemoteReaders");
                foreach (var name in regKey.GetSubKeyNames())
                {
                    var readerKey = regKey.OpenSubKey(name);
                    var reader = ReaderSettings.GetSettings(readerKey);
                    SimulatedReaders[reader.Name] = reader;
                    readerKey.Close();
                }
                regKey.Close();
            }
            catch {}


            SmartCard sc = new SmartCard();
            var s = sc.ListReaders();
            foreach (String r in s)
            {
                ReaderSettings settings = null;
                sc.Connect(r, SmartCard.share.SCARD_SHARE_DIRECT, SmartCard.protocol.SCARD_PROTOCOL_UNDEFINED);
                try
                {
                    byte[] isSimulated = sc.GetAttrib(0x7a009);
                    if (isSimulated != null)
                    {
                        int readerType = isSimulated[0];
                        if (readerType == 0)
                        {
                            var pipeSettings = new PipeReaderSettings();
                            byte[] data;
                            pipeSettings.PipeName = Encoding.ASCII.GetString((data = sc.GetAttrib(0x07a00a)), 0, data.Length - 1);
                            pipeSettings.EventPipeName = Encoding.ASCII.GetString((data = sc.GetAttrib(0x07a00b)), 0, data.Length - 1);
                            pipeSettings.Host = "."; 
                            settings = pipeSettings;
                        }
                        else if (readerType == 1)
                        {
                            var tcpSettings = new TcpIpReaderSettings();
                            byte[] data;
                            tcpSettings.Port = ((data = sc.GetAttrib(0x07a00c))[0] | (data[1] << 8) | (data[2] << 16) | (data[3] << 24));
                            tcpSettings.EventPort = ((data = sc.GetAttrib(0x07a00d))[0] | (data[1] << 8) | (data[2] << 16) | (data[3] << 24));
                            tcpSettings.Host = "127.0.0.1";
                            settings = tcpSettings;
                        }
                    }

                    settings.Name = r;
                    settings.IsRemote = false;
                    SimulatedReaders[r] = settings;
                }
                catch { }

                finally
                {
                    sc.Disconnect(SmartCard.disposition.SCARD_LEAVE_CARD);
                }
            }


            ScanForCards();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Main());
        }

        public static List<ICardImplementation> cardImplementations { get; set; }
        static void ScanForCards()
        {
            cardImplementations = new List<ICardImplementation>();
            AppDomain.CurrentDomain.AppendPrivatePath(".\\cards");
            
            foreach (string path in Directory.GetFiles(".\\cards", "*.dll")) {
                try
                {
                    var ass = Assembly.LoadFile(Path.GetFullPath(path));
                    foreach (Type t in ass.GetTypes())
                    {
                        if (t.IsClass && t.FindInterfaces(new TypeFilter((type, obj) =>
                        {
                            return type == typeof(ICardPlugin);
                        }), null).Length > 0)
                        {
                            ICardPlugin plugin = Activator.CreateInstance(t) as ICardPlugin;
                            cardImplementations.AddRange(plugin.Implementations);
                        }
                    }
                }
                catch { }
            }
        }
    }
}
