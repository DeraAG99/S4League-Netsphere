FROM mcr.microsoft.com/dotnet/runtime:2.1
WORKDIR /app/data

COPY . .
# RUN chmod +x /app/data/start_auth.sh

ENTRYPOINT ["dotnet", "exec", "GameServer.dll"]