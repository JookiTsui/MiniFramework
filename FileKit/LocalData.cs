﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
// 存储到本地的游戏数据
[Serializable]
public class LocalData {
	// 更新版本时添加的新字段
	public string NewField;
}


public class LocalDataMgr
{
	public static LocalData _localData;

    public static void LoadData() {

    public static void SaveData() {
}