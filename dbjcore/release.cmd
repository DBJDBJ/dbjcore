@REM https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-publish
@REM CAUTION: on every runt this creates a new folder ./pub
@REM thus the prevuious ./pub is deleted
@cls
dotnet publish -c Release -o ./pub

@REM To use your published assembly (dbjcore) from other projects on the same machine, you have a few options:
@REM 1. Add it as a local NuGet package:
@REM    ```bash
@REM    dotnet pack -c Release
@REM    ```
@REM    This will create a .nupkg file. Then add a local NuGet source in your consuming project:
@REM    ```xml
@REM    <PropertyGroup>
@REM      <RestoreSources>$(RestoreSources);/path/to/your/package/directory;https://api.nuget.org/v3/index.json</RestoreSources>
@REM    </PropertyGroup>
@REM    ```

@REM 2. Add a direct project reference using a relative or absolute path:
@REM    ```xml
@REM    <ItemGroup>
@REM      <Reference Include="dbjcore">
@REM        <HintPath>C:\path\to\your\pub\dbjcore.dll</HintPath>
@REM      </Reference>
@REM    </ItemGroup>
@REM    ```
@REM The NuGet approach is generally cleaner for longer-term maintenance, while the direct reference is simpler for quick development.

