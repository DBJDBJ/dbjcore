<h1>DBJ CORE</h1>

- [Why?](#why)
- [GitHub Organization Folder aka Why not Git Submodules](#github-organization-folder-aka-why-not-git-submodules)
    - [Working with C# and liking VS Code?](#working-with-c-and-liking-vs-code)
  - [The Actuall Usage](#the-actuall-usage)
    - [Just utilities](#just-utilities)
- [Logging](#logging)
    - [Requirements](#requirements)
    - [Usage](#usage)
    - [Further usage](#further-usage)
- [Dbj Core Configuration](#dbj-core-configuration)
  - [Requirements](#requirements-1)
  - [Usage](#usage-1)


# Why?
DI aka "Dependancy Injection" is basicaly waste of time. Complex OOP with no obvious cause of existence. Thus we have developed this little "dbj net core", from where we use configuration, loggin and little utilities we have collected thorough years. All opinionated.

 **This is not assembly. This is code reuse. Gasp!**

# GitHub Organization Folder aka Why not Git Submodules

### [Working with C# and liking VS Code?](https://code.visualstudio.com/docs/languages/csharp)
 
Don't. Use Visual Studio. Will you allow us to repeat the official advice?

 ***If you want the best possible experience for .NET / C# projects and development on Windows in general, we recommend you use Visual Studio Community.***

 But, not all is lost. `dotnet` cli command is your friend. We find it very easy to quickly setup the project using `dotnet`. Our advise is:

 > VStudio for building and debugging. 'dotnet' cli for easy setup and VSCode for git.

 There are people who do not mix vscode and vstudio and use one or the other. We find both of them lacking some features. Found in the other one.

 > Generally you are encouraged to browse through the code. Decumentation is coming along. Slowly.

 ## The Actuall Usage 

We simply do not use Github submodules. We group projects (repositories) in GIT HUB *organizations folders*. And this is how we clone them, and keep them together, and reference them from each other, under a commom "orgnization folder" root.

That is important since we use Visual Studio solutions, elsewhere on development machines, from where we include other projects, but from the same organization folder. Under which the organization project are cloned.

Example. The '[valstat](https://github.com/valstat)` organization folder happens to be (on my machine): 
```
D:\DEVL\GITHUB\VALSTAT
```
That is the "organization folder". Organizatio folder is just a local folder. At the moment, I have three repositories in that organization folder cloned like so: 
```
D:\DEVL\GITHUB\VALSTAT
├───dbjcore (contains dbjcore.shproj)
├───valstat_csharp (uses dbjcore as "shared code") (sln is in here)
└───valstat_dll_specimen (uses dbjcore as "shared code")
```
"Shared" is Visual Studio shared project concept for simply sharing the code. I is under the organization folder.

`valstat_csharp\valstatcsharp.sln` uses `valstat_dll_specimen\valstat_dll.vcxproj` and `dbjcore\dbjcore.shproj`.

to build `valstatcsharp.sln` you need to clone those two other repositories as well. Under the same organization folder. No submodules required.



### Just utilities

We use, wherever we can this optimization:
```c#
[MethodImpl(MethodImplOptions.AggressiveInlining)]
```
It is not a magic wand. Just a hint to a compiler.
For more just look into the `DBJcore`.

# Logging
### Requirements
Yes [Serilog](https://github.com/serilog/serilog) is better; tens (hunderds) of milions are using [Serilog](https://serilog.net). Loggin utils in here do require executing these 3 cli commands, in the host project:

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
### Usage
Calling through 4 static methods:
```c#
using static DBJLog;

// usage is elswhere 
info( "message" );
debug( "message" );
error( "message" );
fatal( "message" );
```
If used log file will be found here:
```
<project folder parent, full path>\<project folder>\bin\Debug\net7.0\logs\<exe name>YYYMMDD.log
```
That might enrage you. Feel free to go into the code and change it. 

### Further usage

On the top of the dbjcore source: 

```c#
// #define LOG_TO_FILE
```

DBJLog by default otuputs to console as that is the way for container logging.

# Dbj Core Configuration 
Contary to popular belief (prevailing among .NET Lemmings population) .NET core json configuration does not require any specialy named json config files. Any name will do, as long as you make and use your own configuration routines. We just happen to call ours: `appsettings.json`. 

## Requirements

You have to add these lines to the csproj file (that is the hosting project):

```xml
<!-- Required for the configurator to work -->
<ItemGroup>
<Content Include="appsettings.json">
<CopyToOutputDirectory>Always</CopyToOutputDirectory>
</Content>
</ItemGroup>
```
And it is obvious why. And we need to add these to the `csproj` file (of the host project), because we are using .net core for configuration too:

```xml
<ItemGroup>
<PackageReference Include="Microsoft.Extensions.Configuration.Binder" Version="6.0.0" />
<PackageReference Include="Microsoft.Extensions.Configuration.Json" Version="7.0.0" />
<PackageReference Include="Microsoft.Extensions.Configuration.EnvironmentVariables" Version="7.0.0" />
</ItemGroup>
```
## Usage
`DBJCfg.FileName` is the name of the json cfg file (hint: `appsettings.json` ). Content required in this case is:
```json
// appsettings.json
{
  "max_block_count": 64,
  "specimen_blocks": 64,

  "string_to_compress": "Hello World!"
}
```
Usage is simple. There is basically one static method on the `DBJCfg` class. 
```c#
 var max_block_count = DBJCfg.get<short>("max_block_count", 0  );
```
Above says: Find me a key `max_block_count`, cast its value to `short` and return it. If anything goes wrong return the last parameter given. Zero.

Yes, in the .NET universe json files can have comments too.
