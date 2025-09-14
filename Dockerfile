FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY USR/UserService/UserService.csproj UserService/
RUN dotnet restore UserService/UserService.csproj

COPY USR/UserService/ UserService/
WORKDIR /src/UserService
RUN dotnet build UserService.csproj -c Release -o /app/build

FROM build AS publish
WORKDIR /src/UserService
RUN dotnet publish UserService.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

RUN groupadd -r appuser && useradd -r -g appuser appuser

COPY --from=publish /app/publish .

RUN chown -R appuser:appuser /app && \
    chmod -R 755 /app

USER appuser

EXPOSE 3000
ENV ASPNETCORE_URLS=http://+:3000
ENTRYPOINT ["dotnet", "UserService.dll"]
