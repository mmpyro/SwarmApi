pipeline {
    agent none
    stages {
        stage('Build') {
            agent { label 'dotnetslave' } 
            parameters {
                string(name: 'ProjectKey', defaultValue: 'SwarmApi', description: 'ProjectKey for sonarqube.')
                string(name: 'SonarLogin', defaultValue: '035d2995442a2df8832371aa4d93cf379f87e4a6')
            }
            steps {
                sh 'dotnet sonarscanner begin /k:"${param.ProjectKey}" /d:sonar.login="${param.SonarLogin}" /d:sonar.host.url="http://sonarqube:9000"'
                sh 'dotnet build -c Release ./SwarmAgent.sln'
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