FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY . .

RUN dotnet restore
RUN dotnet build

FROM build AS test
RUN dotnet test --logger:trx

