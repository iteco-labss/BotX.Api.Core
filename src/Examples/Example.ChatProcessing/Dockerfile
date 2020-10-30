FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Examples/Example.ChatProcessing/Example.ChatProcessing.csproj", "Examples/Example.ChatProcessing/"]
COPY ["BotX.Api.Core/BotX.Api.csproj", "BotX.Api.Core/"]
RUN dotnet restore "Examples/Example.ChatProcessing/Example.ChatProcessing.csproj"
COPY . .
WORKDIR "/src/Examples/Example.ChatProcessing"
RUN dotnet build "Example.ChatProcessing.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Example.ChatProcessing.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Example.ChatProcessing.dll"]