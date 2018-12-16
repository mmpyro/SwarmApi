pipeline {
    agent none
    parameters {
        string(name: 'PROJECT_KEY', defaultValue: 'SwarmApi', description: 'ProjectKey for sonarqube.')
        string(name: 'SONAR_LOGIN', defaultValue: '880a763e0ed4aece2048d1b65e89bbd26ec0c9f8')
    }
    stages {
        stage('Build') {
            agent { label 'dotnetslave' } 
            steps {
                withSonarQubeEnv('sonarqube') {
                    sh "dotnet sonarscanner begin /k:\"${params.PROJECT_KEY}\" /d:sonar.host.url=\"http://sonarqube:9000\" /d:sonar.login=\"${params.SONAR_LOGIN}\""
                    sh 'dotnet build -c Release ./SwarmAgent.sln'
                    sh "dotnet sonarscanner end /d:sonar.login=\"${params.SONAR_LOGIN}\""
                    sh 'dotnet test ./WebApiSpec/WebApiSpec.csproj --logger "nunit;LogFileName=WebApiSpec.xml" --results-directory ./testReports'
                    nunit testResultsPattern: 'WebApiSpec/testReports/TestResults.xml'
                    sh 'dotnet publish -c Release ./SwarmApi/SwarmApi.csproj -o ./out'
                }
            }

            post {
                always {
                    archiveArtifacts artifacts: 'SwarmApi/out/*.*', fingerprint: true
                }
            }
        }
    }
}