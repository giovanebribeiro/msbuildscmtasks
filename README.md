# MSBuild SCM Tasks

A set of MSBuild tasks to increase productivity for Software Configuration Managers.

## List of tasks
For more details check our wiki!

| Task Name     | Description                                                                   |
|:-------------:|:------------------------------------------------------------------------------|
| [BumpVersion](https://github.com/giovanebribeiro/msbuildscmtasks/wiki/Available-Tasks:-BumpVersion)   | Bump the "AssemblyVersion" property on AssemblyInfo.cs file                   |
| Changelog     | Updates the CHANGELOG.md file with the last commits between last tag and HEAD |
| GitAdd        | Add files to a git repository                                                 |
| GitAddTag     | Add a tag to git repository                                                   |
| GitCommit     | Commit changes in git repository                                              |
| GitStatus     | Show git repository status                                                    |

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
