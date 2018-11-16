pipeline {
    agent none 
    stages {
        stage('Build') {
            agent { docker 'dotnetslave' } 
            steps {
                echo 'Build SwarmApi'
            }
        }
    }
}