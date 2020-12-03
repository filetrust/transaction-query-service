#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine AS base
WORKDIR /app
EXPOSE 443
EXPOSE 80

RUN apk update && \
apk add --no-cache openssl && \
openssl req -x509 -nodes -days 365 -subj "/C=GB/ST=England/O=Glasswall Solutions Ltd/CN=transaction-query-service.com" -newkey rsa:2048 -keyout /etc/ssl/private/server.key -out /etc/ssl/certs/server.crt;

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine AS build
WORKDIR /src
COPY ["TransactionQueryService/TransactionQueryService.csproj", "TransactionQueryService/"]
COPY ["TransactionQueryService.Business/TransactionQueryService.Business.csproj", "TransactionQueryService.Business/"]
COPY ["TransactionQueryService.Common/TransactionQueryService.Common.csproj", "TransactionQueryService.Common/"]
RUN dotnet restore "TransactionQueryService/TransactionQueryService.csproj"
COPY . .
WORKDIR "/src/TransactionQueryService"
RUN dotnet build "TransactionQueryService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TransactionQueryService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=base /etc/ssl/private/server.key /etc/ssl/private/
COPY --from=base /etc/ssl/certs/server.crt /etc/ssl/certs/
ENTRYPOINT ["dotnet", "TransactionQueryService.dll"]
