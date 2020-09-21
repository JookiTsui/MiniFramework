#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
#endif


namespace MiniFramework {
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

		public T LoadSync<T>(string assetName, string assetBundleName) where T : UnityEngine.Object
		{
			return _resLoader.LoadSync<T>(assetName, assetBundleName);
		}

		public void Release()
		{
			_resLoader.Release();
		}
	}
}


