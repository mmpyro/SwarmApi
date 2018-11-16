FROM microsoft/dotnet:2.1-sdk as build
WORKDIR /src
COPY . .
RUN dotnet build -c Release ./SwarmAgent.sln
RUN dotnet test ./WebApiSpec/WebApiSpec.csproj --logger "trx;LogFileName=WebApiSpec.trx" --results-directory /src/testReports
RUN dotnet publish -c Release ./SwarmApi/SwarmApi.csproj -o /out


FROM microsoft/dotnet:2.1-aspnetcore-runtime
WORKDIR /home/app
COPY --from=build ./src/testReports/ ./testReports
COPY --from=build ./out/ ./
ENTRYPOINT dotnet SwarmApi.dll