Whatever moved into here will not be visible in Visual Studio solution.
That is because of this entry in the csproj

```xml
  <ItemGroup>
    <Compile Remove="deprecated\**" />
    <EmbeddedResource Remove="deprecated\**" />
    <None Remove="deprecated\**" />
  </ItemGroup>
```