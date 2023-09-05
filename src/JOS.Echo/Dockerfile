FROM mcr.microsoft.com/dotnet/sdk:8.0 as builder

COPY . app/
WORKDIR /app

RUN dotnet build -c Release
RUN dotnet publish --no-restore --no-build -c Release src/JOS.Echo.csproj

FROM mcr.microsoft.com/dotnet/aspnet:8.0

COPY --from=builder /app/src/JOS.Echo/bin/Release/net8.0/publish/ /app/
WORKDIR /app

USER 1000

ENV ASPNETCORE_URLS="https://+:5555"
EXPOSE 5555

ENTRYPOINT ["dotnet", "JOS.Echo.dll"]
