#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["WebUIApp/WebUIApp.csproj", "WebUIApp/"]
RUN dotnet restore "WebUIApp/WebUIApp.csproj"
COPY . .
WORKDIR "/src/WebUIApp"
RUN dotnet build "WebUIApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WebUIApp.csproj" -c Release -o /app/publish

FROM base AS final
ENV Settings__ServiceBaseUrl="http://color/"
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebUIApp.dll"]