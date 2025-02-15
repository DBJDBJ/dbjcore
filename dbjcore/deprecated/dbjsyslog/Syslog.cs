using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
/*
 public class Program
{
    public static void Main(string[] args)
    {
        // Initialize first
        SyslogClient.Initialize("syslog.example.com");

        // Then you can use it in three ways:

        // 1. Using the Instance property
        SyslogClient.Instance.SendMessage("Using instance directly");

        // 2. Using static helper methods
        SyslogClient.Log("Using static helper");

        // 3. Using dependency injection (recommended)
        var syslogClient = SyslogClient.Instance;
        syslogClient.SendMessage("Using injected instance");
    }
}
 */


namespace dbjcore
{

public class Syslog : IDisposable
    {
        private readonly string _hostname;
        private readonly int _port;
        private UdpClient? _udpClient;
        private bool _disposed;
        private static Syslog ? _instance;
        private static readonly object _lock = new object();

        public static Syslog Instance
        {
            get
            {
                if (_instance == null)
                {
                    throw new InvalidOperationException("Syslog not initialized. Call Initialize first.");
                }
                return _instance;
            }
        }
        /*
         * yes I know ... needs to be called
         */
        public static Syslog Initialize(string hostname = "localhost", int port = 514)
        {
            lock (_lock)
            {
                if (_instance != null)
                {
                    throw new InvalidOperationException("SyslogClient already initialized.");
                }
                _instance = new Syslog(hostname, port);
                AppDomain.CurrentDomain.ProcessExit += (s, e) => _instance.Dispose();
                // If using ASP.NET Core, you can also use:
                // applicationLifetime.ApplicationStopping.Register(() => _instance.Dispose());
                return _instance;
            }
        }

        // Example static method for direct usage without getting Instance
        public static void Log(string message, SyslogSeverity severity = SyslogSeverity.Informational)
        {
            Instance.SendMessage(message, severity);
        }

        private Syslog(string hostname, int port = 514)
        {
            _hostname = hostname;
            _port = port;
            _udpClient = new UdpClient();
        }

        public void SendMessage(string message, SyslogSeverity severity = SyslogSeverity.Informational, SyslogFacility facility = SyslogFacility.User)
        {
            ThrowIfDisposed();
            int priority = ((int)facility * 8) + (int)severity;
            string timestamp = DateTime.Now.ToString("MMM dd HH:mm:ss");
            string hostname = Dns.GetHostName();

            string syslogMessage = $"<{priority}>{timestamp} {hostname} {message}";
            byte[] messageBytes = Encoding.ASCII.GetBytes(syslogMessage);

            _udpClient!.Send(messageBytes, messageBytes.Length, _hostname, _port);
        }

        public void SendMessage(string format, object[] args, SyslogSeverity severity = SyslogSeverity.Informational, SyslogFacility facility = SyslogFacility.User)
        {
            ThrowIfDisposed();
            string message = string.Format(format, args);
            SendMessage(message, severity, facility);
        }

        public async Task SendMessageAsync(string message, SyslogSeverity severity = SyslogSeverity.Informational, SyslogFacility facility = SyslogFacility.User)
        {
            ThrowIfDisposed();
            int priority = ((int)facility * 8) + (int)severity;
            string timestamp = DateTime.Now.ToString("MMM dd HH:mm:ss");
            string hostname = Dns.GetHostName();

            string syslogMessage = $"<{priority}>{timestamp} {hostname} {message}";
            byte[] messageBytes = Encoding.ASCII.GetBytes(syslogMessage);

            await _udpClient!.SendAsync(messageBytes, messageBytes.Length, _hostname, _port);
        }

        public async Task SendMessageAsync(string format, object[] args, SyslogSeverity severity = SyslogSeverity.Informational, SyslogFacility facility = SyslogFacility.User)
        {
            ThrowIfDisposed();
            string message = string.Format(format, args);
            await SendMessageAsync(message, severity, facility);
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(Syslog));
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _udpClient?.Close();
                    _udpClient?.Dispose();
                    if (_udpClient is not null)
                    _udpClient = null;
                }
                _disposed = true;
            }
        }

        ~Syslog()
        {
            Dispose(false);
        }
    }

    // Enums remain the same...

    public enum SyslogSeverity
    {
        Emergency = 0,
        Alert = 1,
        Critical = 2,
        Error = 3,
        Warning = 4,
        Notice = 5,
        Informational = 6,
        Debug = 7
    }

    public enum SyslogFacility
    {
        Kernel = 0,
        User = 1,
        Mail = 2,
        System = 3,
        Security = 4,
        Syslog = 5,
        Printer = 6,
        Network = 7,
        UUCP = 8,
        Clock = 9,
        Security2 = 10,
        FTP = 11,
        NTP = 12,
        LogAudit = 13,
        LogAlert = 14,
        Clock2 = 15,
        Local0 = 16,
        Local1 = 17,
        Local2 = 18,
        Local3 = 19,
        Local4 = 20,
        Local5 = 21,
        Local6 = 22,
        Local7 = 23
    }
}
