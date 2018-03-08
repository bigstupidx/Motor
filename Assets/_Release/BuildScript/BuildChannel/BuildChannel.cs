#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Xml;
using UnityEditor;
using UnityEngine;
using XPlugin.AutoBuilder;
using XPlugin.Data.Json;
using XPlugin.Localization;
using XPlugin.Security;
using Debug = UnityEngine.Debug;

public class BuildChannel : SingleBuildBehaviour {

	[System.Serializable]
	public class Channel {
		public bool Build = true;
		public string Name;
	}

	public string ChannelSdkPath = "/Build/Android/SDK";
	public string VersionName;
	public string VersionCode;
	public int VersionId;//给服务器的版本代号


	public string AESKey = "3b5949e0c26b8776";
	public string AESIv = "7a4752a276de9570";


	[HideInInspector]
	public List<Channel> Channels;

	public override void OnSingleBuildFinish(bool success, SingleBuild arg) {
		foreach (var channel in this.Channels) {
			try {
				if (!channel.Build) {
					continue;
				}
				EditorUtility.DisplayProgressBar("编译APK中", "准备编译渠道包：" + channel.Name, 0);
				var newProjPath = GenerateSdkProject(arg.TargetPath, channel.Name);
				BuildProject(newProjPath);

			} catch (Exception e) {
				Debug.LogException(e);
			}
			EditorUtility.ClearProgressBar();
		}
	}

	public string GetChannelProjPath(string originProj, string channel) {
		return originProj + "-" + channel;
	}

	public string GenerateSdkProject(string originProj, string channel) {
		var newProjPath = GetChannelProjPath(originProj, channel);
		if (Directory.Exists(newProjPath)) {
			Directory.Delete(newProjPath, true);
		}
		FileHelper.DirCopy(originProj, newProjPath);

		var sdkPath = Path.Combine(this.ChannelSdkPath, channel);
		// 复制渠道SDK
		FileHelper.DirCopy(sdkPath, newProjPath);
		//修改版本号
		ApplyVersion(newProjPath);
		//处理splash.png
		var splashFilePath = Path.Combine(newProjPath, "splash.png");
		if (File.Exists(splashFilePath)) {
			var targetSplashFilePath = Path.Combine(newProjPath, "assets/bin/data/splash.png");
			if (File.Exists(targetSplashFilePath)) {
				File.Delete(targetSplashFilePath);
			}
			File.Move(splashFilePath, targetSplashFilePath);
		}
		//处理icon
		ApplyAndroidIcon(newProjPath);
		//加密config
		EncryptConfig(Path.Combine(newProjPath, "assets/Config.txt"));

		Debug.Log("生成工程:" + newProjPath);
		return newProjPath;
	}

	public void BuildProject(string projectPath, bool async = false) {
		var shFile = "";
		if (Application.platform == RuntimePlatform.WindowsEditor) {
			shFile = "BuildAPK.bat";
		} else {
			shFile = "BuildAPK.sh";
		}
		Shell(Path.Combine(projectPath, shFile), Path.Combine("../", Path.GetFileName(projectPath) + ".apk"), false, async);
		Debug.Log("构建工程结束:" + projectPath);
	}

	public static void Shell(string file, string args, bool changeWorkingDir = false, bool async = false) {
		if (!File.Exists(file)) {
			Debug.LogError("脚本文件" + file + "未找到！");
			return;
		}

		ProcessStartInfo startinfo = new ProcessStartInfo();
		startinfo.FileName = file;
		startinfo.Arguments = args;
		if (changeWorkingDir) {
			startinfo.WorkingDirectory = new FileInfo(file).Directory.FullName;
		}
		startinfo.UseShellExecute = false;
		startinfo.RedirectStandardOutput = true;

		Debug.Log("Shell Start: " + startinfo.FileName + " " + startinfo.Arguments + "\nWorkingDirectory:" + startinfo.WorkingDirectory);

		Process p = Process.Start(startinfo);
		if (!async) {
			p.WaitForExit();
			Debug.Log("Shell Complete!\n" + p.StandardOutput.ReadToEnd());
		}
	}

	public static void ApplyAndroidIcon(string projPath) {
		//这里需要用反射，因为System.Draw.dll在Editor的dll下，不能让它进入工程
		Assembly assembly = Assembly.Load("Assembly-CSharp-Editor-firstpass");
		assembly.GetType("IconUtil").GetMethod("ApplyAndroidIcon", BindingFlags.Static | BindingFlags.Public)
			.Invoke(null, new object[] { projPath });
	}

	public void EncryptConfig(string path) {
		Debug.Log("加密:"+path);
		AESFileUtil.WriteAllBytes(path, File.ReadAllBytes(path),this.AESKey, this.AESIv);
	}


	public void ApplyVersion(string path) {
		//修改AndroidManifest.xml中的versionName和versionCode
		string xmlPath = Path.Combine(path, "AndroidManifest.xml");
		XmlDocument xmldoc = new XmlDocument();
		xmldoc.Load(xmlPath);

		XmlElement manifest = (XmlElement)xmldoc.SelectSingleNode("/manifest");
		//XmlAttribute package = manifest.Attributes["package"];
		//package.Value = BuildMgr.Ins.Profile.Package;
		XmlAttribute verName = manifest.Attributes["android:versionName"];
		verName.Value = VersionName;
		XmlAttribute verCode = manifest.Attributes["android:versionCode"];
		verCode.Value = VersionCode;

		XmlWriterSettings setting = new XmlWriterSettings();
		//setting.NewLineOnAttributes = true;
		setting.Indent = true;
		setting.IndentChars = "\t";
		setting.NewLineChars = "\n";
		setting.Encoding = new System.Text.UTF8Encoding(false);
		var writer = XmlWriter.Create(xmlPath, setting);
		xmldoc.Save(writer);
		xmldoc = null;
		writer.Close();
		writer = null;
		
		GC.Collect();

		//修改Config.bin中的versionId
		string configPath = Path.Combine(path, "assets/Config.txt");
		string configText = File.ReadAllText(configPath);

		//Write
		JObject root = JObject.OptParse(configText);
		root["VersionId"] = VersionId;

		//Save
		FileInfo fileTest = new FileInfo(configPath);
		File.WriteAllText(fileTest.FullName, root.ToFormatString());
	}
}
#endif