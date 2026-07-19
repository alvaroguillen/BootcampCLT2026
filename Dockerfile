FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copiamos solo el .csproj primero para cachear el restore
COPY *.csproj ./
RUN dotnet restore BootcampCLT2026.csproj

# Copiamos el resto del código y publicamos
COPY . .
RUN dotnet publish BootcampCLT2026.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "BootcampCLT2026.dll"]