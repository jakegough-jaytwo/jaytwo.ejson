library 'JenkinsBuilderLibrary'

helper.gitHubUsername = 'jakegough'
helper.gitHubRepository = 'jaytwo.ejson'
helper.gitHubTokenCredentialsId = 'github-personal-access-token-jakegough'
helper.nuGetCredentialsId = 'nuget-org-jaytwo'
helper.xunitTestResultsPattern = 'out/testResults/**/*.trx'

helper.run('linux && make && docker', {
    def timestamp = helper.getTimestamp()
    
    withEnv(["TIMESTAMP=${timestamp}"]) {
        stage ('Build') {
            sh "make docker-build"
        }
        stage ('Unit Test') {
            sh "make docker-unit-test"
        }
        stage ('Pack') {
            if(env.BRANCH_NAME == 'master'){
                sh "make docker-pack"
            } else {
                sh "make docker-pack-beta"
            }
        }
        if(env.BRANCH_NAME == 'master' || env.BRANCH_NAME == 'develop'){
            stage ('Publish NuGet') {
                helper.pushNugetPackage('out/packed')
            }
        }
    }
})
