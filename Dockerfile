FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

EXPOSE 80

#copiar archivos del proyecto
COPY . .
RUN dotnet restore

RUN dotnet publish -c Release -o /app
    
#Build image
FROM mcr.microsoft.com/dotnet/sdk:7.0
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "ElPrado.WebApi.dll", "--environment=Staging"]
