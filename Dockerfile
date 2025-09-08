FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY UserService/UserService.csproj UserService/
RUN dotnet restore UserService/UserService.csproj
COPY . .
WORKDIR /src/UserService
RUN dotnet build UserService.csproj -c Release -o /app/build

FROM build AS publish
WORKDIR /src/UserService
RUN dotnet publish UserService.csproj -c Release -o /app/publish /p:UseAppHost=false

RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"
RUN dotnet ef database update --no-build --project UserService.csproj

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 3000
ENV ASPNETCORE_URLS=http://+:3000
ENTRYPOINT ["dotnet", "UserService.dll"]
