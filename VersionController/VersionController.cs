using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace MiniFramework
{
	public class VersionInfo
	{
		/// <summary>
		/// APP版本号, 样式 0.0
		/// </summary>
		public string VersionCode;
		/// <summary>
		/// 资源版本号， 样式0.0, 第1个位数字标记是否需要更新全部资源，第2位数字标记更新部分资源
		/// </summary>
		public string ResourceVersionCode;
		/// <summary>
		/// 服务器存放VersionConfig.json文件的路径
		/// </summary>
		public string ServerVersionConfigFilePath;
		/// <summary>
        /// 服务器存放AssetBundles的根目录
        /// </summary>
		public string ServerBundlesRootPath;
	}

	public class VersionController : Singleton<VersionController>
	{
		// 客户端版本信息文件路径
		private readonly string LocalVersionInfoPath = Path.Combine(Application.persistentDataPath, "VersionConfig.json");
		// 客户端AssetBundle存放的目录
		private readonly string localAssetBundleRootPath = AssetBundleManager.BundlePersistentRootPath;
		// 服务器版本信息
		private VersionInfo _serverVersionInfo = new VersionInfo();
		// 客户端版本信息
		private VersionInfo _localVersionInfo = new VersionInfo();
		// 待下载AssetBundle列表
		private List<string> _toDownloadABsNameList = new List<string>();
		// 下载进度
		public float DownLoadProgress = 0.0f;
		// 服务器的AssetBundleManifest
		AssetBundleManifest _serverManifest;
		// 本地的AssetBundleManifest
		AssetBundleManifest _localManifest;

		/// <summary>
		/// 游戏启动时检查是否需要更新资源
		/// </summary>
		public void OnAppLoadCheckUpdate()
		{
			Debug.Log("persistentDataPath : " + Application.persistentDataPath);
			if (!File.Exists(LocalVersionInfoPath)) {				//Directory.Move(Path.Combine(Application.streamingAssetsPath, "VersionConfig.json"), LocalVersionInfoPath);				//Directory.Move(Path.Combine(Application.streamingAssetsPath, "VersionConfig.json.meta"), LocalVersionInfoPath);				string filePath = Path.Combine(Application.streamingAssetsPath, "VersionConfig.json");				WebRequest.Instance.LoadFileFromLocal(filePath, (bytes) =>				{					// 获取文件名					File.WriteAllBytes(LocalVersionInfoPath, bytes);				});
			}

			CheckOrDownloadRes();
		}

		/// <summary>
		/// 检查是否需要更新APP版本或资源包
		/// </summary>
		private void CheckOrDownloadRes()
		{
			if(WebRequest.Instance == null) {
				Debug.Log("WebRequest Instance is NULL");
				return;
			}
			// 获取客户端版本信息
			WebRequest.Instance.LoadFileFromLocal(LocalVersionInfoPath, localInfo => {
				_localVersionInfo = JsonUtility.FromJson<VersionInfo>(localInfo);
				// 获取服务器端版本信息
				Debug.Log("服务器配置文件URL: " + _localVersionInfo.ServerVersionConfigFilePath);
				WebRequest.Instance.Send(_localVersionInfo.ServerVersionConfigFilePath, serverInfo => {
					_serverVersionInfo = JsonUtility.FromJson<VersionInfo>(serverInfo);
					CompareVersion();
				});
			});
			
			
		}

		private void CompareVersion()
		{
			// 检查APP版本号信息
			float serAPPVer = float.Parse(_serverVersionInfo.VersionCode);
			float locAPPVer = float.Parse(_localVersionInfo.VersionCode);
			if (serAPPVer > locAPPVer) {
				// 直接更新版本，不需要更新资源包
				// TODO APP 的版本信息暂不考虑
				Debug.Log("APP有新版本");
				return;
			}

			// 检查资源大版本号信息
			int serResV_1 = int.Parse(_serverVersionInfo.ResourceVersionCode.Split('.')[0]);
			int locResV_1 = int.Parse(_localVersionInfo.ResourceVersionCode.Split('.')[0]);
			if(serResV_1 > locResV_1) {
				// 更新所有资源包
				Debug.Log("更新所有资源包");
				UpdateAllResources();
				return;
			}

			// 检查资源小版本号信息
			int serResV_2 = int.Parse(_serverVersionInfo.ResourceVersionCode.Split('.')[1]);
			int locResV_2 = int.Parse(_localVersionInfo.ResourceVersionCode.Split('.')[1]);
			if (serResV_2 > locResV_2) {
				// 增量更新资源包，只更新修改过的资源包,
				// 先看看目录下是否有AssetBundles，如果没有说明是第一次更新，先把streamingPath路径下的AssetBundles拷贝到PersistentPath下
				MoveAssetBundlesFromStreamToPersit();
				Debug.Log("增量更新资源包");
				DeltaUpdateResources();
				return;
			}

			Debug.Log("资源已经是最新的了，不需要下载");
		}

		private void MoveAssetBundlesFromStreamToPersit()
		{
			// 如果PersistentPath路径下面还没有相关文件，则从StreamingPath复制过去
			if (!Directory.Exists(localAssetBundleRootPath)) {
				try {
					Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, "AssetBundles"));					// 移动设备上，StreamingAssetPath不支持Directory操作#if UNITY_EDITOR
					//Directory.Move(AssetBundleManager.BundleStreamingRootPath, localAssetBundleRootPath);					// 获取streamingAssetsPath/AssetBundles下的所有文件路径					string[] filePaths = Directory.GetFiles(AssetBundleManager.BundleStreamingRootPath);					for(int i=0; i<filePaths.Length; i++)					{						int fileIndex = i;						Debug.Log(filePaths[fileIndex]);						WebRequest.Instance.LoadFileFromLocal(filePaths[fileIndex], (bytes) =>						{							// 获取文件名							string fileName = filePaths[fileIndex].Substring(filePaths[fileIndex].LastIndexOf('/') + 1);							File.WriteAllBytes(Path.Combine(localAssetBundleRootPath, fileName), bytes);						});					}#else
					// 移动设备上，StreamingAssetPath不支持Directory操作
					
#endif
				} catch (Exception ex) {
					throw new Exception("创建目录失败" + ex.Message);
				}
				
			}
		}

		private void UpdateAllResources()
		{			// 先删除根目录下的所有文件			if (Directory.Exists(localAssetBundleRootPath))			{				Directory.Delete(localAssetBundleRootPath, true);			}
			// 再重新生成一个AssetBundle的根目录
			try {
				Directory.CreateDirectory(localAssetBundleRootPath);
			} catch (Exception ex) {
				throw new Exception("创建目录失败" + ex.Message);
			}
			DownLoadAssetBundles();
		}

		// 只下载更新了的资源，未更新的资源不必重新下载
		private void DeltaUpdateResources()
		{
			DownLoadAssetBundles();
		}

		private void DownLoadAssetBundles()
		{
			// 先比较服务器和本地的Manifest所包含的AssetBundle的区别
			string platformName = AssetBundleManager.PlatformName;
			// 下载服务器的AssetBundleManifest
			WebRequest.Instance.GetAssetBundle(Path.Combine(_localVersionInfo.ServerBundlesRootPath, platformName, platformName), (serverAssetBundle) => {
				_serverManifest = serverAssetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
				serverAssetBundle.Unload(false);
				// 下载本地的AssetBundleManifest
				string localManifestPath = Path.Combine(localAssetBundleRootPath, platformName);
				if (File.Exists(localManifestPath)) {
					WebRequest.Instance.GetAssetBundle(new Uri(localManifestPath).AbsoluteUri, (localAssetBundle) => {
						_localManifest = localAssetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
						localAssetBundle.Unload(false);
						CompareManifestDiff();
					});
				}
				// 服务器和本地的Manifest文件均加载完毕，进行比较
			});
		}

		private void CompareManifestDiff()
		{			// 所有的AssetBundle			// 1. 凡是服务器有、本地没有的都加入待下载列表			// 2. 凡是服务器中AB的hash和本地不一致的，也加入待下载列表
			foreach(var serverAB in _serverManifest.GetAllAssetBundles()) {
				bool needToDownload = true;
				// 检查本地manifest是否存在, 不存在下载服务器所有的AssetBundle
				if (_localManifest != null) {
					foreach (var localAB in _localManifest.GetAllAssetBundles()) {
						if (serverAB == localAB && _serverManifest.GetAssetBundleHash(serverAB) == _localManifest.GetAssetBundleHash(localAB)) {
							needToDownload = false;
						}
					}
				}
				// 需要更新的AssetBundle，加入待下载列表
				if (needToDownload) {
					_toDownloadABsNameList.Add(serverAB);
					_toDownloadABsNameList.Add(serverAB + ".manifest");
				}
			}

			if(_toDownloadABsNameList.Count != 0) {
				// 有资源要更更新
				// 把主AssetBundle也加入下载列表，主AssetBundle名与运行平台同名
				_toDownloadABsNameList.Add(AssetBundleManager.PlatformName);
				_toDownloadABsNameList.Add(AssetBundleManager.PlatformName + ".manifest");
				SendRequestToDownLoadAB();
			} else {				// 没有资源要更新，但是也更新下版本信息文件				Debug.Log("资源没有变化，不需要更新");
				WebRequest.Instance.Send(_localVersionInfo.ServerVersionConfigFilePath, (byte[] fileData) => {
					SaveFileToLocal(LocalVersionInfoPath, fileData);
					Debug.Log("版本信息文件下载完成");
				});
			}
		}

		private void SendRequestToDownLoadAB()
		{
			Debug.Log("一共需要更新 " + _toDownloadABsNameList.Count + " 个资源");
			int alreadyDownloadCount = 0;
			foreach(var abName in _toDownloadABsNameList) {
				WebRequest.Instance.Send(Path.Combine(_localVersionInfo.ServerBundlesRootPath, AssetBundleManager.PlatformName, abName), (byte[] data) => {
					// 将下载好的AssetBundle存入本地
					SaveFileToLocal(Path.Combine(localAssetBundleRootPath, abName), data);
					alreadyDownloadCount++;
					DownLoadProgress = (float)alreadyDownloadCount / _toDownloadABsNameList.Count;
					Debug.Log(abName + " Alreay Downloaded.");
					Debug.Log("Download Progress : " + Math.Round(DownLoadProgress * 100, 0) + "%");
					if(alreadyDownloadCount == _toDownloadABsNameList.Count) {
						// 资源全部下载完成，再将 VersionConig.json 文件替换
						WebRequest.Instance.Send(_localVersionInfo.ServerVersionConfigFilePath, (byte[] fileData) => {
							SaveFileToLocal(LocalVersionInfoPath, fileData);
							Debug.Log("版本信息文件下载完成");
						});
					}
				});
			}
			
		}

		void SaveFileToLocal(string Path, byte[] data)
		{
			File.WriteAllBytes(Path, data);
			//FileStream fs = File.Create(Path);
			//fs.Write(data, 0, data.Length);
			//fs.Flush();
			//fs.Close();
			//fs.Dispose();
		}
	}
}

