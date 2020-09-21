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

	[MenuItem(Mark_AssetBundle)]
	static void MarkAssetBundleTag()
	{
		var path = GetSelectedPath();
		if(!string.IsNullOrEmpty(path)) {
			AssetImporter ai = AssetImporter.GetAtPath(path);
			DirectoryInfo dir = new DirectoryInfo(path);
			if (IsMarkedAB(path)) {
				Menu.SetChecked(Mark_AssetBundle, false);
				ai.assetBundleName = null;
			} else {
				Menu.SetChecked(Mark_AssetBundle, true);
				ai.assetBundleName = dir.Name.Replace('.', '_');
			}

			//AssetDatabase.FindAssets()

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
