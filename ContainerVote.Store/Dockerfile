FROM microsoft/dotnet:2.1-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ContainerVote.Store/ContainerVote.Store.csproj ContainerVote.Store/
COPY ContainerVote.Shared.Primitives/ContainerVote.Shared.Primitives.csproj ContainerVote.Shared.Primitives/
RUN dotnet restore ContainerVote.Store/ContainerVote.Store.csproj
COPY . .
WORKDIR /src/ContainerVote.Store
RUN dotnet build ContainerVote.Store.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish ContainerVote.Store.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "ContainerVote.Store.dll"]
