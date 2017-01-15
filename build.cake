// This script supports project-local 'TestProjectFolder/BuildFolder/' folders for test projects or
// global '.SolutionBuildFolder/Tests/' folder.
//
// Place your .nuspecs in their corresponding project source folders
// (AssemblyInfo.cs must be reachable from .nuspec location).
//
// For NuGet pushing you should set environmental variable NuGetApiKey.
//
using System.Linq;
using System.Text.RegularExpressions;

// Add-ins
//#tool "nuget:?package=GitReleaseNotes"
#tool "nuget:?package=GitVersion.CommandLine"
#tool "nuget:?package=gitlink"
// Waiting for PR: https://github.com/Redth/Cake.ExtendedNuGet/issues/7
//#addin "nuget:?package=NuGet.Core"
//#addin "Cake.ExtendedNuGet"

// Arguments for Cake
var target                                  = Argument<string>("target", "Default");
var configuration                           = Argument<string>("configuration", "Release");

// Names
string testProjectSuffix                    = "Tests";
string cakeToolsFolderName                  = "Tools";
string buildFolderName                      = "Build";
string artifactsFolderName                  = "Artifacts";
string globalTestsFolderName                = "Tests";
string intermediateName                     = "Obj";
string nUnitRunnerName                      = "nunit3-console.exe";
string nUnitTestResultsName                 = "Test run results.xml";
string releaseNotesOutputName               = "Release notes.md";

// Important folders
DirectoryPath root                          = MakeAbsolute(new DirectoryPath("./"));
DirectoryPath toolsPath                     = root.Combine(cakeToolsFolderName);
DirectoryPath globalBuildPath               = root.Combine(buildFolderName);
DirectoryPath globalConfigurationPath       = globalBuildPath.Combine(Directory(configuration));
DirectoryPath artifactsPath                 = root.Combine(artifactsFolderName);
DirectoryPath globalTestsPath               = globalBuildPath.Combine(globalTestsFolderName);
DirectoryPath intermediatePath              = globalBuildPath.Combine(intermediateName).Combine(Directory(configuration));

// Solutions, projects and their .nuspec's
IEnumerable<FilePath> solutions            = GetFiles("./**/*.sln");
IEnumerable<DirectoryPath> solutionPaths   = solutions.Select(solution => solution.GetDirectory());
IEnumerable<FilePath> testProjects         = GetFiles("./**/*" + testProjectSuffix + ".csproj");
// TODO: DNC support
//IEnumerable<DirectoryPath> testDncProjects = GetDirectories("./**/*" + testProjectSuffix + "/");
IEnumerable<FilePath> appProjects          = GetFiles("./**/*.csproj").Except(testProjects);
IEnumerable<FilePath> nuspecs              = GetFiles("./**/*.nuspec");
IEnumerable<FilePath> dncProjects          = GetFiles("./**/project.json");

// NUnit3
FilePath nUnitRunner                       = GetFiles(toolsPath.FullPath + "/**/" + nUnitRunnerName).First();
var nUnitSettings                          = new NUnit3Settings();
nUnitSettings.NoHeader                     = true;
nUnitSettings.NoResults                    = false;
nUnitSettings.ToolPath                     = nUnitRunner.FullPath;
nUnitSettings.Results                      = globalTestsPath.CombineWithFilePath(nUnitTestResultsName);

// NuGet common settings
var nuGetPackSettings = new NuGetPackSettings
{
    BasePath                               = globalConfigurationPath,
    OutputDirectory                        = artifactsPath,
    NoPackageAnalysis                      = false,
    ArgumentCustomization                  = args => args.Append("-Prop Configuration=" + configuration ),
    Symbols                                = true,
};

var nuGetPushSettings =  new NuGetPushSettings {
    ApiKey = EnvironmentVariable("NuGetApiKey"),
    Source = "https://www.nuget.org/api/v2/package"
};

// TODO: DNC support
//var dncNuGetPacksettings = new DotNetCorePackSettings
//{
//    BuildBasePath                          = intermediatePath,
//    Configuration                          = configuration,
//    OutputDirectory                        = artifactsPath,
//    NoBuild                                = true,
//    Verbose                                = false
//};

