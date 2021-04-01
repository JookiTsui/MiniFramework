﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProtoBuf;
using System.IO;
{
    // 协议名
    public string protoName = "";
    // 解码
    public static IExtensible Decode(string protoName, byte[] bytes, int offset, int count) {
        using (var memory = new MemoryStream(bytes, offset, count)) {

    // 编码协议名（2字节长度+字符串）
    public static byte[] EncodeName(IExtensible msgBase) {
        // 名字bytes和长度
        byte[] nameBytes = System.Text.Encoding.UTF8.GetBytes(msgBase.ToString());
        Int16 len = (Int16)nameBytes.Length;
        // 申请bytes数值
        byte[] bytes = new byte[2 + len];
        // 组装2字节的长度信息
        bytes[0] = (byte)(len % 256);
        bytes[1] = (byte)(len / 256);
        // 组装名字bytes
        Array.Copy(nameBytes, 0, bytes, 2, len);
        return bytes;

    // 解码协议名（2字节长度+字符串）
    public static string DecodeName(byte[] bytes, int offset, out int count) {
        count = 0;
        // 必须大于2字节
        if (offset + 2 > bytes.Length) {
            return "";
        // 读取长度
        Int16 len = (Int16)((bytes[offset + 1] << 8) | bytes[offset]);
        //Int16 len = BitConverter.ToInt16(bytes, offset);
        // 长度必须足够
        if (offset + 2 + len > bytes.Length) {
            return "";
        // 解析
        count = 2 + len;
        string name = System.Text.Encoding.UTF8.GetString(bytes, offset + 2, len);
        return name;
}