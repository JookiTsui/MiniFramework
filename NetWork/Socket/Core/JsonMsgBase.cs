﻿using System;
using UnityEngine;

public class JsonMsgBase
{
    // 协议名
    public string protoName = "";
	// 编码
    // 解码
    public static JsonMsgBase Decode(string protoName, byte[] bytes, int offset, int count) {

    // 编码协议名（2字节长度+字符串）
    public static byte[] EncodeName(JsonMsgBase msgBase) {

    // 解码协议名（2字节长度+字符串）
    public static string DecodeName(byte[] bytes, int offset, out int count) {
}