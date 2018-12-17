pipeline {
    agent none
    parameters {
        string(name: 'PROJECT_KEY', defaultValue: 'SwarmApi', description: 'ProjectKey for sonarqube.')
    }
    stages {
        stage('Build') {
            agent { label 'dotnetslave' }
            environment { 
                SONAR_LOGIN = credentials('sonarqube_login') 
            }
            steps {
                withSonarQubeEnv('sonarqube') {
                    sh "dotnet sonarscanner begin /k:\"${params.PROJECT_KEY}\" /d:sonar.host.url=\"http://sonarqube:9000\" /d:sonar.login=\"${SONAR_LOGIN}\""
                    sh 'dotnet build -c Release ./SwarmAgent.sln'
                    sh "dotnet sonarscanner end /d:sonar.login=\"${SONAR_LOGIN}\""
                    sh 'dotnet test ./WebApiSpec/WebApiSpec.csproj --logger "nunit;LogFileName=WebApiSpec.xml" --results-directory ./testReports'
                    nunit testResultsPattern: 'WebApiSpec/testReports/*.xml'
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