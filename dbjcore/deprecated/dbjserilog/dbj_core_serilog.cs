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

// for adding Serilog must do this in the folder where the host project csproj is
// dbjcore is code reused
// $ dotnet add package Serilog
// $ dotnet add package Serilog.Sinks.Console
// $ dotnet add package Serilog.Sinks.File
using Serilog;
using static System.Runtime.InteropServices.JavaScript.JSType;

// use like this:
// using static DBJcore;
// after which you can just use the method names 
// from the class DBJcore in here
// without a class name and dot in the front, for example:
// Writeln( Whoami() );

#endregion usings_declarations

namespace dbjcore;


///////////////////////////////////////////////////////////////////////////////////////
#region logging
///////////////////////////////////////////////////////////////////////////////////////

/*
do this in the folder where the host project csproj is

$ dotnet add package Serilog
$ dotnet add package Serilog.Sinks.Console
$ dotnet add package Serilog.Sinks.File
*/
[Obsolete("Migrate to the XXI century.Use dbj kontalog.")] 
public sealed class DBJLog : System.IDisposable
{
    public readonly static string text_line = "-------------------------------------------------------------------------------";
    public readonly static string app_friendly_name = System.AppDomain.CurrentDomain.FriendlyName;

    static string app_name
    {
        get
        {
            return app_friendly_name; //  app_friendly_name.Substring(0, app_friendly_name.IndexOf('.'));
        }
    }

