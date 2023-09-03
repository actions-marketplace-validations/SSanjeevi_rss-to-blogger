# Build SDK image
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /BlogRssFeed
 
# Copy everything from the project folder
# to the /app working directory inside this image
COPY . ./
# Build and publish a release into out subfolder
RUN dotnet publish -c Release -o out
 
# Build runtime image
FROM mcr.microsoft.com/dotnet/runtime:6.0
WORKDIR /BlogRssFeed
# Copy everything from /app/out folder in compiler image
# to the /app working directory inside this image
COPY --from=build-env /BlogRssFeed/out .
# Entrypoint **must** specify absolute path to the executable
# because GitHub actions will pass in custom working directory
ENTRYPOINT ["dotnet", "/BlogRssFeed/BlogRssFeed.exe"]
