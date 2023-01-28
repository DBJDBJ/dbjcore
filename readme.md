# DBJ CORE

dbj net core; reusable (and opinionated)

 **This is not assembly. This is code reuse. Gasp!**

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