// GitReleaseNotes settings
var gitReleaseNotesSettings = new GitReleaseNotesSettings {
    Verbose                                = true,
    //IssueTracker                           = IssueTracker.GitHub, // BitBucket, GitHub, Jira, YouTrack
    AllLabels                              = true,
    AllTags                                = true,
    //Categories                             = "Category1"
    //Version                                = "1.2.3.4"
    //RepoUserName                           = "user",
    //RepoPassword                           = "password",
    //RepoUrl                                = "http://myrepo.com",
    //RepoBranch                             = "master",
    //IssueTrackerUrl                        = "http://myissuetracker.com",
    //IssueTrackerUserName                   = "user",
    //IssueTrackerPassword                   = "password",
    //IssueTrackerProjectId                  = "1234",
};


///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////

//Setup(context =>
//{
//    // Executed BEFORE the first task.
//});

//Teardown(context =>
//{
//    // Executed AFTER the last task.
//});


///////////////////////////////////////////////////////////////////////////////
// TASK DEFINITIONS
///////////////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    // Clean solution directories.
    foreach(var path in solutionPaths)
    {
        CleanDirectories(path + "/**/bin/" + configuration);
        CleanDirectories(path + "/**/obj/" + configuration);
    }
    // Clean global build folder.
    if (DirectoryExists(globalBuildPath.Combine(configuration))) {
        CleanDirectories(globalBuildPath.Combine(configuration).FullPath);
    }
    // Clean global tests folder.
    if (DirectoryExists(globalTestsPath.Combine(configuration))) {
        CleanDirectories(globalTestsPath.Combine(configuration).FullPath);
    }

    CreateDirectory(artifactsPath);
});

void DoGitVersionForBuildServer() {
    try {
        var versionInfo = GitVersion(new GitVersionSettings{
            UpdateAssemblyInfo = true,
            OutputType = GitVersionOutput.BuildServer
        });
    } catch {
         Warning("GitVersion could not find a valid git repo.");
    }

}

Task("Version")
    .Does(() =>
{
    DoGitVersionForBuildServer();

    // TODO: DNC support
    // Update project.json
    //var versionInfo = GitVersion(new GitVersionSettings{ OutputType = GitVersionOutput.Json });
    //var updatedProjectJson = System.IO.File.ReadAllText(specifyProjectJson)
    //    .Replace("1.0.0-*", versionInfo.NuGetVersion);
    //System.IO.File.WriteAllText(specifyProjectJson, updatedProjectJson);
});

Task("GitLink")
    .Does(() =>
{
    GitLink(root.FullPath);
});

Task("Restore")
    .Does(() =>
{
    // Restore all NuGet packages.
    foreach(var solution in solutions)
    {
        Information("Restoring {0}", solution);
        NuGetRestore(solution);
        //DotNetCoreRestore(solution);
    }
});

Task("Build")
    .IsDependentOn("Clean")
    .IsDependentOn("Version")
    .IsDependentOn("Restore")
    .Does(() =>
{
    // TODO: DNC support
    //var dncSettings = new DotNetCoreBuildSettings {
    //    Configuration = configuration
    //}
    //
    //foreach(var dncProject in dncProjects) {
    //    DotNetCoreBuild(dncProject.FullPath, dncSettings);
    //}

    // Build all solutions.
    foreach(var solution in solutions)
    {
        Information("Building {0}", solution);

        if(IsRunningOnWindows())
        {
          // Use MSBuild
            MSBuild(solution, settings => settings
                .SetPlatformTarget(PlatformTarget.MSIL)
                .WithProperty("TreatWarningsAsErrors","true")
                .WithProperty("NoWarn","CS1591")
                .WithTarget("Build")
                .SetVerbosity(Verbosity.Minimal)
                .SetConfiguration(configuration));
        }
        else
        {
            // Use XBuild
            XBuild(solution, settings => settings.SetConfiguration(configuration));
        }

    }
});

