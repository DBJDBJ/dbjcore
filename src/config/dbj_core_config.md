<h1>DBJ CORE CONFIG</h1>

# Dbj Core Configuration Service

Contrary to popular belief (prevailing among the .NET Lemmings population) .NET core json configuration does not require any specially named json config files. Any name will do, as long as you make and use your own configuration routines. We just happen to call ours: `appsettings.json`. You can call yours `iliketobedifferent.js`.

If you want your own name dive into the code please and change accordingly.

## dbjcore config requirements

If you are sane and you are using Visual Studio please just observe this section. You would understand why.

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

```cmd
dotnet add package Microsoft.Extensions.Configuration.Binder
dotnet add package Microsoft.Extensions.Configuration.Json
dotnet add package Microsoft.Extensions.Configuration.EnvironmentVariables
```

Let us repeat: do that in the folder where the csproj of the host project is. Not the sln but the csproj file.

## Usage

`DBJCfg.FileName` is the name of the json cfg file (hint: `appsettings.json` mentioned above). In case you have a itch to change it, do change it here. 

config specimen used bellow:
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
