FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /App

# Copy everything
COPY . ./
# Restore as distinct layers
RUN dotnet restore
RUN dotnet test
# Build and publish a release
RUN dotnet publish -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 as runtime
ENV ASPNETCORE_ENVIRIONMENT=Production
ENV ASPNETCORE_URLS=http://0.0.0.0:8080
WORKDIR /App
EXPOSE 5000
COPY --from=build /App/out .
ENTRYPOINT ["dotnet", "api-mandado.dll"]
