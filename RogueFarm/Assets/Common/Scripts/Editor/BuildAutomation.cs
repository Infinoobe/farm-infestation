#if UNITY_EDITOR
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using Debug = UnityEngine.Debug;

namespace Editor
{
    public class BuildAutomation
    {
        [MenuItem("Itch/Build and upload")]
        public static void BuildGame()
        {
            var scenes = new []{ "Assets/Scenes/SampleScene.unity" }; // Scenes to include in the build
            BuildOptions buildOptions = BuildOptions.None;   // Build Options (e.g. Development build)
            BuildTarget buildTarget = BuildTarget.WebGL; // Target platform

            var buildPath = Path.Combine("Builds", buildTarget.ToString()); // Output path

            var report = BuildPipeline.BuildPlayer(scenes, buildPath, buildTarget, buildOptions);
            if (report.summary.result != BuildResult.Succeeded)
            {
                Debug.LogError("Build Failed");
                return;
            }
            
            var f = new FileInfo(report.summary.outputPath);
            var zipFile = f.FullName + ".zip";
            
            ZipArchive(f.FullName, zipFile);
            Debug.Log("Archive complete");

            ButlerItchUpload(zipFile, "infinoobe/farm-infestation:web");
            Debug.Log("Upload complete");
        }

        private static void ButlerItchUpload(string zipFile, string itchTarget)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "butler",
                Arguments = $"push \"{zipFile}\" {itchTarget}",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process process = new Process { StartInfo = startInfo })
            {
                process.Start();
                process.StandardOutput.ReadToEnd();
                process.WaitForExit();
            }
        }

        public static void ZipArchive( string folderToZipPath, string outputArchivePath)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "7z.exe",
                Arguments = $"a \"{outputArchivePath}\" \"{folderToZipPath}\\*\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process process = new Process { StartInfo = startInfo })
            {
                process.Start();
                process.StandardOutput.ReadToEnd();
                process.WaitForExit();
            }
        }
    }
}
#endif
