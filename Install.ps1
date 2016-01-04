param($installPath, $toolsPath, $package, $project)

function Get-SolutionDir {
    if($dte.Solution -and $dte.Solution.IsOpen) {
        return Split-Path $dte.Solution.Properties.Item("Path").Value
    }
    else {
        throw "Solution not avaliable"
    }
}

function Copy-Targets($project){
	$solutionDir = Get-SolutionDir
	$tasksPath = (Join-Path $solutionDir ".scm")

	if(!(Test-Path $tasksPath)){
		mkdir $tasksPath | Out-Null
	}

	Write-Host "Copying tasks from 'packages' directory to .scm folder"
	Copy-Item "$toolsPath\*.dll" $tasksPath -Force | Out-Null
	Copy-Item "$toolsPath\*.Targets" $tasksPath -Force | Out-Null

	Write-Host "Please commit the '.scm' folder."
	return "$tasksPath"
}

function Main{
	$tasksPath = Copy-Targets $project
}

Main