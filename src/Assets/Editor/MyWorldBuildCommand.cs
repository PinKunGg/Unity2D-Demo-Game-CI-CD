using System.IO.Compression;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class MyWorldBuildCommand
{
    #region Entry point

    public static void CustomBuildAndroid()
    {
        ApplyAndroid();
    }

    public static void CustomBuildWindows()
    {
        ApplyWindows();
    }

    public static void BuildAndroidDevelopment()
    {
        ApplyAndroid(GetScriptingDefines("DevMode", "SERVER_DEVELOPMENT"));
    }
    public static void BuildAndroidStaging()
    {
        ApplyAndroid(GetScriptingDefines("DevMode", "SERVER_STAGING"));
    }
    public static void BuildAndroidProductionDebug()
    {
        ApplyAndroid(GetScriptingDefines("DevMode", "SERVER_PRODUCTION"));
    }
    public static void BuildAndroidProduction()
    {
        ApplyAndroid(GetScriptingDefines("SERVER_PRODUCTION"));
    }
    // public static void BuildIosDevelopment()
    // {
    //     ApplyIos(GetScriptingDefines("DevMode", "SERVER_DEVELOPMENT"));
    // }

    // public static void BuildIosStaging()
    // {
    //     ApplyIos(GetScriptingDefines("DevMode", "SERVER_STAGING"));
    // }

    // public static void BuildIosProductionDebug()
    // {
    //     ApplyIos(GetScriptingDefines("DevMode", "SERVER_PRODUCTION"));
    // }

    // public static void BuildIosProduction()
    // {
    //     ApplyIos(GetScriptingDefines("SERVER_PRODUCTION"));
    // }
    #endregion


    private static void ApplyAndroid(string[] scriptsDefines)
    {
        EditorUserBuildSettings.buildAppBundle = true;
        PlayerSettings.keystorePass = Environment.GetEnvironmentVariable("ANDROID_KEYSTORE_PASS");
        PlayerSettings.keyaliasPass = Environment.GetEnvironmentVariable("ANDROID_KEYALIAS_PASS");
        BuildReport result = BuildPipeline.BuildPlayer(new BuildPlayerOptions
        {
            locationPathName = "Builds/Android/android.aab",
            scenes = GetScences(),
            target = BuildTarget.Android,
            targetGroup = BuildTargetGroup.Android,
            options = BuildOptions.CompressWithLz4,
            extraScriptingDefines = scriptsDefines,
        });

        PrintGameciResult(result.summary);
    }

    private static void ApplyAndroid()
    {
        BuildPlayerOptions buildOpstions = new()
        {
            options = BuildOptions.Development,
            scenes = GetScences(),
            locationPathName = "Builds/Android/android.apk",
            target = BuildTarget.Android,
            targetGroup = BuildTargetGroup.Android
        };

        var result = BuildPipeline.BuildPlayer(buildOpstions);

        PrintGameciResult(result.summary);
    }

    private static void ApplyWindows()
    {
        Console.WriteLine($">>>>>License: {UnityEngine.Windows.LicenseInformation.isOnAppTrial}");

        PlayerSettings.SplashScreen.show = true;
        PlayerSettings.SplashScreen.showUnityLogo = true;

        BuildPlayerOptions buildOpstions = new()
        {
            options = BuildOptions.Development,
            scenes = GetScences(),
            locationPathName = "Builds/Android/android.aab",
            target = BuildTarget.StandaloneWindows64,
            targetGroup = BuildTargetGroup.Standalone
        };

        var result = BuildPipeline.BuildPlayer(buildOpstions);

        PrintGameciResult(result.summary);
    }

    // private static void ApplyIos(string[] scriptsDefines)
    // {
    //     EditorUserBuildSettings.buildAppBundle = true;
    //     BuildReport result = BuildPipeline.BuildPlayer(new BuildPlayerOptions
    //     {
    //         locationPathName = "Builds/iOS/",
    //         scenes = GetScences(),
    //         target = BuildTarget.iOS,
    //         targetGroup = BuildTargetGroup.iOS,
    //         options = BuildOptions.CompressWithLz4,
    //         extraScriptingDefines = scriptsDefines,
    //     });
    //     PrintGameciResult(result.summary);
    // }

    private static string[] GetScriptingDefines(params string[] additional)
    {
        return additional
            .Append("UNITY_POST_PROCESSING_STACK_V2")
            .Append("USING_ADDRESSABLES")
            .Append("CT_RADIO")
            .Append("CT_UI")
            .Append("CT_RADIO_DEMO")
            .ToArray();
    }


    private static string[] GetScences()
    {
        List<string> scenes = new List<string>();
        foreach (var scene in EditorBuildSettings.scenes)
        {
            if (scene.enabled)
                scenes.Add(scene.path);
        }
        return scenes.ToArray();
    }




    /// <summary>
    /// print build result required by game-ci v3
    /// </summary>
    /// <param name="summary"></param>
    private static void PrintGameciResult(BuildSummary summary)
    {
        if (summary.result != BuildResult.Succeeded)
        {
            Console.WriteLine(
                $"{Environment.NewLine}" +
                $"xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx{Environment.NewLine}" +
                $"x      !! Build Failed !!       x{Environment.NewLine}" +
                $"xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx{Environment.NewLine}" +
                $"{Environment.NewLine}" +
                $"Duration: {summary.totalTime}{Environment.NewLine}" +
                $"Warnings: {summary.totalWarnings}{Environment.NewLine}" +
                $"Errors: {summary.totalErrors}{Environment.NewLine}" +
                $"Size: {summary.totalSize} bytes{Environment.NewLine}" +
                $"{Environment.NewLine}"
            );
            return;
        }
        // This format is required by the game-ci build action
        Console.WriteLine(
                $"{Environment.NewLine}" +
                $"###########################{Environment.NewLine}" +
                $"#      Build results      #{Environment.NewLine}" +
                $"###########################{Environment.NewLine}" +
                $"{Environment.NewLine}" +
                $"Duration: {summary.totalTime}{Environment.NewLine}" +
                $"Warnings: {summary.totalWarnings}{Environment.NewLine}" +
                $"Errors: {summary.totalErrors}{Environment.NewLine}" +
                $"Size: {summary.totalSize} bytes{Environment.NewLine}" +
                $"{Environment.NewLine}"
        );
    }
}