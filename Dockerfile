FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["TournamentBracket.Api/TournamentBracket.Api.csproj", "TournamentBracket.Api/"]
COPY ["TournamentBracket.Application/TournamentBracket.Application.csproj", "TournamentBracket.Application/"]
COPY ["TournamentBracket.Domain/TournamentBracket.Domain.csproj", "TournamentBracket.Domain/"]
COPY ["TournamentBracket.Infrastructure/TournamentBracket.Infrastructure.csproj", "TournamentBracket.Infrastructure/"]

RUN dotnet restore "TournamentBracket.Api/TournamentBracket.Api.csproj"

COPY . .
WORKDIR "/src/TournamentBracket.Api"
RUN dotnet build -c $BUILD_CONFIGURATION -o /app/build --no-restore

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false --no-restore


FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS final
USER app
WORKDIR /app
COPY --from=publish /app/publish .

ARG DB_HOST
ARG TOURNAMENT_DB_USER
ARG TOURNAMENT_DB_PASSWORD
ENV DB_HOST=$DB_HOST \
    TOURNAMENT_DB_USER=$TOURNAMENT_DB_USER \
    TOURNAMENT_DB_PASSWORD=$TOURNAMENT_DB_PASSWORD

EXPOSE 8080 8081 5141

ENTRYPOINT ["dotnet", "TournamentBracket.Api.dll"]
CMD ["--migrate"]