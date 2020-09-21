#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using System.IO;

namespace MiniFramework
{
	public class EditorResLoader : IResLoader
	{
		public EditorResLoader()
		{
		}

		~EditorResLoader()
		{
			Release();
		}


		//		/// <summary>
		//		/// 从所有的AssetBundle中找是否存在名为 resName 的资源
		//		/// </summary>
		//		/// <typeparam name="T"></typeparam>
		//		/// <param name="resName"></param>
		//		/// <returns></returns>
		//		public T LoadSync<T>(string resName) where T : UnityEngine.Object
		//		{
		//#if UNITY_EDITOR
		//			// 遍历所有的AssetBundle
		//			string[] allABNames = AssetDatabase.GetAllAssetBundleNames();
		//			foreach(var abName in allABNames) {
		//				//Debug.Log("AssetBundleName: " + abName);
		//				// 遍历AssetBundle中所有资源的路径
		//				string[] assetPaths = AssetDatabase.GetAssetPathsFromAssetBundle(abName);
		//				foreach(var asset_path in assetPaths) {
		//					//Debug.Log("Asset Path: " + asset_path);
		//					// 获取对应路径的资源类型及名称
		//					//Type type = AssetDatabase.GetMainAssetTypeAtPath(asset_path);
		//					DirectoryInfo dirInfo = new DirectoryInfo(asset_path);
		//					// 当前默认不会出现资源名重复的问题，包括不同类型的文件也不能重复， 否则有可能导致最后加载出来的资源有误
		//					if(dirInfo.Name.Split('.')[0].ToLower() == resName.ToLower()) {
		//						// 找到了
		//						return AssetDatabase.LoadAssetAtPath<T>(asset_path);
		//					} 
		//				}
		//			}
		//			Debug.LogError("Asset Resource \" " + resName + " \" Didn't Mark a AssetBundle Tag");
		//#endif
		//			return null;
		//		}

		public T LoadSync<T>(string assetName, string assetBundleName) where T : UnityEngine.Object
		{
			//Debug.Log("Called From EditorResLoader");
			// 遍历AssetBundle中所有资源的路径
			string[] assetPaths = AssetDatabase.GetAssetPathsFromAssetBundle(assetBundleName.ToLower());
			foreach (var asset_path in assetPaths) {
				//Debug.Log("Asset Path: " + asset_path);
				// 获取对应路径的资源类型及名称
				//Type type = AssetDatabase.GetMainAssetTypeAtPath(asset_path);
				DirectoryInfo dirInfo = new DirectoryInfo(asset_path);
				// 当前默认不会出现资源名重复的问题，包括不同类型的文件也不能重复， 否则有可能导致最后加载出来的资源有误
				if (dirInfo.Name.Split('.')[0].ToLower() == assetName.ToLower()) {
					// 找到了
					return AssetDatabase.LoadAssetAtPath<T>(asset_path);
				}
			}
			Debug.LogError("Asset Resource \" " + assetName + " \" Didn't Mark a AssetBundle Tag");
			return null;
		}


		public void Release()
		{
			Resources.UnloadUnusedAssets();
		}
	}
}
#endif
