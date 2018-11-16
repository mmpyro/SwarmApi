pipeline {
    agent none 
    stages {
        stage('Build') {
            agent { label 'dotnetslave' } 
            steps {
                sh 'dotnet build -c Release ./SwarmAgent.sln'
                sh 'dotnet test ./WebApiSpec/WebApiSpec.csproj --logger "trx;LogFileName=WebApiSpec.trx" --results-directory ./testReports'
                sh 'dotnet publish -c Release ./SwarmApi/SwarmApi.csproj -o ./out'
                sh 'cat ./testReports/*.trx'
            }
        }
    }
}