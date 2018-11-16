pipeline {
    agent none
    stages {
        stage('Build') {
            agent { label 'dotnetslave' } 

            steps {
                sh 'dotnet build -c Release ./SwarmAgent.sln'
                sh 'dotnet test ./WebApiSpec/WebApiSpec.csproj --logger "nunit;LogFileName=WebApiSpec.xml" --results-directory ./testReports'
                sh 'dotnet publish -c Release ./SwarmApi/SwarmApi.csproj -o ./out'
            }

            post {
                always {
                    archiveArtifacts artifacts: 'SwarmApi/out/*.*', fingerprint: true
                    nunit testResultsPattern: 'WebApiSpec.xml'
                }
            }
        }
    }
}