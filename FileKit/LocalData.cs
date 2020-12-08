using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;using System.Runtime.Serialization.Formatters.Binary;using UnityEngine;
// 存储到本地的游戏数据
[Serializable]
public class LocalData {
	// 更新版本时添加的新字段
	public string NewField;
}


public class LocalDataMgr
{
	public static LocalData _localData;

    public static void LoadData() {		if (!File.Exists(Application.persistentDataPath + "/LocalData.data")) {			// 说明是第一次进入游戏，先存入本地初始化数据			_localData = new LocalData();			// 初始化数据			SaveData();			return;		}		try {			BinaryFormatter bf = new BinaryFormatter();			using (FileStream fs = File.Open(Application.persistentDataPath + "/LocalData.data", FileMode.Open)) {				fs.Position = 0;				_localData = (LocalData)bf.Deserialize(fs);				// 更新版本时判断是否有新增加的字段				bool isHaveNewFeild = false;				if (_localData.NewField == null) {					_localData.NewField = "I am new field";					isHaveNewFeild = true;				}				if (isHaveNewFeild) {					SaveData();				}			}		} catch (System.Exception ex) {			Debug.Log(ex.Message);		}	}

    public static void SaveData() {		try {			BinaryFormatter bf = new BinaryFormatter();			using (FileStream fs = File.Create(Application.persistentDataPath + "/LocalData.data")) {				bf.Serialize(fs, _localData);			}		} catch (System.Exception ex) {			Debug.LogWarning(ex.Message);		}	}
}