    // is this a trick? hack? a cludge? no.
    // this method is deliberately not static
    // so that instance must be made to use it
    // Why? :wink:
    public Serilog.Events.LogEventLevel enabled_level()
    {
        if (Serilog.Log.IsEnabled(Serilog.Events.LogEventLevel.Verbose)) return Serilog.Events.LogEventLevel.Verbose;
        if (Serilog.Log.IsEnabled(Serilog.Events.LogEventLevel.Debug)) return Serilog.Events.LogEventLevel.Debug;
        if (Serilog.Log.IsEnabled(Serilog.Events.LogEventLevel.Information)) return Serilog.Events.LogEventLevel.Information;
        if (Serilog.Log.IsEnabled(Serilog.Events.LogEventLevel.Warning)) return Serilog.Events.LogEventLevel.Warning;
        if (Serilog.Log.IsEnabled(Serilog.Events.LogEventLevel.Error)) return Serilog.Events.LogEventLevel.Error;
        if (Serilog.Log.IsEnabled(Serilog.Events.LogEventLevel.Fatal)) return Serilog.Events.LogEventLevel.Fatal;

        // this is true example when exception has a role
        // in exceptional situations only
        throw new Exception("method " + DBJcore.Whoami() + ": This should never happen");

    }


#if LOG_TO_FILE
    // this path is obviously deeply wrong :P
    // ROADMAP: it will be externaly configurable
    public readonly static string log_file_path_template_ = "{0}logs\\{1}.log";
#endif
    public DBJLog()
    {
        // https://github.com/serilog/serilog/wiki/Configuration-Basics#minimum-level
        // default level is Information and that is considered for a production system

        // default is not file but console logging
        // all that container logging needs is to console only
        // that goes to container logs and is collected by log agents/dispatchers
#if LOG_TO_FILE
        string log_file_path_ = string.Format(log_file_path_template_, AppContext.BaseDirectory, app_name);
        Serilog.Log.Logger = new LoggerConfiguration()
           .WriteTo.File(log_file_path_, rollingInterval: RollingInterval.Day)
#if DEBUG
           .MinimumLevel.Debug()
#endif
           .CreateLogger();
#else
        Serilog.Log.Logger = new LoggerConfiguration()
#if DEBUG
    .MinimumLevel.Debug()
#else
    // WARNING: Serilog default level is Info, we move it to Fatal
    // in production we want only fatal messages
    .MinimumLevel.Fatal()
#endif
    .WriteTo.Console()
    .CreateLogger();
#endif

#if LOG_TO_FILE
        DBJLog.log_file_header(log_file_path_);
#endif

#if DEBUG
#if DBJLOG_LEVEL_CHECK
        // we found this as an very wellcome de-confuzor for many users
        Serilog.Events.LogEventLevel? what_level = enabled_level();

        Serilog.Log.Debug(" ");
        Serilog.Log.Debug("dbj_core: Log level check. Using Serilog");
        Serilog.Log.Debug(" ");
        Serilog.Log.Fatal("FATAL is on the top level");
        Serilog.Log.Error("ERROR is bellow FATAL");
        Serilog.Log.Warning("WARNING is bellow ERROR");
        Serilog.Log.Information("INFORMATION is bellow WARNING");
        Serilog.Log.Debug("DEBUG is bellow INFORMATION");
        Serilog.Log.Verbose("VERBOSE is on the bottom level");
        Serilog.Log.Debug(" ");
        Serilog.Log.Debug("dbj_core: this is DEBUG build and default is .MinimumLevel.Debug() ");
        Serilog.Log.Debug("dbj_core: result of enabled_level() is: " + what_level.ToString());
        Serilog.Log.Debug(" ");
        Serilog.Log.Debug("HINT: In production we want fatal level only.");
        Serilog.Log.Debug(" ");

        what_level = null;
        // if minimum level is DEBUG only Verbose will not be shown
        // https://github.com/serilog/serilog/wiki/Configuration-Basics#minimum-level
        // default level is Information; considered for a production 
#endif // DBJLOG_LEVEL_CHECK
#endif

    } // Main()

#if LOG_TO_FILE
    static void log_file_header(string log_file_path_)
    {
        Serilog.Log.Information(text_line);
        Serilog.Log.Information($"[{System.DateTime.Now.ToLocalTime().ToString()}] Starting {app_name}");
        Serilog.Log.Information($"Launched from {Environment.CurrentDirectory}");
        Serilog.Log.Information($"Physical location {AppDomain.CurrentDomain.BaseDirectory}");
#if DEBUG
        Serilog.Log.Debug($"AppContext.BaseDir {AppContext.BaseDirectory}");
        Serilog.Log.Debug("This app is built in a DEBUG mode");
#endif
        Serilog.Log.Information(text_line);
        ProcessModule? pm_ = Process.GetCurrentProcess().MainModule;
        if (pm_ != null)
        {
            Serilog.Log.Information($"Runtime Call {Path.GetDirectoryName(pm_.FileName)}");
        }
        Serilog.Log.Information($"Log file location:{log_file_path_}");
        Serilog.Log.Information(text_line);
    }
#endif

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void debug_(string msg_)
    {
        Serilog.Log.Debug(msg_);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void info_(string msg_) { Serilog.Log.Information(msg_); }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void error_(string msg_) { Serilog.Log.Error(msg_); }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void fatal_(string msg_) { Serilog.Log.Fatal(msg_); }

    // this is where log instance is made on demand once and not before called the first time
    static System.Lazy<DBJLog> lazy_log = new Lazy<DBJLog>(() => new DBJLog());
    private bool disposedValue;

    // use this to create the DBJLog instance (through its contructor) at least once 
    static public DBJLog logger { get { return lazy_log.Value; } }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void dispatch_(
        Action<string> log_, string format, params object[] args
        )
    {
        if (args.Length < 1)
            log_(format);
        else
        {
            log_(string.Format(format, args));
        }
    }

    // calling one of these will lazy load the loger and then use it
    // repeated calls will reuse the same instance
    // log.info("where is this going then?");
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void debug(string format, params object[] args)
    {
        dispatch_((msg_) => logger.debug_(msg_), format, args);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void info(string format, params object[] args)
    {
        dispatch_((msg_) => logger.info_(msg_), format, args);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void error(string format, params object[] args)
    {
        dispatch_((msg_) => logger.error_(msg_), format, args);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void fatal(string format, params object[] args)
    {
        dispatch_((msg_) => logger.fatal_(msg_), format, args);
    }
    //-----------------------------------------------------------------
    ~DBJLog()
    {
        Dispose(false);
    }

    private void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                Serilog.Log.CloseAndFlush();
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            disposedValue = true;
        }
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~DBJLog()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }

    void IDisposable.Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

}

#endregion logging
