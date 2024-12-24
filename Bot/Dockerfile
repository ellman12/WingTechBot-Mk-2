FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app
COPY . . 

# Debug
# RUN dotnet publish -c Debug -o out

# Release
RUN dotnet publish -c Release -o out

COPY config.json /app/

FROM mcr.microsoft.com/dotnet/runtime:9.0
WORKDIR /app
COPY --from=build /app/out . 
CMD ["dotnet", "WingTechBot.dll"]

