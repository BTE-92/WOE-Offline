using System;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

public static class BuildIOS
{
	public static void Build()
	{
		PlayerSettings.SplashScreen.show = false;
		PlayerSettings.SplashScreen.showUnityLogo = false;
		PlayerSettings.SetScriptingBackend(BuildTargetGroup.iOS, ScriptingImplementation.IL2CPP);
		// WOE_IOS_ARCH: "arm64" (default), "armv7" or "universal". Which slice to BUILD is chosen later with xcodebuild ARCHS=.
		string arch = (Environment.GetEnvironmentVariable("WOE_IOS_ARCH") ?? "arm64").ToLowerInvariant();
		PlayerSettings.SetArchitecture(BuildTargetGroup.iOS, arch == "arm64" ? 1 : (arch == "armv7" ? 0 : 2)); // 0 = ARMv7, 1 = ARM64, 2 = Universal
		PlayerSettings.iOS.targetOSVersionString = "9.0";
		PlayerSettings.iOS.buildNumber = "1";
		PlayerSettings.iOS.appleEnableAutomaticSigning = true;
		PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, "com.traplight.whatonearth");

		BuildPlayerOptions options = new BuildPlayerOptions();
		options.scenes = new string[] { "Assets/MainScene.unity" };
		string outDir = Environment.GetEnvironmentVariable("WOE_IOS_OUT"); // default: <project>/Builds/iOS
		options.locationPathName = string.IsNullOrEmpty(outDir) ? "Builds/iOS" : outDir;
		options.target = BuildTarget.iOS;
		options.targetGroup = BuildTargetGroup.iOS;
		options.options = BuildOptions.None;

		BuildReport report = BuildPipeline.BuildPlayer(options);
		Console.WriteLine("BUILD RESULT: iOS " + report.summary.result + ", errors: " + report.summary.totalErrors + ", size: " + report.summary.totalSize);
		EditorApplication.Exit(report.summary.result == BuildResult.Succeeded ? 0 : 2);
	}

	[PostProcessBuild(999)]
	public static void FixXcodeProject(BuildTarget target, string path)
	{
		if (target != BuildTarget.iOS)
		{
			return;
		}
		string projPath = PBXProject.GetPBXProjectPath(path);
		PBXProject project = new PBXProject();
		project.ReadFromFile(projPath);
		string main = project.TargetGuidByName("Unity-iPhone");
		project.SetBuildProperty(main, "ENABLE_BITCODE", "NO");
		project.SetBuildProperty(project.ProjectGuid(), "ENABLE_BITCODE", "NO");
		// Chipmunk C sources (Libraries/Plugins/iOS/Chipmunk): include paths + C standard (original CMake used gnu99)
		project.AddBuildProperty(main, "HEADER_SEARCH_PATHS", "$(SRCROOT)/Libraries/Plugins/iOS/Chipmunk/include");
		project.AddBuildProperty(main, "HEADER_SEARCH_PATHS", "$(SRCROOT)/Libraries/Plugins/iOS/Chipmunk/include/chipmunk");
		project.SetBuildProperty(main, "GCC_C_LANGUAGE_STANDARD", "gnu99");
		// FMOD needs these system frameworks
		project.AddFrameworkToProject(main, "AudioToolbox.framework", false);
		project.AddFrameworkToProject(main, "AVFoundation.framework", false);
		project.WriteToFile(projPath);

		// Push + Game Center need a paid developer account: strip them so a free Apple ID can sign the app.
		string text = File.ReadAllText(projPath);
		text = Regex.Replace(text, @"\s*com\.apple\.(GameCenter\.iOS|GameControllers\.appletvos|Push) = \{\s*enabled = 1;\s*\};", string.Empty);
		text = text.Replace("CODE_SIGN_ENTITLEMENTS = whatonearth.entitlements;", "CODE_SIGN_ENTITLEMENTS = \"\";");
		File.WriteAllText(projPath, text);
		string entitlements = Path.Combine(path, "whatonearth.entitlements");
		if (File.Exists(entitlements))
		{
			File.WriteAllText(entitlements, "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n<!DOCTYPE plist PUBLIC \"-//Apple//DTD PLIST 1.0//EN\" \"http://www.apple.com/DTDs/PropertyList-1.0.dtd\">\n<plist version=\"1.0\">\n  <dict>\n  </dict>\n</plist>\n");
		}
		// arm64-only export: Unity leaves "armv7" in UIRequiredDeviceCapabilities, which makes new iOS versions reject it.
		// Armv7/universal exports keep Unity's default ("armv7", which 64-bit devices also satisfy).
		if (PlayerSettings.GetArchitecture(BuildTargetGroup.iOS) == 1)
		{
			string plistPath = Path.Combine(path, "Info.plist");
			PlistDocument plist = new PlistDocument();
			plist.ReadFromFile(plistPath);
			PlistElementArray caps = plist.root.CreateArray("UIRequiredDeviceCapabilities");
			caps.AddString("arm64");
			plist.WriteToFile(plistPath);
		}
		Console.WriteLine("POSTPROCESS: bitcode off, Chipmunk paths, audio frameworks, push/GameCenter capabilities removed");
	}
}
