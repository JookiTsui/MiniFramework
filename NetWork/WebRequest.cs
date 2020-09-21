using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class WebRequest : MonoSingleton<WebRequest>
{

	/// <summary>
	/// 对外方法，从指定地址获取文本文件
	/// </summary>
	/// <param name="url"></param>
	/// <param name="OnSuccessCallBack"></param>
	public void Send(string url, Action<string> OnSuccessCallBack)
	{
		StartCoroutine(SendWebRequest(url , OnSuccessCallBack));
	}

	/// <summary>
	/// 从指定地址获取文本文件
	/// </summary>
	/// <param name="url"></param>
	/// <param name="OnSuccessCallBack"></param>
	/// <returns></returns>
	private IEnumerator SendWebRequest(string url, Action<string> OnSuccessCallBack)
	{
		var request = UnityWebRequest.Get(url);
		yield return request.SendWebRequest();
		if (CheckRequestError(request)) {
			while (!request.downloadHandler.isDone) {
				yield return new WaitForSeconds(0.1f);
			}
			string responseTex = request.downloadHandler.text;

			OnSuccessCallBack?.Invoke(responseTex);
		}
	}

	public void LoadFileFromLocal(string filePath, Action<string> OnSuccessCallBack) {
		StartCoroutine(LoaalLoclFile(filePath, OnSuccessCallBack));
	}

	private IEnumerator LoaalLoclFile(string filePath, Action<string> OnSuccessCallBack)
	{
		Uri uri = new Uri(filePath);
		var request = UnityWebRequest.Get(uri.AbsoluteUri);
		yield return request.SendWebRequest();
		if (CheckRequestError(request)) {
			while (!request.downloadHandler.isDone) {
				yield return new WaitForSeconds(0.1f);
			}
			string responseTex = request.downloadHandler.text;

			OnSuccessCallBack?.Invoke(responseTex);
		}
	}

	/// <summary>
	/// 对外方法， 从服务器获取非文本文件
	/// </summary>
	/// <param name="url"></param>
	/// <param name="OnSuccessCallBack"></param>
	public void Send(string url, Action<byte[]> OnSuccessCallBack)
	{
		StartCoroutine(SendWebRequest(url, OnSuccessCallBack));
	}
	/// <summary>
	/// 从服务器获取非文本文件
	/// </summary>
	/// <param name="url"></param>
	/// <param name="OnSuccessCallBack"></param>
	/// <returns></returns>
	private IEnumerator SendWebRequest(string url, Action<byte[]> OnSuccessCallBack)
	{
		var request = UnityWebRequest.Get(url);
		yield return request.SendWebRequest();
		if (CheckRequestError(request)) {
			while (!request.downloadHandler.isDone) {
				yield return new WaitForSeconds(0.1f);
			}
			byte[] data = request.downloadHandler.data;
			OnSuccessCallBack?.Invoke(data);
		}
	}

	public void GetAssetBundle(string url , Action<AssetBundle> OnSuccessCallBack)
	{
		StartCoroutine(SendWebRequest(url, OnSuccessCallBack));
	}

	private IEnumerator SendWebRequest(string url, Action<AssetBundle> OnSuccessCallBack)
	{
		var request = UnityWebRequestAssetBundle.GetAssetBundle(url);
		yield return request.SendWebRequest();
		if (CheckRequestError(request)) {
			while (!request.downloadHandler.isDone) {
				yield return new WaitForSeconds(0.1f);
			}
			AssetBundle assetBundle = DownloadHandlerAssetBundle.GetContent(request);
			if(assetBundle != null) {
				OnSuccessCallBack?.Invoke(assetBundle);
			} else {
				Debug.LogError(url + ", Asset Not Fount on Server");
			}
		}
	}

	/// <summary>
	/// 检查网络请求是否出错
	/// </summary>
	/// <param name="request"></param>
	/// <returns></returns>
	bool CheckRequestError(UnityWebRequest request)
	{
		if (request.isHttpError || request.isNetworkError) {
			Debug.LogError("网络错误 " + request.error);
			return false;
		}
		return true;
	}
}
