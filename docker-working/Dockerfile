FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-alpine
COPY TLC.Api/bin/Release/netcoreapp2.2/publish/ app/

ENTRYPOINT ["dotnet", "app/TLC.Api.dll"]
