FROM mcr.microsoft.com/dotnet/sdk:8.0 as builder
# https://learn.microsoft.com/en-us/nuget/reference/cli-reference/cli-ref-environment-variables
ARG informationalVersion
ARG version
ENV NUGET_XMLDOC_MODE=none
COPY . app/
WORKDIR /app

RUN dotnet build -c Release /p:AssemblyVersion=$version /p:InformationalVersion=$informationalVersion
RUN dotnet test -c Release --no-build
RUN dotnet publish src/JOS.Echo -c Release -o published --no-build

FROM mcr.microsoft.com/dotnet/aspnet:8.0-jammy

COPY --from=builder /app/published/ /app/
WORKDIR /app

RUN curl -sSL -O https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb
RUN dpkg -i packages-microsoft-prod.deb
RUN rm packages-microsoft-prod.deb
RUN apt update \
    && apt install -y --no-install-recommends \
        curl \
        wget \
        gnupg2 \
        libmsquic \
        software-properties-common

RUN apt-get -y install libcap2-bin
RUN setcap 'cap_net_bind_service=+ep' /usr/share/dotnet/dotnet

USER 1000

ENV ASPNETCORE_URLS https://+:443
EXPOSE 443/tcp
EXPOSE 443/udp

ENTRYPOINT ["dotnet", "JOS.Echo.dll"]
