# MSBuild SCM Tasks [![Build Status](https://travis-ci.org/giovanebribeiro/msbuildscmtasks.svg?branch=master)](https://travis-ci.org/giovanebribeiro/msbuildscmtasks)

A set of MSBuild tasks to increase productivity for Software Configuration Managers.

## List of tasks
For more details check our wiki!

| Task Name     | Description                                                                   |
|:-------------:|:------------------------------------------------------------------------------|
| [BumpVersion](https://github.com/giovanebribeiro/msbuildscmtasks/wiki/Available-Tasks:-BumpVersion)   | Bump the "AssemblyVersion" property on AssemblyInfo.cs file                   |
| [Changelog](https://github.com/giovanebribeiro/msbuildscmtasks/wiki/Available-Tasks:-Changelog)     | Updates the CHANGELOG.md file with the last commits between last tag and HEAD |
| [GitAdd](https://github.com/giovanebribeiro/msbuildscmtasks/wiki/Available-Tasks:-Git#git-task-add)        | Add files to a git repository                                                 |
| [GitAddTag](https://github.com/giovanebribeiro/msbuildscmtasks/wiki/Available-Tasks:-Git#git-task-add-tag)     | Add a tag to git repository                                                   |
| [GitCommit](https://github.com/giovanebribeiro/msbuildscmtasks/wiki/Available-Tasks:-Git#git-task-commit)     | Commit changes in git repository                                              |
| [GitStatus](https://github.com/giovanebribeiro/msbuildscmtasks/wiki/Available-Tasks:-Git#git-task-status)     | Show git repository status                                                    |

## How to install?

You must use the [NuGet Package Manager Console](http://docs.nuget.org/consume/package-manager-console)
```
> Install-Package MSBuildSCMTasks
```

## How to use?
In your build.proj file, include these line
```xml
<Import Project="$(MSBuildProjectDirectory)\packages\MSBuildSCMTasks<most.recent.release>\MSBuild.SCM.Tasks.Targets"/>
```
To allow the use of tasks in your build.

More details about each task, check the wiki (on contruction).

Please if you found any problems or questions, create an issue. Or create a Pull Request :D

## Licence

MIT
