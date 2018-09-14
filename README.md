# Just.Anarchy
Asp.net core chaos monkey library

Usage:
Add a new nuget reference in csproj file:
```cs
<PackageReference Include="Just.Anarchy" Version="x.x.x" />
```

```cs
mvc.AddAnarchy()
.....
app.UseAnarchy()
```
Default chaos is off :)

Api usage:
```cs/api/anarchy/enable
/api/anarchy/disable
/api/anarchy/state
/api/anarchy/enable/CpuAnarchy
```
By Luis Roberto Teixeira Pereira & Matt Jenner 
