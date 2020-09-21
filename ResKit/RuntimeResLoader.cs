using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace MiniFramework
{
	public class RuntimeResLoader : IResLoader
	{
		// Cache AssetBundleManager
		AssetBundleManager _abManager;
		// Current ResLoader Loaded AssetBundles
		List<AssetBundle> _loadedBundles = new List<AssetBundle>();	

		public RuntimeResLoader()
		{
			_abManager = AssetBundleManager.Instance;
			_loadedBundles.Clear();
			Debug.Log("Created a new RuntimeResLoader");
		}

		~RuntimeResLoader()
		{
			Release();
		}

		/// <summary>
		/// Load Asset Sync By Asset Name and AssetBundle Name
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="assetName"></param>
		/// <param name="assetBundleName"></param>
		/// <returns></returns>
		public T LoadSync<T>(string assetName, string assetBundleName) where T : UnityEngine.Object
		{
			//Debug.Log("Called From AssetBundleLoader");
			// 加载所有依赖AB包
			foreach (var abName in _abManager.Manifest.GetAllDependencies(assetBundleName.ToLower())) {
				LoadAssetBundle(abName);
			}
			// 加载资源包本体
			AssetBundle ab = LoadAssetBundle(assetBundleName);
			// 加载对应的资源
			T assetObj = ab.LoadAsset<T>(assetName);
			if (assetObj == null) {
				Debug.LogError("Asset " + assetName + " is not exist in AssetBundles");
				return null;
			} else {
				return assetObj;
			}
		}

		/// <summary>
        /// Releace Current ResLoader Loaded AssetBundle in the memory
        /// </summary>
		public void Release()
		{
			if(_loadedBundles.Count > 0)
			{
				_abManager.DecreaseBundlesReference(_loadedBundles);
				_loadedBundles.Clear();
			}
		}

		private AssetBundle LoadAssetBundle(string assetBundleName)
		{
			foreach(var loadedAB in _loadedBundles)
			{
				if(loadedAB.name == assetBundleName)
				{
					return loadedAB;
				}
			}

			AssetBundle ab = _abManager.SyncLoadBundleByName(assetBundleName);
			_loadedBundles.Add(ab);
			return ab;
		}
	}
}
