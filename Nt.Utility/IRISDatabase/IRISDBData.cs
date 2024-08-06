using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nt.Utility.IRISDatabase
{
    public class IRISDBData
    {
        private static IRISDBData _instance;

        public static IRISDBData Instance => _instance ?? (_instance = new IRISDBData());


        public string Server { get; set; }
        public string Port { get; set; }
        public string Nt_Namepsace { get; set; }
        public string FA { get; set; }
        public string KASSA { get; set; }
        public string KEY { get; set; }
        public string from_Table { get; set; }
        public string to_Table { get; set; }
        public string VKO { get; set; }

        public void SaveIRISDBData(string server, string port, string _Nt_Namespace, string _FA, string _KASSA, string _KEY, string _from_Table, string _to_Table)
        {
            Server = server;
            Port = port;
            Nt_Namepsace = _Nt_Namespace;
            FA = _FA;
            KASSA = _KASSA;
            KEY = _KEY;
            from_Table = _from_Table;
            to_Table = _to_Table;
        }

    }
}
