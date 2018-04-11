pipeline {
  agent any
  stages {
    stage('abc') {
      parallel {
        stage('abc') {
          steps {
            sh 'ls -l'
          }
        }
        stage('def') {
          steps {
            sh 'ls -al'
          }
        }
      }
    }
    stage('fgh') {
      steps {
        sh 'ls -al'
      }
    }
  }
}