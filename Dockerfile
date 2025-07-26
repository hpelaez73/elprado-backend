FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

COPY . .

# Limpiar caché (opcional, pero útil)
RUN dotnet nuget locals all --clear

# Restaurar dependencias
RUN dotnet restore

# Publicar
RUN dotnet publish -c Release -o /app

# Imagen final más liviana
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime
WORKDIR /app

COPY --from=build /app .

EXPOSE 80

ENTRYPOINT ["dotnet", "ElPrado.WebApi.dll", "--environment=Staging"]
