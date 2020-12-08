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
			//Debug.Log("Created a new EditorResLoader");
		}

		~EditorResLoader()
		{
			Release();
		}		public T LoadSync<T>(string assetName, string assetBundleName) where T : UnityEngine.Object
		{
			// 遍历AssetBundle中所有资源的路径
			string[] assetPaths = AssetDatabase.GetAssetPathsFromAssetBundle(assetBundleName.ToLower());
			foreach (var asset_path in assetPaths) {
				// 获取对应路径的资源类型及名称
				DirectoryInfo dirInfo = new DirectoryInfo(asset_path);				// 当前默认不会出现资源名重复的问题，包括不同类型的文件也不能重复， 否则有可能导致最后加载出来的资源有误
				if (dirInfo.Name.Replace(dirInfo.Extension, "").ToLower() == assetName.ToLower()) {
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

		public void UnLoadAssetBundle(string abName) {			Debug.Log("UnLoadAssetBundle 被调用了");			Release();		}
	}
}
#endif
