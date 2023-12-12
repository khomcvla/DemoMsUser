FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

COPY ../DemoMsUser.csproj ./
RUN dotnet restore "DemoMsUser.csproj"

COPY ["../", "./"]
RUN dotnet build "DemoMsUser.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DemoMsUser.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DemoMsUser.dll"]
