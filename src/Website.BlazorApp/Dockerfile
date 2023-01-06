FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["./Website.BlazorApp.csproj", "WebsiteAdmin/Website.BlazorApp/"]
RUN dotnet restore "WebsiteAdmin/Website.BlazorApp/Website.BlazorApp.csproj"
WORKDIR "/src/WebsiteAdmin/Website.BlazorApp"
COPY ["server-cert.crt", "/usr/local/share/ca-certificates/"]
COPY . .
RUN chmod 644 /usr/local/share/ca-certificates/server-cert.crt && update-ca-certificates
RUN dotnet build "Website.BlazorApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Website.BlazorApp.csproj" -c Release -o /app/publish

FROM base AS final
RUN apt-get update && apt-get install -y iputils-ping && apt-get install -y curl
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Website.BlazorApp.dll"]
