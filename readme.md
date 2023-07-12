<h1>DBJ CORE</h1>

- [Why?](#why)
- [Folder Organization aka Why not Git Submodules](#folder-organization-aka-why-not-git-submodules)
    - [You are working with C# and liking VS Code (a lot)?](#you-are-working-with-c-and-liking-vs-code-a-lot)
  - [The Actuall Usage](#the-actuall-usage)
    - [Beware of the siren call of the git submodules. Resist.](#beware-of-the-siren-call-of-the-git-submodules-resist)
  - [Side advice (out of the blue)](#side-advice-out-of-the-blue)
- [dbjcore services \*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*](#dbjcore-services-)
- [Logging](#logging)
  - [Requirements](#requirements)
  - [Usage](#usage)
  - [Further usage features](#further-usage-features)
  - [Those pesky log levels](#those-pesky-log-levels)
- [Dbj Core Configuration Service](#dbj-core-configuration-service)
  - [dbjcore config requirements](#dbjcore-config-requirements)
  - [Usage](#usage-1)


# Why?
<h2>&nbsp;</h2>
Here is why ...
<h2>&nbsp;</h2>

![](media/supersimplecode.png)
<h2>&nbsp;</h2>
To add. DI aka "Dependancy Injection" is basicaly waste of time. Complex OOP with no obvious cause of existence. Especially for simple one man team projects. Thus we have developed this little "dbj net core", from where we use configuration, loggin and little utilities we have collected thorough years. 

<h2>&nbsp;</h2>

 **This is not assembly. This is code reuse. Gasp!**

<h2>&nbsp;</h2>

# Folder Organization aka Why not Git Submodules

### You are [working with C# and liking VS Code (a lot)?](https://code.visualstudio.com/docs/languages/csharp)
 
Don't. Use Visual Studio. Allow us to repeat the official advice from the VS Code page:
 >
 > ***If you want the best possible experience for .NET / C# projects and development on Windows in general, we recommend you use Visual Studio Community.***
 >
 
 But, not all is lost (even for you). `dotnet` cli command is your friend. We find it very easy to quickly setup the project using `dotnet`. Our advice is:

 > VStudio for building and debugging. 'dotnet' cli for easy setup and VSCode for git.

 There are people who do not mix vscode and vstudio and use one or the other. We find both of them lacking some features. Found in the other one.

 > 
 > Generally you are encouraged to browse through the code here. Decumentation is coming along. Slowly.
 > 

 ## The Actuall Usage 

We have suffered github submodules. No point in hiding it :wink:

--- 
***You are strongly advised to use VisualStudio and use `dbjcore.shproj` found in here.***

---
We simply do not use Github submodules, the only submodule we use is this repository. We cluster projects (repositories) (on our local disks) in GIT HUB *organizations folders*. And this is how we clone them, and keep them together, and reference them from each other, under a commom "organization folder" root folder.

That is important since we use Visual Studio solutions, elsewhere on development machines, from where we include other projects, but from the same organization folder. Under which the organization project are cloned. 

Sounds confuzing. Example. The '[valstat](https://github.com/valstat)` "organization folder",  happens to be (on my machine) over here: 
```
D:\DEVL\GITHUB\VALSTAT
```
That is the "organization folder". Organizatio folder is just a local root folder. At the moment, I have three repositories in that organization folder cloned like so: 
```
D:\DEVL\GITHUB\VALSTAT
├───dbjcore (contains dbjcore.shproj)
├───valstat_csharp (uses dbjcore as "shared code" Visual Studio feature) (sln is in here)
└───valstat_dll_specimen (uses dbjcore as "shared code" too)
```
"Shared" is Visual Studio shared project mechanism for simply sharing the code. 

`valstat_csharp\valstatcsharp.sln` uses `valstat_dll_specimen\valstat_dll.vcxproj` and `dbjcore\dbjcore.shproj`.

to build `valstatcsharp.sln` you need to clone those two other repositories as well. Under the same organization folder. No submodules required. `dbjcore` only.

### Beware of the siren call of the git submodules. Resist.

So where do we keep re-usable code? In `dbjcore` where else.  This is mainly possible because we do not like multithreading, we like multiprocessing. And we build inside "fat containers". Container with decoupled processes inside.  

In 2023, [DAPR](https://dapr.io/) , is one ready made example. fat-container (or process) with a lot of modules inside. 

The outcome is our code is small(ish) and it is decoupled in remote services. Thus `dbjcore` is all we need to be reused. 

That is not entirely true, and we keep on adding stuff to it. It is kind-of-a our own "side car". We certainly hope it will not grow to those proportions. And complexity.

## Side advice (out of the blue)

We use, wherever we can this .NET optimization:
```c#
[MethodImpl(MethodImplOptions.AggressiveInlining)]
```
It is not a magic wand. Just a hint to a compiler. But very effective.
For more just look into the `DBJcore`.

<hr/>

# dbjcore services **********************************************

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

## Further usage features

On the top of the dbjcore source you can spot: 

```c#
// #define LOG_TO_FILE
```

DBJLog by default otuputs to console as that is the way for container logging.

## Those pesky log levels

Dive into the code. DBJLog constructor contains simple check to show you what are they and what is the default DEBUG situation in respect to the log levels.

After a while you can simply switch off that in your debug builds. Reading the code you already understand how.

# Dbj Core Configuration Service

Contrary to popular belief (prevailing among the .NET Lemmings population) .NET core json configuration does not require any specialy named json config files. Any name will do, as long as you make and use your own configuration routines. We just happen to call ours: `appsettings.json`. You can call yours `iliketobedifferent.js`.

Again if you want your own name dive into the code please.

## dbjcore config requirements

You have to add these lines to the csproj file (that is the hosting project):

```xml
<!-- Required for the configurator to work -->
<ItemGroup>
<Content Include="appsettings.json">
<CopyToOutputDirectory>Always</CopyToOutputDirectory>
</Content>
</ItemGroup>
```
And it is obvious why. That tells you want to include `appsettings.json`, and you want it copied to the output directory upon building. Also we need to add these three packages :
```bash
dotnet add package Microsoft.Extensions.Configuration.Binder
dotnet add package Microsoft.Extensions.Configuration.Json
dotnet add package Microsoft.Extensions.Configuration.EnvironmentVariables
```
Let us repeat: do that in the folder where the csproj of the host project is. Not the sln but the csproj file.

## Usage

`DBJCfg.FileName` is the name of the json cfg file (hint: `appsettings.json` ). In case you have a itch to change it. Content requirements (in this readme file kind-of-a-terse-explanation), are:
```json
// appsettings.json example for the usage explanations bellow
{
  "max_block_count": 64,
  "specimen_blocks": 64,

  "string_to_compress": "Hello World!"
}
```
Usage is simple. There is basically one static method on the `DBJCfg` class. 
```c#
// example using the config file above
 var max_block_count = DBJCfg.get<short>("max_block_count", 0  );
```
Above says: Find me the key `max_block_count`, cast its value to `short` and return it. If anything goes wrong return the last parameter given. And that is zero.

hint: Yes, in the .NET universe json files can have comments too.
