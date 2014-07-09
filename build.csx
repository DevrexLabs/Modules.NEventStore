using System.Text.RegularExpressions;
 
var target = Argument("target", "NuGet");
var config = Argument("config", "Release");
var output = "./build";
var version = ParseVersion("./src/OrigoDB.Modules.NEventStore/Properties/AssemblyInfo.cs");
 
if(version == null)
{
   // We make sure the version is set.
   throw new InvalidOperationException("Could not parse version.");
}
 
////////////////////////////////////////////////
// TASKS
////////////////////////////////////////////////
 
Task("Clean")
   .Does(() =>
{
   CleanDirectory(output);
});
 
 Task("Restore-NuGet-Packages")
	.IsDependentOn("Clean")
	.Does(() =>
{
	NuGetRestore("./src/OrigoDB.Modules.NEventStore.sln");
});

Task("Build")
   .IsDependentOn("Clean")
   .Does(() =>
{
   MSBuild("./src/OrigoDB.Modules.NEventStore/OrigoDB.Modules.NEventStore.csproj", settings => 
      settings.SetConfiguration(config)
		 .UseToolVersion(MSBuildToolVersion.VS2012)
         .WithTarget("clean")
         .WithTarget("build")); 	
});


Task("Copy")
   .IsDependentOn("Build")
   .Does(() =>
{
   var pattern = "src/OrigoDB.*/bin/" + config + "/OrigoDB.*.dll";
   CopyFiles(pattern, output);
});

Task("Zip")
   .IsDependentOn("Copy")
   .Does(() =>
{
   var root = "./build/";
   var output = "./build/OrigoDB.Modules.NEventStore.binaries." + version + "-" + config + ".zip";
   var files = root + "/*";
 
   // Package the bin folder.
   Zip(root, output);	
});
 
Task("NuGet")
   .IsDependentOn("Zip")
   .Does(() =>
{
   NuGetPack("./OrigoDB.NEventStore.nuspec", new NuGetPackSettings {
      Version = version,
      OutputDirectory = "./build",
	  Symbols = true
   });
});
 
////////////////////////////////////////////////
// RUN TASKS
////////////////////////////////////////////////
 
RunTarget(target);

////////////////////////////////////////////////
// UTILITIES
////////////////////////////////////////////////
 
private string ParseVersion(string filename)
{
   var file = FileSystem.GetFile(filename);
   using(var reader = new StreamReader(file.OpenRead()))
   {
      var text = reader.ReadToEnd();
      Regex regex = new Regex(@"AssemblyVersion\(""(?<theversionnumber>\d+\.\d+\.\d+)""\)");
      return regex.Match(text).Groups["theversionnumber"].Value;
   }
}
