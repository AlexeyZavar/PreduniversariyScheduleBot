FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
RUN apt-get update && apt-get install -y libgdiplus

WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["MarkBot/MarkBot.csproj", "MarkBot/"]
COPY ["MarkBot.MosRu/MarkBot.MosRu.csproj", "MarkBot.MosRu/"]
COPY ["MarkBot.Parsers/MarkBot.Parsers.csproj", "MarkBot.Parsers/"]
COPY ["MarkBot.Schedule/MarkBot.Schedule.csproj", "MarkBot.Schedule/"]
RUN dotnet restore "MarkBot/MarkBot.csproj"
COPY . .
WORKDIR "/src/MarkBot"
RUN dotnet build "MarkBot.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MarkBot.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MarkBot.dll"]
