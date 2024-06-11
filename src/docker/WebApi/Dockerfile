FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /App
EXPOSE 8080

RUN apt-get update \
    && apt-get install -y --no-install-recommends \
    clang zlib1g-dev

COPY . .
COPY ./WebApi/*.csproj ./WebApi
COPY ./Infrastructure/*.csproj ./Infrastructure

RUN dotnet restore RabbitMQ-and-MassTransit.sln
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /App
COPY --from=build-env /App/out .
EXPOSE 8080
ENTRYPOINT ["dotnet", "WebApi.dll"]