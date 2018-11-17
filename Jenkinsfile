pipeline {
    agent none
    parameters {
        string(name: 'ProjectKey', defaultValue: 'SwarmApi', description: 'ProjectKey for sonarqube.')
        string(name: 'SonarLogin', defaultValue: '035d2995442a2df8832371aa4d93cf379f87e4a6')
    }
    stages {
        stage('Build') {
            agent { label 'dotnetslave' } 
            steps {
                sh 'dotnet sonarscanner begin /k:"SwarmApi" /d:sonar.host.url="http://sonarqube:9000" /d:sonar.login="035d2995442a2df8832371aa4d93cf379f87e4a6"'
                sh 'dotnet build -c Release ./SwarmAgent.sln /d:sonar.host.url="http://sonarqube:9000" /d:sonar.login="035d2995442a2df8832371aa4d93cf379f87e4a6"'
                sh 'dotnet sonarscanner end'
                sh 'dotnet test ./WebApiSpec/WebApiSpec.csproj --logger "nunit;LogFileName=WebApiSpec.xml" --results-directory ./testReports'
                nunit testResultsPattern: 'WebApiSpec/testReports/TestResults.xml'
                sh 'dotnet publish -c Release ./SwarmApi/SwarmApi.csproj -o ./out'
            }

            post {
                always {
                    archiveArtifacts artifacts: 'SwarmApi/out/*.*', fingerprint: true
                }
            }
        }
    }
}