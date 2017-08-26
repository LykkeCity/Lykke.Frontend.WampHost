FROM microsoft/aspnetcore:2.0
WORKDIR /app
COPY . .
RUN find / -name "libuv.so"
COPY /runtimes/debian-x64/native/libuv.so /app
ENTRYPOINT ["dotnet", "Lykke.Frontend.WampHost.dll"]
