using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
	/// <summary>
	/// 线程锁
	/// </summary>
	private static readonly object objlock = new object();

	/// <summary>
	/// 是否是全局单例
	/// </summary>
	protected static bool isGlobal = true;

    public static T Instance {
		get {
			if(_instance == null) {
				lock (objlock) {
					if(_instance == null) {
						_instance = FindObjectOfType<T>();
						if(_instance == null) {
							GameObject singletonObj = new GameObject();
							singletonObj.name = "(singleton)" + typeof(T);
							_instance = singletonObj.AddComponent<T>();
							if (isGlobal) {
								DontDestroyOnLoad(singletonObj);
							}
						}
					}
				}
			}
			return _instance;
		}
	}
}
