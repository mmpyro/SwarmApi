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
                    sh 'dotnet test ./WebApiSpec/WebApiSpec.csproj /p:CollectCoverage=true /p:CoverletOutputFormat=opencover --logger "nunit;LogFileName=WebApiSpec.xml" --results-directory ./testReports'
                    sh "dotnet sonarscanner begin /k:\"${params.PROJECT_KEY}\" /d:sonar.host.url=\"http://sonarqube:9000\" /d:sonar.login=\"${SONAR_LOGIN}\" /d:sonar.cs.opencover.reportsPaths=\"WebApiSpec/coverage.opencover.xml\" /d:sonar.coverage.exclusions=\"**Spec*.cs\""
                    sh 'dotnet build -c Release ./SwarmAgent.sln'
                    sh "dotnet sonarscanner end /d:sonar.login=\"${SONAR_LOGIN}\""
                    nunit testResultsPattern: 'WebApiSpec/testReports/*.xml'
                    sh 'dotnet publish -c Release ./SwarmApi/SwarmApi.csproj -o ./out'
                }
            }
        }
        stage("Quality Gate") {
            steps {
              timeout(time: 1, unit: 'HOURS') {
                waitForQualityGate abortPipeline: true
              }
            }
        }
    }

    post {
        always {
            archiveArtifacts artifacts: 'SwarmApi/out/*.*', fingerprint: true
        }
    }
}