using System;
namespace MiniFramework
{
	public interface IResLoader
	{
		/// <summary>
		/// 同步加载资源, resName为资源在Asset目录下的文件名，不加后缀
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="assetName">填写资源在Asset目录下的文件名，不加后缀</param>
		/// <returns></returns>
		T LoadSync<T>(string assetName, string assetBundleName) where T : UnityEngine.Object;

		/// <summary>
		/// 卸载资源
		/// </summary>
		/// <param name="obj"></param>
		void Release();
	}
}
