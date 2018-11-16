pipeline {
    agent { label 'dotnetslave' } 
    stages {
        stage('Build') {
            steps {
                sh 'dotnet build -c Release ./SwarmAgent.sln'
                sh 'dotnet test ./WebApiSpec/WebApiSpec.csproj --logger "trx;LogFileName=WebApiSpec.trx" --results-directory ./testReports'
                sh 'dotnet publish -c Release ./SwarmApi/SwarmApi.csproj -o ./out'
            }
        }
    }

    post {
        always {
            archiveArtifacts artifacts: 'SwarmApi/out/*.*', fingerprint: true
        }
    }
}