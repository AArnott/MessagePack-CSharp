# Updating Generated.cs

Generated.cs is a generated file and should not be edited manually.
To update Generated.cs, follow these steps:

1. Set the `RunningMPC` property in SharedData.csproj to `true`.
1. Execute these commands:
   ```ps1
   dotnet build src\MessagePack -f netstandard1.6
   dotnet run -p src\MessagePack.UniversalCodeGenerator -- -i sandbox\SharedData\SharedData.csproj -o sandbox\sandbox\Generated.cs
   ```

1. Revert the change to `RunningMPC` in SharedData.csproj.
