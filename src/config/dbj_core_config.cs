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

///////////////////////////////////////////////////////////////////////////////////////
#region configuration
///////////////////////////////////////////////////////////////////////////////////////

/*

-------------------------------------------------------------------------------------------------------
We/I am use/ing json as config file format
------------------------------------------------------------------------------------------------------ -

Ditto in the proj file there must be information where is the config json file
and what is its name. Like so:
    
<ItemGroup>
<Content Include="appsettings.json">
<CopyToOutputDirectory>Always</CopyToOutputDirectory>
</Content>
</ItemGroup>

followed with section that adds the dot net components to use from the code

<ItemGroup>
<PackageReference Include="Microsoft.Extensions.Configuration.Binder" Version="6.0.0" />
<PackageReference Include="Microsoft.Extensions.Configuration.Json" Version="7.0.0" />
<PackageReference Include="Microsoft.Extensions.Configuration.EnvironmentVariables" Version="7.0.0" />
</ItemGroup>
*/
public sealed class DBJCfg
{
    IConfiguration config;
    public readonly string config_file_name = string.Empty;

    
    /*
    config file name is completely 100% arbitrary
    it is hidden as def. val constructor parameter

    I know that is not a good thing, but I am lazy , and besides that
    How to configure the configuration? Externaly? Probably using the cli arguments?
    */
    public DBJCfg(string json_config_file = "appsettings.json")
    {
        config_file_name = json_config_file;
        // Build a config object, using env vars and JSON providers.
        config = new ConfigurationBuilder()
      .AddJsonFile(json_config_file)
      .AddEnvironmentVariables()
      .Build();
    }

    // given path return value as string or empty string 
    public string kpv(string path_)
    {
        try
        {
            var section_ = this.config.GetRequiredSection(path_);
#if DBJ_TRACE
            Log.info($"cfg {DBJcore.Whoami()}() for path: '{path_}'  -- key: '{section_.Key}', val: '{section_.Value}'");
#endif
            return section_.Value!;
        }
        catch (Exception x_)
        {
#if DEBUG
            Kontalog.error($"No element in the cfg json found for the path: '{path_}'");
#endif
            Kontalog.error(x_.ToString());
        }
        return string.Empty;
    }

    /* 
    given path return the value or default value cast to the required type
    */
    T read<T>(string path_, T default_)
    {
        try
        {
            var section_ = this.config.GetRequiredSection(path_);
#if DBJ_TRACE
            Log.info($"cfg {DBJcore.Whoami()}() -- path: '{path_}'  key:'{section_.Key}', val: '{section_.Value}'");
#endif
            if (typeof(T).IsValueType && default_ == null)
            {
                // Handle value types with null default values
                default_ = Activator.CreateInstance<T>();
            }
            return section_.Get<T>() ?? default_;
        }
        catch (Exception x_)
        {
#if DEBUG
            Kontalog.error($"No element found for the path: '{path_}'");
#endif
            Kontalog.error(x_.ToString());
        }
        return default_; // not: default(T)!;
    }

    // this is where cfg is made on demand once 
    // and not before it is called for the first time
    static Lazy<DBJCfg> lazy_cfg = new Lazy<DBJCfg>(() => new DBJCfg());

    static public DBJCfg instance { get { return lazy_cfg.Value; } }
    //
    // calling will lazy load the Configurator and then use it
    //
    // var ip1 = Cfg.get<string>("ip1", "192.168.0.0");
    //
    public static T get<T>(string path_, T dflt_) { return instance.read<T>(path_, dflt_); }

    public static string FileName { get { return instance.config_file_name; } }

} // Config standardorum superiorum

#endregion configuration



