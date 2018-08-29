FROM microsoft/dotnet:2.1-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ContainerVote.Calculate/ContainerVote.Calculate.csproj ContainerVote.Calculate/
COPY ContainerVote.Shared.Primitives/ContainerVote.Shared.Primitives.csproj ContainerVote.Shared.Primitives/
RUN dotnet restore ContainerVote.Calculate/ContainerVote.Calculate.csproj
COPY . .
WORKDIR /src/ContainerVote.Calculate
RUN dotnet build ContainerVote.Calculate.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish ContainerVote.Calculate.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "ContainerVote.Calculate.dll"]
