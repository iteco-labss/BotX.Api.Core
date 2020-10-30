FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Examples/Example.StateMachine/Example.StateMachine.csproj", "Examples/Example.StateMachine/"]
COPY ["BotX.Api.Core/BotX.Api.csproj", "BotX.Api.Core/"]
RUN dotnet restore "Examples/Example.StateMachine/Example.StateMachine.csproj"
COPY . .
WORKDIR "/src/Examples/Example.StateMachine"
RUN dotnet build "Example.StateMachine.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Example.StateMachine.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Example.StateMachine.dll"]