pipeline {
    agent none 
    stages {
        stage('Build') {
            agent { label 'dotnetslave' } 
            steps {
                sh 'pwd'
                sh 'ls'
                sh 'dotnet build -c Release ./SwarmAgent.sln'
                sh 'dotnet test ./WebApiSpec/WebApiSpec.csproj --logger "trx;LogFileName=WebApiSpec.trx" --results-directory ./testReports'
                sh 'dotnet publish -c Release ./SwarmApi/SwarmApi.csproj -o ./out'
                mstest testResultsFile:"**/*.trx", keepLongStdio: true
            }
        }
    }

    post {
        always {
            archiveArtifacts artifacts: 'build/out/*.*', fingerprint: true
        }
    }
}