FROM mcr.microsoft.com/dotnet/sdk:8.0-preview as builder
# https://learn.microsoft.com/en-us/nuget/reference/cli-reference/cli-ref-environment-variables
ENV NUGET_XMLDOC_MODE=none
COPY . app/
WORKDIR /app

RUN dotnet publish src/JOS.Echo -c Release -o published --no-build

FROM mcr.microsoft.com/dotnet/aspnet:8.0-preview

COPY --from=builder /app/published/ /app/
WORKDIR /app

USER 1000

ENV ASPNETCORE_URLS https://+:443
EXPOSE 443

ENTRYPOINT ["dotnet", "JOS.Echo.dll"]
