Param(
    [string]$Configuration="Release"
)

. .\buildutils.ps1

# Main Script entry point -----------

pushd $PSScriptRoot
$oldPath = $env:Path
$ErrorActionPreference = "Stop"
$InformationPreference = "Continue"
try
{
    $msbuild = Find-MSBuild
    if( !$msbuild )
    {
        throw "MSBuild not found"
    }

    if( !$msbuild.FoundOnPath )
    {
        $env:Path = "$env:Path;$($msbuild.BinPath)"
    }

    # setup standard MSBuild logging for this build
    $msbuildLoggerArgs = @('/clp:Verbosity=Minimal')

    if (Test-Path "C:\Program Files\AppVeyor\BuildAgent\Appveyor.MSBuildLogger.dll")
    {
        $msbuildLoggerArgs = $msbuildLoggerArgs + @("/logger:`"C:\Program Files\AppVeyor\BuildAgent\Appveyor.MSBuildLogger.dll`"")
    }

    $buildPaths = Get-BuildPaths $PSScriptRoot

    Write-Information "Build Paths:"
    Write-Information ($buildPaths | Format-Table | Out-String)

    if( Test-Path -PathType Container $buildPaths.BuildOutputPath )
    {
        Write-Information "Cleaning output folder from previous builds"
        rd -Recurse -Force -Path $buildPaths.BuildOutputPath
    }

    md BuildOutput\NuGet\ | Out-Null

    $BuildInfo = Get-BuildInformation $buildPaths
    if($env:APPVEYOR)
    {
        Update-AppVeyorBuild -Version $BuildInfo.FullBuildNumber
    }

    $packProperties = @{ version=$($BuildInfo.PackageVersion)
                         llvmversion=$($BuildInfo.LlvmVersion)
                         buildbinoutput=(normalize-path (Join-path $($buildPaths.BuildOutputPath) 'bin'))
                         configuration=$Configuration
                       }

    $msBuildProperties = @{ Configuration = $Configuration
                            FullBuildNumber = $BuildInfo.FullBuildNumber
                            PackageVersion = $BuildInfo.PackageVersion
                            FileVersionMajor = $BuildInfo.FileVersionMajor
                            FileVersionMinor = $BuildInfo.FileVersionMinor
                            FileVersionBuild = $BuildInfo.FileVersionBuild
                            FileVersionRevision = $BuildInfo.FileVersionRevision
                            FileVersion = $BuildInfo.FileVersion
                            LlvmVersion = $BuildInfo.LlvmVersion
                          }

    Write-Information "Build Parameters:"
    Write-Information ($BuildInfo | Format-Table | Out-String)

    # Need to invoke NuGet directly for restore of vcxproj as /t:Restore target doesn't support packages.config
    # and PackageReference isn't supported for native projects
    Write-Information "Restoring NuGet Packages for LibLLVM.vcxproj"
    Invoke-NuGet restore src\LibLLVM\LibLLVM.vcxproj -PackagesDirectory $buildPaths.NuGetRepositoryPath -Verbosity quiet

    Write-Information "Building LibLLVM"
    Invoke-MSBuild -Targets Build -Project src\LibLLVM\MultiPlatformBuild.vcxproj -Properties $msBuildProperties -LoggerArgs $msbuildLoggerArgs

    Write-Information "Restoring NuGet Packages for Llvm.NET"
    Invoke-MSBuild -Targets Restore -Project src\Llvm.NET.sln -Properties $msBuildProperties -LoggerArgs $msbuildLoggerArgs

    Write-Information "Building Llvm.NET"
    Invoke-MSBuild -Targets Build -Project src\Llvm.NET.sln -Properties $msBuildProperties -LoggerArgs $msbuildLoggerArgs
}
finally
{
    popd
    $env:Path = $oldPath
}
