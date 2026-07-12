FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY src/FCG.PaymentsAPI.Domain/FCG.PaymentsAPI.Domain.csproj src/FCG.PaymentsAPI.Domain/
COPY src/FCG.PaymentsAPI.Application/FCG.PaymentsAPI.Application.csproj src/FCG.PaymentsAPI.Application/
COPY src/FCG.PaymentsAPI.Infrastructure/FCG.PaymentsAPI.Infrastructure.csproj src/FCG.PaymentsAPI.Infrastructure/
COPY src/FCG.PaymentsAPI.API/FCG.PaymentsAPI.API.csproj src/FCG.PaymentsAPI.API/
RUN dotnet restore src/FCG.PaymentsAPI.API/FCG.PaymentsAPI.API.csproj

COPY src/ src/
RUN dotnet publish src/FCG.PaymentsAPI.API/FCG.PaymentsAPI.API.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "FCG.PaymentsAPI.API.dll"]
