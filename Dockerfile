FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY ./AccountService.Server/*.csproj ./AccountService.Server/
COPY ./accountservice.client/ ./accountservice.client/

COPY NuGet.Config ./

RUN dotnet restore "./AccountService.Server/AccountService.Server.csproj" --configfile "./NuGet.Config"

COPY ./AccountService.Server/. ./AccountService.Server/
COPY ./accountservice.client/. ./accountservice.client/

ARG BUILD_ENV

RUN apt-get update -yq && \
    apt-get install -yq curl gnupg ca-certificates

# Add NodeSource GPG key
RUN curl -fsSL https://deb.nodesource.com/gpgkey/nodesource-repo.gpg.key | gpg --dearmor -o /etc/apt/keyrings/nodesource.gpg
# Add Node.js 20.x repository
RUN echo "deb [signed-by=/etc/apt/keyrings/nodesource.gpg] https://deb.nodesource.com/node_20.x nodistro main" | tee /etc/apt/sources.list.d/nodesource.list

# Update package lists and install Node.js
RUN apt-get update -yq && \
    apt-get install -yq nodejs

RUN dotnet publish "./AccountService.Server/AccountService.Server.csproj" --configfile "./NuGet.Config" -c "${BUILD_ENV}" -o /out



FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app

COPY --from=build /out .

ENV ASPNETCORE_URLS=http://+:80

EXPOSE 8000

CMD [ "dotnet", "AccountService.Server.dll" ]
