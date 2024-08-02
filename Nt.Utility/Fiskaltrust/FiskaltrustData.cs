using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Nt.Utility
{
    public class FiskaltrustData
    {
        private static FiskaltrustData _instance;

        public static FiskaltrustData Instance => _instance ?? (_instance = new FiskaltrustData());

        public string Server { get; set; }
        public string Port { get; set; }
        public string CashboxID { get; set; }
        public string TerminalID { get; set; }
        public string UserID { get; set; }
        public string OpenTransactions { get; set; }
        public string MaxNumberOfClients { get; set; }
        public string CurrentNumberOfClients { get; set; }
        public string CurrentClientIds { get; set; }
        public string MaxNumberOfStartedTransactions { get; set; }
        public string CurrentNumberOfStartedTransactions { get; set; }
        public string CurrentStartedTransactionNumbers { get; set; }
        public string MaxNumberOfSignatures { get; set; }
        public string CurrentNumberOfSignatures { get; set; }
        public string MaxLogMemorySize { get; set; }
        public string CurrentLogMemorySize { get; set; }
        public string CurrentState { get; set; }
        public string FirmwareIdentification { get; set; }
        public string CertificationIdentification { get; set; }
        public string SignatureAlgorithm { get; set; }
        public string LogTimeFormat { get; set; }
        public string SerialNumberOctet { get; set; }
        public string PublicKeyBase64 { get; set; }
        public string CertificatesBase64 { get; set; }

        public void SaveFiskaltrustData(string server, string port, string cashboxID, string terminalID, string userID, string openTransactions)
        {
            Server = server;
            Port = port;
            CashboxID = cashboxID;
            TerminalID = terminalID;
            UserID = userID;
            OpenTransactions = openTransactions;
        }
    }
}