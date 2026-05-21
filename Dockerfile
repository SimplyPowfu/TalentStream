FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

COPY *.sln ./
COPY src/TalentStream.Core/*.csproj ./src/TalentStream.Core/
COPY src/TalentStream.Infrastructure/*.csproj ./src/TalentStream.Infrastructure/
COPY src/TalentStream.WebApi/*.csproj ./src/TalentStream.WebApi/
RUN dotnet restore

COPY . ./
RUN dotnet publish src/TalentStream.WebApi/TalentStream.WebApi.csproj -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build-env /app/out .

EXPOSE 80
ENTRYPOINT ["dotnet", "TalentStream.WebApi.dll"]