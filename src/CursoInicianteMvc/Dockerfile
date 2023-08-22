FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/CursoInicianteMvc/CursoInicianteMvc.csproj", "src/CursoInicianteMvc/"]
RUN dotnet restore "src/CursoInicianteMvc/CursoInicianteMvc.csproj"
COPY . .
WORKDIR "/src/src/CursoInicianteMvc"
RUN dotnet build "CursoInicianteMvc.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CursoInicianteMvc.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CursoInicianteMvc.dll"]
