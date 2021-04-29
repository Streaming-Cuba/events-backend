FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /app

ENV DOTNET_CLI_TELEMETRY_OPTOUT 1

# copy csproj and restore as distinct layers
COPY ./Events.API/*.csproj ./
RUN dotnet restore

# copy everything else and build
COPY ./Events.API/ ./
RUN dotnet publish -c Release -o out

# build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY --from=build-env /app/out .

# ENV ASPNETCORE_URLS http://*:$PORT
# ENTRYPOINT ["dotnet", "Events.API.dll"]
CMD ASPNETCORE_URLS=http://*:$PORT dotnet Events.API.dll
