using System;
using System.IO;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Nt.Utility.Tools
{
    internal class Ping
    {
        private string _logFilePath;
        private bool _isPinging;
        private CancellationTokenSource _cancellationTokenSource;

        public Ping()
        {
            // Pfad zur Textdatei im selben Verzeichnis wie die EXE
            //_logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Nt.Utility.Ping-LOG.txt");
            _logFilePath = Path.Combine("C:\\Temp", "Nt.Utility.Ping-LOG.txt") ;
        }

        public async Task StartPingingAsync(string server, string port)
        {
            if (_isPinging)
            {
                return;
            }

            _isPinging = true;
            _cancellationTokenSource = new CancellationTokenSource();

            while (!_cancellationTokenSource.Token.IsCancellationRequested)
            {
                string result;

                if (string.Equals(port, "PORT", StringComparison.OrdinalIgnoreCase) || string.Equals(port, "", StringComparison.OrdinalIgnoreCase))
                {
                    // ICMP-Ping
                    result = await IcmpPingAsync(server);
                }
                else if (int.TryParse(port, out int parsedPort))
                {
                    // TCP-Ping
                    result = await TcpPingAsync(server, parsedPort);
                }
                else
                {
                    result = $"Invalid port specified: {port}\n";
                }

                await LogToFileAsync(result);
                await Task.Delay(1000); // 1 Sekunde warten zwischen den Pings
            }
        }

        public void StopPinging()
        {
            if (_isPinging)
            {
                _cancellationTokenSource.Cancel();
                _isPinging = false;
            }
        }

        private async Task<string> TcpPingAsync(string server, int port)
        {
            try
            {
                using (var client = new TcpClient())
                {
                    var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                    var connectTask = client.ConnectAsync(server, port);
                    var delayTask = Task.Delay(1000); // 1 Sekunde warten

                    // Warten auf das Connect oder den Timeout
                    if (await Task.WhenAny(connectTask, delayTask) == connectTask)
                    {
                        stopwatch.Stop();
                        var roundTripTime = stopwatch.ElapsedMilliseconds;
                        return $"TCP Ping to {server}:{port} successful at {DateTime.Now} - Time: {roundTripTime}ms\n";
                    }
                    else
                    {
                        stopwatch.Stop();
                        return $"TCP Ping to {server}:{port} failed (timeout) at {DateTime.Now}\n";
                    }
                }
            }
            catch (Exception ex)
            {
                return $"TCP Ping to {server}:{port} failed with error: {ex.Message}\n";
            }
        }

        private async Task<string> IcmpPingAsync(string server)
        {
            try
            {
                using (var ping = new System.Net.NetworkInformation.Ping())
                {
                    PingReply reply = await ping.SendPingAsync(server, 1000); // 1 Sekunde Timeout
                    if (reply.Status == IPStatus.Success)
                    {
                        return $"ICMP Ping to {server} successful at {DateTime.Now} - Time: {reply.RoundtripTime}ms\n";
                    }
                    else
                    {
                        return $"ICMP Ping to {server} failed with status: {reply.Status} at {DateTime.Now}\n";
                    }
                }
            }
            catch (Exception ex)
            {
                return $"ICMP Ping to {server} failed with error: {ex.Message}\n";
            }
        }

        private async Task LogToFileAsync(string text)
        {
            try
            {
                await File.AppendAllTextAsync(_logFilePath, text);
            }
            catch (Exception ex)
            {
                // Hier könntest du optional eine andere Logging-Methode einbauen oder eine Exception werfen
                Console.WriteLine($"Failed to write to log file: {ex.Message}");
            }
        }
    }
}