Task("Run-Unit-Tests")
    .IsDependentOn("Build")
    .Does(() =>
{
    // TODO: DNC support
    //var settings = new DotNetCoreRunSettings
    //{
    //    Configuration = configuration
    //};
    //
    //foreach(var testDncProject in testDncProjects) {
    //    DotNetCoreTest(testDncProject.GetDirectory().FullPath, settings);
    //}

    // Run all test solutions.
    foreach(var testProject in testProjects)
    {
        var localTestProjectPath = testProject.GetDirectory().Combine("bin").Combine(configuration);

        Information("Searching for tests in {0}", localTestProjectPath);
        if (DirectoryExists(localTestProjectPath)) {
            var localTestAssemblies = GetFiles(localTestProjectPath.FullPath + "/*Tests.dll",
                                               fi => !fi.Path.FullPath.Contains("/obj/"));

            foreach(var testAssembly in localTestAssemblies)
            {
                Information("Running tests in {0}", testAssembly.FullPath);
                NUnit3(testAssembly.FullPath, nUnitSettings);
            }
        }
    }

    Information("Searching for tests in {0}", globalTestsPath.Combine(configuration));
    if (DirectoryExists(globalTestsPath.Combine(configuration))) {
        var globalTestAssemblies = GetFiles(globalTestsPath.Combine(configuration).FullPath + "/*Tests.dll",
                                            fi => !fi.Path.FullPath.Contains("/obj/"));
        foreach(var testAssembly in globalTestAssemblies)
        {
            Information("Running tests in {0}", testAssembly.FullPath);
            NUnit3(testAssembly.FullPath, nUnitSettings);
        }
    }
});


Task ("Package")
    .IsDependentOn("Run-Unit-Tests")
    .Does (() =>
{
    // TODO: DNC support
    //DotNetCorePack(projectJsonPath, settings);

    // Pack all .nuspecs
    try {
        var versionInfo = GitVersion(new GitVersionSettings{ OutputType = GitVersionOutput.Json });
    } catch {
         Warning("GitVersion could not find a valid git repo.");
    }

    foreach(var nuspec in nuspecs) {
        var assemblyInfoFiles = System.IO.Directory.GetFiles(nuspec.GetDirectory().FullPath, "AssemblyInfo.cs",
                                                             SearchOption.AllDirectories);
        var assemblyInfoFile = assemblyInfoFiles.First();
        var assemblyInfo = ParseAssemblyInfo(File(assemblyInfoFile));

        Information("Packing nuget for {0}", nuspec);
        nuGetPackSettings.Copyright     = assemblyInfo.Copyright;
        nuGetPackSettings.Version       = assemblyInfo.AssemblyVersion;
        NuGetPack(nuspec, nuGetPackSettings);
    }
});

Task("DeployNuget")
    .IsDependentOn("Run-Unit-Tests")
    .IsDependentOn("Package")
    .Does(() =>
{
    var allPackages = GetFiles(artifactsPath.FullPath + "/*.nupkg").ToArray();
    if (allPackages.Length == 0) {
        Error("There are no nuget packages in '" + artifactsPath.FullPath + ".");
        return;
    }

    var packagesWithVersions = from packageFile in allPackages
                               where !packageFile.FullPath.EndsWith(".symbols.nupkg", StringComparison.OrdinalIgnoreCase)
                               select new { File = packageFile, Version = ExtractVersion(packageFile) };
    FilePath latestNuGetPackage = packagesWithVersions.OrderByDescending(t => t.Version).First().File;
    NuGetPush(latestNuGetPackage.FullPath, nuGetPushSettings);
});

///////////////////////////////////////////////////////////////////////////////
// TARGETS
///////////////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Run-Unit-Tests")
    .IsDependentOn("Package");


///////////////////////////////////////////////////////////////////////////////
// EXECUTION
///////////////////////////////////////////////////////////////////////////////

RunTarget(target);


// Auxiliary

private Version ExtractVersion(FilePath filename)
{
    var regex = new Regex(@"\d+(\.\d+)+");
    var matches = regex.Matches(filename.FullPath).Cast<Match>().Where(m => m.Success).ToArray();
    if (matches.Length == 0) {
        Error("Could not find version in file '" + filename + "'.");
    }

    var lastMatch = matches[matches.Length - 1];
    return new Version(lastMatch.Value);
}