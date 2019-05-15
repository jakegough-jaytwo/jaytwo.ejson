library 'JenkinsBuilderLibrary'

helper.gitHubUsername = 'jakegough'
helper.gitHubRepository = 'jaytwo.ejson'
helper.gitHubTokenCredentialsId = 'github-personal-access-token-jakegough'
helper.nuGetCredentialsId = 'nuget-org-jaytwo'
helper.xunitTestResultsPattern = 'out/testResults/**/*.trx'

helper.run('linux && make && docker', {
    def timestamp = helper.getTimestamp()
    def safeJobName = env.JOB_NAME.replaceAll('[^A-Za-z0-9]', '_').toLowerCase()
    def dockerLocalTag = "jenkins__${safeJobName}__${timestamp}"
    
    withEnv(["DOCKER_TAG=${dockerLocalTag}", "TIMESTAMP=${timestamp}"]) {
        try {
            stage ('Build') {
                sh "make docker-build"
            }
            stage ('Unit Test') {
                sh "make docker-unit-test-only"
                cobertura coberturaReportFile: 'out/coverage/Cobertura.xml'
                publishHTML target: [
                  allowMissing: false,
                  alwaysLinkToLastBuild: false,
                  keepAll: true,
                  reportDir: 'out/coverage/html',
                  reportFiles: 'index.htm',
                  reportName: "Test Coverage"
                ]
            }
            stage ('Pack') {
                if(env.BRANCH_NAME == 'master'){
                    sh "make docker-pack-only"
                } else {
                    sh "make docker-pack-beta-only"
                }
            }
            if(env.BRANCH_NAME == 'master' || env.BRANCH_NAME == 'develop'){
                stage ('Publish NuGet') {
                    helper.pushNugetPackage('out/packed')
                }
            }
        }
        finally {
            // inside the withEnv()
            sh "make docker-clean"
        }
    }
})
