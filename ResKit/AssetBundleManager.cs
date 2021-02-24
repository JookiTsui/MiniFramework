using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class AssetBundleManager : Singleton<AssetBundleManager>
{#if UNITY_ANDROID && !UNITY_EDITOR
		public static string PlatformName = "Android";
#elif UNITY_IOS && !UNITY_EDITOR
		public static string PlatformName = "iOS";
#elif UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
		public static string PlatformName = "StandaloneWindows";
#elif UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
	public static string PlatformName = "StandaloneOSX";
#endif
	// AssetBundle File PersistenPath Path	public static string BundlePersistentRootPath = Path.Combine(Application.persistentDataPath, "AssetBundles", PlatformName);
	// AssetBundle File streamingAssetsPath Path
	public static string BundleStreamingRootPath = Path.Combine(Application.streamingAssetsPath, "AssetBundles", PlatformName);
	// AssetBundle File Path
	string _bundleRootPath;
	/// <summary>
	/// All Loaded AssetBundles
	/// </summary>
	public Dictionary<AssetBundle, int> _loadedBunddlesRefCount = new Dictionary<AssetBundle, int>();
	/// <summary>
	/// AssetBundleManifest 文件
	/// </summary>
	public AssetBundleManifest Manifest;

	public AssetBundleManager()
	{
		_bundleRootPath = BundlePersistentRootPath;
		if (!File.Exists(Path.Combine(_bundleRootPath, PlatformName)))
		{
			_bundleRootPath = BundleStreamingRootPath;
		}		// 加载根目录下的AssetBundle[与文件夹同名]		AssetBundle assetBundle = AssetBundle.LoadFromFile(Path.Combine(_bundleRootPath, PlatformName));
		// 拿到Manifest
		Manifest = assetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
		// clear the loaded Bundles reference count
		_loadedBunddlesRefCount.Clear();
		_loadedBunddlesRefCount.Add(assetBundle, 1);
	}

	~AssetBundleManager()
	{
		foreach (var ab in _loadedBunddlesRefCount.Keys)
		{
			ab.Unload(true);
		}
	}

	/// <summary>
	/// Get AssetBundle From AssetBundlesRefCounter if Exist, or Load From File by Path
	/// </summary>
	/// <param name="assetBundleName"></param>
	/// <returns></returns>
	public AssetBundle SyncLoadBundleByName(string assetBundleName)
	{
		foreach (var bundle in _loadedBunddlesRefCount.Keys)
		{
			if (bundle.name == assetBundleName)
			{
				IncreaseBundleReference(bundle);
				return bundle;
			}
		}
		AssetBundle newBundle = AssetBundle.LoadFromFile(Path.Combine(_bundleRootPath, assetBundleName));
		_loadedBunddlesRefCount.Add(newBundle, 1);
		Debug.Log("Bundle " + newBundle.name + " has been loaded and begin to count");
		return newBundle;
	}

	/// <summary>
	/// Increase AssetBundle Reference Count 
	/// </summary>
	/// <param name="bundle"></param>
	public void IncreaseBundleReference(AssetBundle bundle)
	{
		if (_loadedBunddlesRefCount.ContainsKey(bundle))
		{
			_loadedBunddlesRefCount[bundle]++;
			Debug.Log("Bundle " + bundle.name + " reference count plus one, now is " + _loadedBunddlesRefCount[bundle]);
		}
	}

	/// <summary>
	/// Decrease AssetBundle Reference Count
	/// </summary>
	/// <param name="toReleaseBundles"></param>
	public void DecreaseBundlesReference(List<AssetBundle> toReleaseBundles)
	{
		foreach (var bundle in toReleaseBundles)
		{
			if (_loadedBunddlesRefCount.ContainsKey(bundle))
			{
				_loadedBunddlesRefCount[bundle]--;
				Debug.Log("Bundle " + bundle.name + " reference count minus one, now is " + _loadedBunddlesRefCount[bundle]);
				if (_loadedBunddlesRefCount[bundle] == 0)
				{
					Debug.Log("Bundle " + bundle.name + " has been released");
					bundle.Unload(true);
					_loadedBunddlesRefCount.Remove(bundle);
				}
			}
		}
	}
}

