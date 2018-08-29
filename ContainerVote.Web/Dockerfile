FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ContainerVote.Web/ContainerVote.Web.csproj ContainerVote.Web/
RUN dotnet restore ContainerVote.Web/ContainerVote.Web.csproj
COPY . .
WORKDIR /src/ContainerVote.Web
RUN dotnet build ContainerVote.Web.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish ContainerVote.Web.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "ContainerVote.Web.dll"]
