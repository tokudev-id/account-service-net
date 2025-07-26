FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY ./AccountService.Server/*.csproj ./AccountService.Server/
COPY ./accountservice.client/ ./accountservice.client/

COPY NuGet.Config ./

RUN dotnet restore "./AccountService.Server/AccountService.Server.csproj" --configfile "./NuGet.Config"

COPY ./AccountService.Server/. ./AccountService.Server/
COPY ./accountservice.client/. ./accountservice.client/

ARG DB_USERNAME
ARG DB_PASSWORD
ARG REDIS_PASSWORD
ARG BUILD_ENV

# Set environment variables for the .NET application at build time
# These will be available to the application when it runs
ENV ConnectionStrings__ApplicationDBConnectionString="Server=mysql-server;Port=3306;Database=account-service-$BUILD_ENV;Uid=$DB_USERNAME;pwd=$DB_PASSWORD;Convert Zero Datetime=True;SslMode=none;AllowPublicKeyRetrieval=True;Charset=utf8mb4;"
ENV ConnectionStrings__ISGrantDBConnectionString="Server=mysql-server;Port=3306;Database=account-service-$BUILD_ENV-grant-v6;Uid=$DB_USERNAME;pwd=$DB_PASSWORD;Convert Zero Datetime=True;SslMode=none;AllowPublicKeyRetrieval=True;Charset=utf8mb4;"
ENV ConnectionStrings__ISConfigDBConnectionString="Server=mysql-server;Port=3306;Database=account-service-$BUILD_ENV-config-v6;Uid=$DB_USERNAME;pwd=$DB_PASSWORD;Convert Zero Datetime=True;SslMode=none;AllowPublicKeyRetrieval=True;Charset=utf8mb4;"
ENV ConnectionStrings__Redis="redis-internal"
ENV RedisServer__RedisInstanceName="account:$BUILD_ENV:"
ENV RedisServer__RedisCacheConfiguration="redis-internal,password=$REDIS_PASSWORD"
ENV RedisServer__RedisInstanceName="redis-internal,allowAdmin=true,password=$REDIS_PASSWORD"

RUN dotnet publish "./AccountService.Server/AccountService.Server.csproj" --configfile "./NuGet.Config" -c "${BUILD_ENV}" -o /out



FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app

COPY --from=build /out .

ENV ASPNETCORE_URLS=http://+:80

EXPOSE 8000

CMD [ "dotnet", "AccountService.Server.dll" ]
