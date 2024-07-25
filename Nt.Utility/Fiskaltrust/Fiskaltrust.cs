using fiskaltrust.ifPOS.v1;
// fiskaltrust.ifPOS.v0;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using ProtoBuf.Bcl;
using fiskaltrust.Middleware.Interface.Client;
using fiskaltrust.Middleware.Interface.Client.Grpc;
using Grpc.Net.Client;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Data.SqlTypes;
using System.Configuration;
using ProtoBuf;
using Newtonsoft.Json;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using System.Text.Json;

namespace Nt.Utility.Fiskaltrust
{
    public class Fiskaltrust
    {

        private readonly string _server;
        private readonly string _port;
        private readonly string _cashBoxID;
        private readonly string _terminalID;
        private readonly string _userID;
        private readonly string _openTransaktions;
        private readonly string _NCPosSystemID;
        private readonly MainWindow mainWindow;

        private POS.POSClient _client;
        private string _timestamp;
        private string _transactionNumbersJson;

        public Fiskaltrust(string server, string port, string cashboxID, string terminalID, string userID, string openTransaktions)
        {
            _server = server;
            _port = port;
            _cashBoxID = cashboxID;
            _terminalID = terminalID;
            _userID = userID;
            _openTransaktions = openTransaktions;
            _NCPosSystemID = "1f2a492e-39d8-4799-b7e7-15435a4c8a24";

            GetTimestamp();


        }
        private string GetTimestamp()
        {
            _timestamp = System.DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffffffZ");
            return _timestamp;
        }

        private long ToNetTicks(System.DateTime dateTime)
        {
            return dateTime.ToUniversalTime().Ticks;
        }

