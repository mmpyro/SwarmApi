pipeline {
    agent none 
    stages {
        stage('Build') {
            agent { label 'dotnetslave' } 
            steps {
                echo 'Build SwarmApi'
            }
        }
    }
}