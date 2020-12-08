#if UNITY_EDITOR
using UnityEditor;#endif
using UnityEngine;namespace MiniFramework {
	public class ResLoader : IResLoader
	{
		IResLoader _resLoader;
		public ResLoader()
		{
#if UNITY_EDITOR
			if(Menu.GetChecked("MiniTools/模拟真机资源加载")) {
				_resLoader = new RuntimeResLoader();
			} else {
				_resLoader = new EditorResLoader();
			}
#else
			_resLoader = new RuntimeResLoader();
#endif
		}

		public TextAsset LoadLuaScript(string moudleName, string assetBundleName) {			if (!moudleName.Contains(".lua")) {				moudleName += ".lua";			}			assetBundleName = assetBundleName.ToLower();			return LoadSync<TextAsset>(moudleName, assetBundleName);		}

		public T LoadSync<T>(string assetName, string assetBundleName) where T : UnityEngine.Object
		{
			assetBundleName = assetBundleName.ToLower();
			return _resLoader.LoadSync<T>(assetName, assetBundleName);
		}

		public void Release()
		{
			_resLoader.Release();
		}

		public void UnLoadAssetBundle(string abName) {			abName = abName.ToLower();			_resLoader.UnLoadAssetBundle(abName);		}		/* 以下方法初衷是为了方便在Lua中调用 */		public GameObject LoadPrefabSync(string assetName, string assetBundleName) {			return LoadSync<GameObject>(assetName, assetBundleName);		}

		public Sprite LoadSpriteSync(string assetName, string assetBundleName) {			return LoadSync<Sprite>(assetName, assetBundleName);		}
	}
}


