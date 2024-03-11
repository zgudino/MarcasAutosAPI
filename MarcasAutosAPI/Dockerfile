FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 7005
EXPOSE 5234

ENV ASPNETCORE_URLS=http://+:5234

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["MarcasAutosAPI/MarcasAutosAPI.csproj", "MarcasAutosAPI/"]
RUN dotnet restore "MarcasAutosAPI/MarcasAutosAPI.csproj"
COPY . .
WORKDIR "/src/MarcasAutosAPI"
RUN dotnet build "MarcasAutosAPI.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "MarcasAutosAPI.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MarcasAutosAPI.dll"]