        private static ProtoBuf.Bcl.DateTime ToBclDateTime(System.DateTime dateTime)
        {
            var utcDateTime = dateTime.ToUniversalTime();
            var seconds = (long)(utcDateTime - new System.DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
            return new ProtoBuf.Bcl.DateTime
            {
                Value = seconds,
                Scale = ProtoBuf.Bcl.DateTime.Types.TimeSpanScale.Seconds,
                Kind = dateTime.Kind == DateTimeKind.Utc ? ProtoBuf.Bcl.DateTime.Types.DateTimeKind.Utc :
                       dateTime.Kind == DateTimeKind.Local ? ProtoBuf.Bcl.DateTime.Types.DateTimeKind.Local :
                       ProtoBuf.Bcl.DateTime.Types.DateTimeKind.Unspecified
            };
        }

        public void FiskalConnect()
        {
            var url = $"http://{_server}:{_port}/json/v1/Sign";
            var channel = GrpcChannel.ForAddress(url);
            _client = new POS.POSClient(channel);
        }


        public async Task<string> EchoAsync(string message)
        {
            FiskalConnect();

            var request = new EchoRequest { Message = message };

            var response = await _client.EchoAsync(request);

            Console.WriteLine("Response: " + response.Message);
            return response.Message;
        }

        public async Task<string> CreateStartReceiptAsync()
        {
            FiskalConnect();

            var request = new fiskaltrust.ifPOS.v1.ReceiptRequest
            {
                FtCashBoxID = _cashBoxID,
                FtPosSystemId = _NCPosSystemID,
                CbReceiptMoment = ToBclDateTime(System.DateTime.UtcNow),
                FtReceiptCase = 4919338172267102211,
                CbTerminalID = _terminalID,
                CbReceiptReference = "INIT",
                CbUser = _userID,


            };

            var response = await _client.SignAsync(request);

            return response.FtReceiptIdentification;
        }

        public async Task<string> ZeroReceiptAsync()
        {
            FiskalConnect();

            var request = new fiskaltrust.ifPOS.v1.ReceiptRequest
            {
                FtCashBoxID = _cashBoxID,
                FtPosSystemId = _NCPosSystemID,
                CbReceiptMoment = ToBclDateTime(System.DateTime.UtcNow),
                FtReceiptCase = 4919338172267102210,
                CbTerminalID = _terminalID,
                CbReceiptReference = "Zero-Receipt"


            };
            var response = await _client.SignAsync(request);

            return response.FtReceiptIdentification;

        }

        public async Task<string> ZeroReceiptTSEInfoAsync()
        {
            FiskalConnect();

            var request = new fiskaltrust.ifPOS.v1.ReceiptRequest
            {
                FtCashBoxID = _cashBoxID,
                FtPosSystemId = _NCPosSystemID,
                CbReceiptMoment = ToBclDateTime(System.DateTime.UtcNow),
                FtReceiptCase = 4919338172275490818,
                CbTerminalID = _terminalID,
                CbReceiptReference = "ZeroReceiptAfterFailure",


            };
            var response = await _client.SignAsync(request);

            return response.FtReceiptIdentification;

        }
        public async Task<string> ZeroReceiptCloseTransaktionsAsync()
        {
            FiskalConnect();

            var request = new fiskaltrust.ifPOS.v1.ReceiptRequest
            {
                FtCashBoxID = _cashBoxID,
                FtPosSystemId = _NCPosSystemID,
                CbReceiptMoment = ToBclDateTime(System.DateTime.UtcNow),
                FtReceiptCase = 4919338172803973122,
                CbTerminalID = _terminalID,
                CbReceiptReference = "Zero-Receipt-close-transaktions"


            };
            var response = await _client.SignAsync(request);

            return response.FtReceiptIdentification;

        }

        public async Task<string> OutOfOperationAsync()
        {
            
            FiskalConnect();

            var request = new fiskaltrust.ifPOS.v1.ReceiptRequest
            {
                FtCashBoxID = _cashBoxID,
                FtPosSystemId = _NCPosSystemID,
                CbTerminalID = _terminalID,
                CbReceiptReference = "OutOfOperation",
                CbReceiptMoment = ToBclDateTime(System.DateTime.UtcNow),
                FtReceiptCase = 4919338172283879428,
                CbUser = _userID,
            };
            var response = await _client.SignAsync(request);

            return response.FtReceiptIdentification;

        }

        public async Task<string> DailyClosingAsync()
        {
            FiskalConnect();

            var request = new fiskaltrust.ifPOS.v1.ReceiptRequest
            {
                FtCashBoxID = _cashBoxID,
                FtPosSystemId = _NCPosSystemID,
                CbTerminalID = _terminalID,
                CbReceiptReference = "daily-closing-" + _timestamp,
                CbReceiptMoment = ToBclDateTime(System.DateTime.UtcNow),
                FtReceiptCase = 4919338172803973127,
            };
            var response = await _client.SignAsync(request);

            return response.FtReceiptIdentification;
        }


        public async Task<string> FailTransaktionsMultiReceiptAsync()
        {
            ProcessTransactionNumbers(_openTransaktions);
            FiskalConnect();

            var request = new fiskaltrust.ifPOS.v1.ReceiptRequest
            {
                FtCashBoxID = _cashBoxID,
                FtPosSystemId = _NCPosSystemID,
                CbTerminalID = _terminalID,
                CbReceiptReference = "",
                CbReceiptMoment = ToBclDateTime(System.DateTime.UtcNow),
                FtReceiptCase = 4919338172267102219,
                FtReceiptCaseData = _transactionNumbersJson, 
            
                };
            var response = await _client.SignAsync(request);

            return response.FtReceiptIdentification;

        }
        private void ProcessTransactionNumbers(string transactionNumbersText)
        {
            var transactionNumbers = transactionNumbersText
                .Split(',')
                .Select(s => s.Trim()) // Leerzeichen vor und nach der Nummer entfernen
                .Where(s => !string.IsNullOrEmpty(s)) // Leere Einträge filtern
                .Select(int.Parse) // In Ganzzahlen umwandeln
                .ToList();

            // Erzeuge das JSON für die Transaktionsnummern
            var transactionNumbersData = new
            {
                CurrentStartedTransactionNumbers = transactionNumbers
            };

            // JSON-String erstellen und Escape-Zeichen hinzufügen
            _transactionNumbersJson = JsonConvert.SerializeObject(transactionNumbersData);
        }

        public async Task<string> InitiateSCUAsync()
        {
            FiskalConnect();

            var request = new fiskaltrust.ifPOS.v1.ReceiptRequest
            {
                FtCashBoxID = _cashBoxID,
                FtPosSystemId = _NCPosSystemID,
                CbTerminalID = _terminalID,
                CbReceiptReference = "ZeroReceiptAfterFailure",
                CbReceiptMoment = ToBclDateTime(System.DateTime.UtcNow),
                FtReceiptCase = 4919338172283879447,

            };
            var response = await _client.SignAsync(request);

            return response.FtReceiptIdentification;

        }

        public async Task<string> InitiateSCUDeaktivateAsync()
        {
            FiskalConnect();

            var request = new fiskaltrust.ifPOS.v1.ReceiptRequest
            {
                FtCashBoxID = _cashBoxID,
                FtPosSystemId = _NCPosSystemID,
                CbTerminalID = _terminalID,
                CbReceiptReference = "ZeroReceiptAfterFailure",
                CbReceiptMoment = ToBclDateTime(System.DateTime.UtcNow),
                FtReceiptCase = 4919338172267102231,

            };
            var response = await _client.SignAsync(request);

            return response.FtReceiptIdentification;

        }

        public async Task<string> FinishSCUAsync()
        {
            FiskalConnect();

            var request = new fiskaltrust.ifPOS.v1.ReceiptRequest
            {
                FtCashBoxID = _cashBoxID,
                FtPosSystemId = _NCPosSystemID,
                CbTerminalID = _terminalID,
                CbReceiptReference = "ZeroReceiptAfterFailure",
                CbReceiptMoment = ToBclDateTime(System.DateTime.UtcNow),
                FtReceiptCase = 4919338172267102232,

            };
            var response = await _client.SignAsync(request);

            return response.FtReceiptIdentification;

        }

        public async Task<string> ExportJournalAsync(System.DateTime fromDate, System.DateTime toDate)
        {
            FiskalConnect();

            var fromTicks = ToNetTicks(fromDate);
            var toTicks = ToNetTicks(toDate);

            var request = new JournalRequest
            {
                FtJournalType = 4919338167972134914, // Beispieltyp
                From = fromTicks,
                To = toTicks
            };

            var responseStream = _client.Journal(request);

            var journalData = new List<uint>();

            await foreach (var response in responseStream.ResponseStream.ReadAllAsync())
            {
                journalData.AddRange(response.Chunk);
            }

            // Konvertiere journalData in JSON-String
            string json;
            try
            {
                json = System.Text.Json.JsonSerializer.Serialize(journalData, new JsonSerializerOptions { WriteIndented = true });
            }
            catch (Exception ex)
            {
                // Fehlerbehandlung falls die Serialisierung fehlschlägt
                throw new InvalidOperationException("Fehler beim Serialisieren der Journal-Daten.", ex);
            }

            return json;
        }
    }
}


