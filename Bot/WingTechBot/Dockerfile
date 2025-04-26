FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app
COPY . .

RUN dotnet publish -c Release -o out


FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
RUN apt update
RUN apt install -y ffmpeg libopus0 libsodium23 libopus-dev libsodium-dev opus-tools
COPY --from=build /app/out .
EXPOSE 5000
# CMD ["dotnet", "WingTechBot.dll"]
CMD ["dotnet", "WingTechBot.dll", "--no-recreate-commands"]

