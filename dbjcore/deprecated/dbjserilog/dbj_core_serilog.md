<h1>DBJ CORE LOGGING</h1>

<h2>OBSOLETE CONCEPT.</h2>
<h3>Please migrate to the XXI century. Use dbj kontalog. Write code to run in containers only.</h3>

> 
> &nbsp;
> 
> Still here? OK please proceed
> 
> &nbsp;
> 

# Logging
## Requirements
Well [Serilog](https://github.com/serilog/serilog) is the ruler; tens (hunderds) of milions are using [Serilog](https://serilog.net). Logging utils in here do require executing these 3 cli commands, in the host project:

```
// do this in the folder where the host project csproj is
// dbjcore is code reused
$ dotnet add package Serilog
$ dotnet add package Serilog.Sinks.Console
$ dotnet add package Serilog.Sinks.File
```
Let us repeat: above has ot be done in the host project. There is no csproj in the dbjcore. Above will add these 3 lines in the csproj file:
```xml
  <ItemGroup>
    <PackageReference Include="serilog" Version="2.12.0" />
    <PackageReference Include="Serilog.Sinks.Console" Version="4.1.0" />
    <PackageReference Include="serilog.sinks.file" Version="5.0.0" />
  </ItemGroup>
```
## Usage
Calling is through 4 static methods:
```c#
using static DBJLog;

// usage is elswhere 
info( "message" );
debug( "message" );
error( "message" );
fatal( "message" );
```
If used (as logging target) log file will be found here:
```
<project folder parent, full path>\<project folder>\bin\Debug\net7.0\logs\<exe name>YYYMMDD.log
```
That might enrage you. Feel free to go into the code and change it. 

>
>
> Philosophy: if you need some change feel free to D.I.Y.
> 
> 

## You like to worry about log files?

On the top of the `dbjcore.cs` source you can spot: 

```c#
// #define LOG_TO_FILE
```

DBJLog by default outputs to console as that is the only sane way for container logging. Which in turn makes it insane to use Serilog, NLOG or whatever_log.

## Those pesky log levels

Dive into the code. DBJLog constructor contains simple check to show you what are they and what is the default DEBUG situation in respect to the log levels.

After a while you can simply switch off that in your debug builds. Reading the code you already understand how.
