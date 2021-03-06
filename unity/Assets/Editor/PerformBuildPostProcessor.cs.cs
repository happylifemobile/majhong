﻿using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using ChillyRoom.UnityEditor.iOS.Xcode;

/*
	A simple, shared PostProcessBuildPlayer script to enable Objective-C modules. This lets us add frameworks
	from our source files, rather than through modifying the Xcode project. 
*/

public class PerformBuildPostProcessor
{

	[PostProcessBuild (10000)] // We should try to run last
	public static void OnPostProcessBuild (BuildTarget BuildTarget, string path)
	{
		#if (UNITY_5 && UNITY_IOS)
		UnityEngine.Debug.Log ("ScarletPostProcessor: Enabling Objective-C modules");
		string pbxprojPath = path + "/Unity-iPhone.xcodeproj/project.pbxproj";
		PBXProject project = new PBXProject ();
		project.ReadFromString (File.ReadAllText (pbxprojPath));
		string target = project.TargetGuidByName ("Unity-iPhone");

		project.SetBuildProperty (target, "CLANG_ENABLE_MODULES", "YES");
		project.AddBuildProperty (target, "OTHER_LDFLAGS", "$(inherited)");
//		project.RemoveFrameworkFromProject (target, "Metal.framework");

		File.WriteAllText (pbxprojPath, project.WriteToString ());

		//remove libPods-Unity-iPhone.a generated by google admob
		List<string> lines = new List<string> ();
		string foundKeyword = "libPods-Unity-iPhone.a";
		foreach (string str in File.ReadAllLines(pbxprojPath)) {
			if (!str.Contains (foundKeyword)) { 
				lines.Add (str);
			} else {
//				Debug.Log (str);
			}
		}
		File.WriteAllLines (pbxprojPath, lines.ToArray ());
		NativeLocale.AddLocalizedStringsIOS (path, Path.Combine (Application.dataPath, "Locale/iOS"));
		//
		#else
		UnityEngine.Debug.Log("Unable to modify build settings in XCode project. Build " +
		                        "settings must be set manually");
		#endif

//		ModifyFrameworkAndInfoList();
	}

	public static void ModifyFrameworkAndInfoList (BuildTarget BuildTarget, string path)
	{
		if (BuildTarget == BuildTarget.iOS) {
			string projPath = PBXProject.GetPBXProjectPath (path);
			PBXProject proj = new PBXProject ();

			proj.ReadFromString (File.ReadAllText (projPath));
			string xtarget = proj.TargetGuidByName ("Unity-iPhone");

			// add extra framework(s)
			proj.AddFrameworkToProject (xtarget, "AssetsLibrary.framework", false);

			// set code sign identity & provisioning profile
			proj.SetBuildProperty (xtarget, "CODE_SIGN_IDENTITY", "iPhone Distribution: _______________");
			proj.SetBuildProperty (xtarget, "PROVISIONING_PROFILE", "********-****-****-****-************"); 

			// rewrite to file
			File.WriteAllText (projPath, proj.WriteToString ());

			// 由于我的开发机是英文系统，但游戏需要设置为中文；
			// 需要在修改 Info.plist 中的 CFBundleDevelopmentRegion 字段为 zh_CN

			// Get plist
			string plistPath = path + "/Info.plist";
			PlistDocument plist = new PlistDocument ();
			plist.ReadFromString (File.ReadAllText (plistPath));

			// Get root
			PlistElementDict rootDict = plist.root;

			// Change value of CFBundleDevelopmentRegion in Xcode plist
			rootDict.SetString ("CFBundleDevelopmentRegion", "zh_CN");

			PlistElementArray urlTypes = rootDict.CreateArray ("CFBundleURLTypes");

			// add weixin url scheme
			PlistElementDict wxUrl = urlTypes.AddDict ();
			wxUrl.SetString ("CFBundleTypeRole", "Editor");
			wxUrl.SetString ("CFBundleURLName", "weixin");
			PlistElementArray wxUrlScheme = wxUrl.CreateArray ("CFBundleURLSchemes");
			wxUrlScheme.AddString ("____________");            

			// Write to file
			File.WriteAllText (plistPath, plist.WriteToString ());
		}
	}
}