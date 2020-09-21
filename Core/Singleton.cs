
public class Singleton<T> where T : new()
{
    private static T _instance;
	/// <summary>
	/// 线程锁
	/// </summary>
    private static readonly object objlock = new object();

    public static T Instance {
        get {
            if(_instance == null) {
				lock (objlock) {
					if(_instance == null) {
						_instance = new T();
					}
				}
			}
			return _instance;
		}
	}
}
