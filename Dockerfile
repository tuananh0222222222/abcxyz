


# Sử dụng hình ảnh chính thức của .NET Core SDK để xây dựng
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app
EXPOSE 80
EXPOSE 443

WORKDIR /app
COPY *.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o out

# Sử dụng hình ảnh chính thức của ASP.NET Core Runtime để chạy ứng dụng
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS build-final
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "demodockerv2.webapi.dll"]
