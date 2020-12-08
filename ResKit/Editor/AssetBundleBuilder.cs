using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace MiniFramework
{
	public class AssetBundleBuilder
	{
		public const string SimulationModePath = "MiniTools/模拟真机资源加载";
		[MenuItem(SimulationModePath)]
		static void SetSimulationMode()
		{
			Menu.SetChecked(SimulationModePath, !Menu.GetChecked(SimulationModePath));
		}

		[MenuItem("MiniTools/Build AssetBundles/StandaloneOSX")]
		static void BuildAssetBundlesForOSX()
		{
			BuildAssetBundles(Path.Combine(Application.streamingAssetsPath, "AssetBundles/StandaloneOSX"), BuildTarget.StandaloneOSX);
		}

		[MenuItem("MiniTools/Build AssetBundles/iOS")]
		static void BuildAssetBundlesForIOS()
		{
			BuildAssetBundles(Path.Combine(Application.streamingAssetsPath, "AssetBundles/iOS"), BuildTarget.iOS);
		}

		[MenuItem("MiniTools/Build AssetBundles/Android")]
		static void BuildAssetBundlesForAndroid()
		{
			BuildAssetBundles(Path.Combine(Application.streamingAssetsPath, "AssetBundles/Android"), BuildTarget.Android);
		}

		[MenuItem("MiniTools/Build AssetBundles/StandaloneWindows")]
		static void BuildAssetBundlesForWindows()
		{
			BuildAssetBundles(Path.Combine(Application.streamingAssetsPath, "AssetBundles/StandaloneWindows"), BuildTarget.StandaloneWindows);
		}

		static void BuildAssetBundles(string ABPath, BuildTarget target)
		{
			if (!File.Exists(ABPath)) {
				Directory.CreateDirectory(ABPath);
			}
			BuildPipeline.BuildAssetBundles(ABPath, BuildAssetBundleOptions.ChunkBasedCompression, target);
			AssetDatabase.Refresh();
			Debug.Log(target + " 端AB包打包完毕");
		}
	}
}

