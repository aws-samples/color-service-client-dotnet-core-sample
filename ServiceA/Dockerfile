#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["ServiceA/ColorService.csproj", "ColorService/"]
RUN dotnet restore "ColorService/ColorService.csproj"
COPY . .
WORKDIR "/src/ServiceA"
RUN dotnet build "ColorService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ColorService.csproj" -c Release -o /app/publish

FROM base AS final
ENV Settings__Color="Green"
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ColorService.dll"]