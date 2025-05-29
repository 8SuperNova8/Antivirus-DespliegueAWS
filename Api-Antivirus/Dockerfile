# Etapa de construcción
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copiar archivos del proyecto y restaurar dependencias
COPY . .

# Restaura dependencias
RUN dotnet restore "Api-Antivirus.csproj"

# Compilar y publicar la aplicación
RUN dotnet publish -c Release -o /app/publish

# Etapa de ejecución en una imagen optimizada
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Copiar la aplicación publicada
COPY --from=build /app/publish .

# Exponer el puerto para la aplicación
# Puerto 80 (requerido por AWS Elastic Beanstalk/ECS).
EXPOSE 80
# Comando para ejecutar la aplicación
ENTRYPOINT ["dotnet", "Api-Antivirus.dll"]