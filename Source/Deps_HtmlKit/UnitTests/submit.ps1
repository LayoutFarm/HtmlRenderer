$revision = git rev-parse HEAD
$branch = git rev-parse --abbrev-ref HEAD
$commitAuthor = git show --quiet --format="%aN" $revision
$commitEmail = git show --quiet --format="%aE" $revision
$commitMessage = git show --quiet --format="%s" $revision

#nuget install coveralls.net

Set-PSDebug -Trace 2
..\packages\coveralls.net.0.5.0\csmacnz.Coveralls.exe `
--opencover -i opencover.xml `
--repoToken $Env:COVERALLS_REPO_TOKEN `
--commitId $revision `
--commitBranch $branch `
--commitAuthor $commitAuthor `
--commitEmail $commitEmail `
--commitMessage $commitMessage `
--useRelativePaths `
--basePath .\bin\Debug
