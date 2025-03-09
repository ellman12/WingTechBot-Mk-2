FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app
COPY . .

RUN dotnet publish -c Release -o out


FROM mcr.microsoft.com/dotnet/runtime:9.0
WORKDIR /app
RUN apt update
RUN apt install -y libopus0 libsodium23 ffmpeg libopus-dev opus-tools
COPY --from=build /app/out .
CMD ["dotnet", "WingTechBot.dll"]

