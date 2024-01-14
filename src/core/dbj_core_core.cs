/*
 This code is (c) by dbj@dbj.org, CC BY SA 4.0
*/
// we log to console, that is all that one needs
// #define LOG_TO_FILE
#if DEBUG
#define DBJLOG_LEVEL_CHECK
#endif
//#define KONTALOG_HANDLES_SQLSVR_CLIENT_EXCEPTION

#region usings_declarations

#nullable enable
// #define LOG_TO_FILE
// above redirects Writeln to log.info and Writerr to log.error
// using System;
// using System.IO;
//using System;
//using System.Diagnostics;
//using System.IO;
using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.Extensions.Configuration;
#if KONTALOG_HANDLES_SQLSVR_CLIENT_EXCEPTION
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Extensions.Configuration;
// need Microsoft.Data.SqlClient.SqlException
#endif

// use like this:
// using static DBJcore;
// after which you can just use the method names 
// from the class DBJcore in here
// without a class name and dot in the front, for example:
// Writeln( Whoami() );

#endregion usings_declarations

namespace dbjcore;

#region common utilities
// we keep it in the global names space
public sealed class DBJcore
{

    /// <summary>
    /// 
    /// by default return the friendly exe name
    /// 
    /// if arg is false on the local machine this returns a machine name
    /// on azure or linux container code tis will return something
    /// enitrely different
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string my_domain(bool exe_name_ = true)
    {
        /* <ROADMAP>
         * System.AppDomain.CurrentDomain.FriendlyName 
         * - Returns the filename with extension (e.g. MyApp.exe).

System.Diagnostics.Process.GetCurrentProcess().ProcessName 
        - Returns the filename without extension (e.g. MyApp).

System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName 
        - Returns the full path and filename (e.g. C:\Examples\Processes\MyApp.exe). 
        You could then pass this into System.IO.Path.GetFileName() 
        or System.IO.Path.GetFileNameWithoutExtension() 
        to achieve the same results as the above.
         */
        if (exe_name_ == true)
        {
            return System.AppDomain.CurrentDomain.FriendlyName;
        }

        Lazy<string> lazy_ = new Lazy<string>(
            () => Environment.UserDomainName.ToString()
        );
        return lazy_.Value;
    }

    /// <summary>
    /// Return the name of the calling assembly or executable
    /// unles the argument is true
    /// where this assembly is "exactly what is says on the tin"
    /// aka this assembly
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ThisName(bool this_name = false)
    {
        if (this_name == true)
        {
            return Assembly.GetExecutingAssembly().GetName().FullName;
        }
        return Assembly.GetCallingAssembly().GetName().FullName;

    }

    /// <summary>
    /// Random word made from GUID
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string guid_word(bool up_ = false)
    {
        // wow, what a cludge, don't do this at home ;)
        // wait of 1 mili sec so that guids 
        // are sufficiently different
        System.Threading.Thread.Sleep(1);
        string input = Guid.NewGuid().ToString("N");
        input = up_ ? input.ToUpper() : input.ToLower();
        // remove numbers
        return Regex.Replace(input, @"[\d-]", string.Empty);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string local_now()
    {
        return DateTime.Now.ToLocalTime().ToString();
    }

    // Console.WriteLine("Hello my UTC timestamp      : " + iso8601(1));
    // Console.WriteLine("Hello iso8601 with 'T'      : " + iso8601(2));
    // Console.WriteLine("Hello full iso8601          : " + iso8601(3));
    // Console.WriteLine("Hello  my local timestamp   : " + iso8601());
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string iso8601(short kind_ = 0)
    {
        switch (kind_)
        {
            case 1:
                // no 'T' fromat e.g. "2023-07-15 05:56:27"
                return DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            case 2:
                // official 'T' format e.g.  2023-07-15T07:56:27
                return DateTime.Now.ToLocalTime().ToString("s", System.Globalization.CultureInfo.InvariantCulture);
            case 3:
                // a sortable date/time pattern; conforms to ISO 8601.
                return DateTime.Now.ToLocalTime().ToString("o", System.Globalization.CultureInfo.InvariantCulture);
            default:
                // default e.g. 2023-07-15 07:56:27
                return DateTime.Now.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
        }
    }

    /// <summary>
    /// Return the name of the caller
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Whoami([CallerMemberName] string? caller_name = null)
    {
        if (string.IsNullOrEmpty(caller_name))
            return "unknown";
        if (string.IsNullOrWhiteSpace(caller_name))
            return "unknown";
        return caller_name;
    }

    /// <summary>
    /// Return the file name and source line number to the caller
    /// returned is a record type
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static (string File, int Line) FileLineInfo(
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        // make and return a record
        return new(filePath, lineNumber);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Assert(bool condition_)
    {
        System.Diagnostics.Debug.Assert(condition_);
        return condition_;
    }

    /*
     * string test = "Testing 1-2-3";

    // convert string to stream
    byte[] byteArray = Encoding.ASCII.GetBytes(test);
    MemoryStream stream = new MemoryStream(byteArray);

    // convert stream to string
    StreamReader reader = new StreamReader(stream);
    string text = reader.ReadToEnd();
     */
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static System.IO.Stream ToStream(string sval_)
    {
        byte[] byteArray = Encoding.ASCII.GetBytes(sval_);
        return new MemoryStream(byteArray);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToString(System.IO.Stream sval_)
    {
        StreamReader reader = new StreamReader(sval_);
        return reader.ReadToEnd();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToUTF8(string sval_)
    {
        byte[] bytes = Encoding.Default.GetBytes(sval_);
        return Encoding.UTF8.GetString(bytes);
    }

    /// <summary>
    /// retun the local IP as a string
    /// if "127.0.0.1" is returned obviously there is no network
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string local_ip()
    {
        Lazy<string> lazyIP = new Lazy<string>(
            () =>
            {
                using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
                {
                    try
                    {
                        socket.Connect("8.8.8.8", 65530);
                        IPEndPoint endPoint = (IPEndPoint)socket.LocalEndPoint!;
                        return endPoint!.Address.ToString();
                    }
                    catch (Exception)
                    {
#if DEBUG
                        Kontalog.error($"No local IP found from the socket");
#endif
                        return "127.0.0.1";
                    }
                }
            }
        );

        return lazyIP.Value;
    }

} // Program_context

#endregion common utilities



