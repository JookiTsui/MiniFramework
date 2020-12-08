using UnityEngine;
using UnityEditor;
using System.IO;

[InitializeOnLoad]
public class ResKitMenu : ScriptableObject
{
	private const string Mark_AssetBundle = "Assets/Mark AssetBundle";

	static ResKitMenu() {
		Selection.selectionChanged = OnSelectionChanged;
	}

	static void OnSelectionChanged() {
		var path = GetSelectedPath();
		if (!string.IsNullOrEmpty(path)) {
			Menu.SetChecked(Mark_AssetBundle, IsMarkedAB(path));
		}
	}

	// 设置菜单选项在Assets下，如此就会出现在右键菜单中
	[MenuItem(Mark_AssetBundle)]
	static void MarkAssetBundleTag()
	{
		// 获取选中资源的路径
		var path = GetSelectedPath();
		if(!string.IsNullOrEmpty(path)) {
			// 根据资源路径获取资源信息，已经打过标签的资源都会记录在在AssetImporter中
			AssetImporter ai = AssetImporter.GetAtPath(path);
			DirectoryInfo dir = new DirectoryInfo(path);
			// 判断资源是否已经被打过标签，打过标签再次点击则取消标记
			if (IsMarkedAB(path)) {
				Menu.SetChecked(Mark_AssetBundle, false);
				ai.assetBundleName = null;
			} else {
				Menu.SetChecked(Mark_AssetBundle, true);
				// 标签名的规范为 文件名 + 下划线 + 后缀
				ai.assetBundleName = dir.Name.Replace('.', '_');
			}
			// 自动移除没有使用的标签
			AssetDatabase.RemoveUnusedAssetBundleNames();
		}
	}

	static bool IsMarkedAB(string path)
	{
		var ai = AssetImporter.GetAtPath(path);
		var dirInfo = new DirectoryInfo(path);
		return string.Equals(ai.assetBundleName, dirInfo.Name.Replace('.', '_').ToLower());
	}

	static string GetSelectedPath()
	{

		string path = "";
		string[] guids = Selection.assetGUIDs;
		if (guids.Length == 1 && !string.IsNullOrEmpty(guids[0])) {
			path = AssetDatabase.GUIDToAssetPath(guids[0]);
		}
		return path;
	}
}
