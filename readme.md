# DBJ CORE

DI aka "Dependany Injection" is basicaly waste of time. Thus we have developed this little "dbj net core". Reusable (and opinionated)

 **This is not assembly. This is code reuse. Gasp!**

 Please use it as git submodule.


### [Working with C# & VS Code?](https://code.visualstudio.com/docs/languages/csharp)
 
Allow us to repeat the official advice:

 ***If you want the best possible experience for .NET / C# projects and development on Windows in general, we recommend you use Visual Studio Community.***

 But. `dotnet` cli command is your friend. We find it very easy to quickly setup the project using `dotnet`. Our advise is:

 > VStudio for building and debugging. 'dotnet' cli for easy setup and VSCode for git.

 There are people who do not mix vscode and vstudio and use one or the other. We find both of them lacking some features. Found in the other one.

 Generally you are encouraged to browse through the code. Decumentation is coming along. Slowly.

## Just utilities

Use like this:
```c#
using static dbjcore;
```
after which you can just use the method names 
from the class utl in there, without a class name and dot in front, for example:
```c#
Writeln( Whoami() );
```
Namespace? Well yes, the code is yours (copyright is mine) so either keep it in global namespace or some of yours namespaces.
By default is is `namespace dbjcore;`. We (all) think you can deal with that on your own.

## Logging

Yes serilog is better. Loggin utils in here do require executing these 3 cli commands, in the host project:

```
$ dotnet add package Serilog
$ dotnet add package Serilog.Sinks.Console
$ dotnet add package Serilog.Sinks.File
```
Which will add these 3 lines in the csproj file:
```xml
  <ItemGroup>
    <PackageReference Include="serilog" Version="2.12.0" />
    <PackageReference Include="Serilog.Sinks.Console" Version="4.1.0" />
    <PackageReference Include="serilog.sinks.file" Version="5.0.0" />
  </ItemGroup>
```
Calling through 4 static methods:
```c#
Log.info( "message" );
Log.debug( "message" );
Log.error( "message" );
Log.fatal( "message" );
```
Log file will be found here:
```
<project folder parent, full path>\<project folder>\bin\Debug\net7.0\logs\<exe name>YYYMMDD.log
```

- Find the `DO_NOT_CONSOLE` in the dbjcore source. 
- Depending on it existence 
  - `DBJCore.Writeln("message")` might output to console or to `Log.info( "message" )`.
  - `DBJCore.Writerr("message")` might output to console or to `Log.error( "message" )`.
  - `DBJCore.Writedbg("message")` might output to console or to `Log.debug( "message" )`.


## Configuration 
.NET core json confiugration does not require any specialy named json config files.
We just happen to call it `appsettings.json`. Thus we have to add these lines to the csproj file:

```xml
<!-- Required for the configurator to work -->
<ItemGroup>
<Content Include="appsettings.json">
<CopyToOutputDirectory>Always</CopyToOutputDirectory>
</Content>
</ItemGroup>
```

Amd we need to add these, because we are using .net core for configuration too:

```xml
<ItemGroup>
<PackageReference Include="Microsoft.Extensions.Configuration.Binder" Version="6.0.0" />
<PackageReference Include="Microsoft.Extensions.Configuration.Json" Version="7.0.0" />
<PackageReference Include="Microsoft.Extensions.Configuration.EnvironmentVariables" Version="7.0.0" />
</ItemGroup>
```
