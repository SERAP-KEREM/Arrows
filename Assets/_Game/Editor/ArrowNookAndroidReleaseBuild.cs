using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace ArrowNook.Editor
{
    public static class ArrowNookAndroidReleaseBuild
    {
        [MenuItem("Tools/Arrow Game/Build Android App Bundle")]
        public static void BuildAndroidAppBundle()
        {
            if (!EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android))
                throw new BuildFailedException("Could not switch build target to Android.");

            EditorUserBuildSettings.buildAppBundle = true;
            ArrowNookReleaseGuard.ValidatePlayStoreRelease();

            string[] scenes = EditorBuildSettings.scenes
                .Where(scene => scene.enabled)
                .Select(scene => scene.path)
                .ToArray();
            if (scenes.Length == 0)
                throw new BuildFailedException("No enabled scenes are configured in Build Settings.");

            string outputDir = "Builds/Android";
            Directory.CreateDirectory(outputDir);
            string outputPath = Path.Combine(outputDir, $"ArrowNook-{PlayerSettings.bundleVersion}.aab");
            var options = new BuildPlayerOptions
            {
                scenes = scenes,
                locationPathName = outputPath,
                target = BuildTarget.Android,
                options = BuildOptions.None
            };

            BuildReport report = BuildPipeline.BuildPlayer(options);
            if (report.summary.result != BuildResult.Succeeded)
                throw new BuildFailedException($"Android App Bundle build failed: {report.summary.result}");

            Debug.Log($"Android App Bundle created: {outputPath}");
        }
    }
}
