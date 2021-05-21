using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.Npm;
using Nuke.Common.Tools.NuGet;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;

class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    public static int Main () => Execute<Build>(x => x.Compile);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Solution("src/api/Payment.Tracker/Payment.Tracker.sln")]
    readonly Solution Solution;
    readonly AbsolutePath SourceDirectory = RootDirectory / "src";
    readonly AbsolutePath AppDirectory = RootDirectory / "src" / "app" / "payment-tracker";
    readonly AbsolutePath PublishDirectory = RootDirectory / "publish";

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            SourceDirectory.GlobDirectories("**/bin", "**/obj", "**/dist").ForEach(path => Directory.Delete(path, true));
            var nodeModulesPath = AppDirectory / "node_modules";
            if (Directory.Exists(nodeModulesPath))
            {
                DeleteDirectory(nodeModulesPath);
                //Directory.Delete(nodeModulesPath);
            }

            var distPath = AppDirectory / "www";
            if (Directory.Exists(distPath))
            {
                Directory.Delete(distPath);
            }

            DeleteDirectory(PublishDirectory);
        });

    Target Restore => _ => _
        .Executes(() =>
        {
            NpmTasks.NpmInstall(settings =>
                settings
                    .SetProcessLogOutput(true)
                    .SetProcessWorkingDirectory(AppDirectory));
            
            NuGetTasks.NuGetRestore(settings =>
                settings
                    .SetTargetPath(Solution)
                    .EnableNoCache());
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(async () =>
        {
            var args = "build";
            if (Equals(Configuration, Configuration.Release))
            {
                args += " --prod";
            }
                
            await RunIonicAsync(args);

            DotNetTasks.DotNetBuild(settings =>
                settings
                    .SetProjectFile(Solution.GetProject("Payment.Tracker.Api")?.Path)
                    .SetConfiguration(Configuration)
                    .SetVerbosity(DotNetVerbosity.Minimal));
        });

    Target Publish => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            AbsolutePath srcDir = AppDirectory / "www";
            AbsolutePath destDir = PublishDirectory / "app";
            
            CopyDirectoryRecursively(srcDir, destDir);
            
            DotNetTasks.DotNetPublish(settings =>
                settings
                    .SetConfiguration(Configuration)
                    .SetVerbosity(DotNetVerbosity.Minimal)
                    .SetFramework("net5.0")
                    .SetRuntime("linux-x64")
                    .SetProject(Solution.GetProject("Payment.Tracker.Api"))
                    .SetOutput(PublishDirectory / "api"));
        });

    async Task RunIonicAsync(string command)
    {
        Process proc = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                WorkingDirectory = AppDirectory,
                FileName = "ionic",
                Arguments = command
            }
        };

        proc.Start();
        await proc.WaitForExitAsync();
    }
}
