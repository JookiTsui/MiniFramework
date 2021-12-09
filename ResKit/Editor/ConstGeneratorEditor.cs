using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text;
using System.IO;

public class ConstGeneratorEditor
{
    [MenuItem("Tools/ConstGenerator")]
    static void Generate()
    {
        StringBuilder sbtemp = new StringBuilder();
        sbtemp.AppendLine("public class _Const {");

        //Layer 常量
        for(int i=0; i<32; i++)
        {
            var layerName = LayerMask.LayerToName(i);
            layerName = layerName
                .Replace(" ", "_")
                .Replace("&", "_")
                .Replace(".", "_")
                .Replace("/", "_")
                .Replace(",", "_")
                .Replace(";", "_")
                .Replace("-", "_");
            if (!string.IsNullOrEmpty(layerName))
            {
                sbtemp.AppendFormat("\tpublic const int LAYER_{0} = {1};\n", layerName.ToUpper(), i);
            }
        }

        //Tag 常量
        //内置的先写死
        sbtemp.AppendLine("\tpublic const string " + "Tag_Untagged".ToUpper() + "=" + "\"Untagged\";");
        sbtemp.AppendLine("\tpublic const string " + "Tag_Respawn".ToUpper() + "=" + "\"Respawn\";");
        sbtemp.AppendLine("\tpublic const string " + "Tag_Finish".ToUpper() + "=" + "\"Finish\";");
        sbtemp.AppendLine("\tpublic const string " + "Tag_EditorOnly".ToUpper() + "=" + "\"EditorOnly\";");
        sbtemp.AppendLine("\tpublic const string " + "Tag_MainCamera".ToUpper() + "=" + "\"MainCamera\";");
        sbtemp.AppendLine("\tpublic const string " + "Tag_Player".ToUpper() + "=" + "\"Player\";");
        sbtemp.AppendLine("\tpublic const string " + "Tag_GameController".ToUpper() + "=" + "\"GameController\";");

        //拿到自定义Tag
        //从 ProjectSettings 中拿到 TagManager.asset
        Object[] asset = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset");
        if(asset != null && asset.Length > 0)
        {
            for(int i=0; i<asset.Length; i++)
            {
                // 创建序列化对象
                SerializedObject so = new SerializedObject(asset[i]);
                // 读取具体对象
                SerializedProperty tags = so.FindProperty("tags");
                for(int j=0; j<tags.arraySize; j++)
                {
                    string tagName = tags.GetArrayElementAtIndex(j).stringValue;
                    sbtemp.AppendFormat("\tpublic const string TAG_{0}=\"{1}\";\n", tagName.ToUpper(), tagName);
                }
            }
        }

        sbtemp.Append("}");
        // 写入硬盘
        File.WriteAllText("Assets/Scripts/GeneratedConst.cs", sbtemp.ToString());
        // 通知编辑器刷新
        AssetDatabase.Refresh();
    }
}
