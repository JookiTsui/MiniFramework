using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using System.Linq;
using ProtoBuf;
using System.IO;

public class ProtoNetManager 
{	// 优化后的NetManager	// 定义套接字	static Socket socket;	// 接收缓冲区	static ByteArray readBuff;	// 写入队列	static Queue<ByteArray> writeQueue;	// 是否正在连接	static bool isConnecting = false;	// 是否正在关闭	static bool isClosing = false;	// 协议编码方式, 默认是Protobuf	public static MsgEncodeType msgEncodeType = MsgEncodeType.Protobuf;	// 是否启用心跳	public static bool isUsePing = true;	// 心跳间隔时间	public static int pingInterval = 30;	// 上一次发送PING的时间	static float lastPingTime = 0;	// 上一次收到PONG的时间	static float lastPongTime = 0;	// 事件委托类型	public delegate void EventListener(string err);	//public Action<string> EventListener;	// 事件监听列表	private static Dictionary<NetEvent, EventListener> eventListeners = new Dictionary<NetEvent, EventListener>();	// 消息委托类型	public delegate void MsgListener(IExtensible msgBase);	// 消息监听列表	private static Dictionary<string, MsgListener> msgListeners = new Dictionary<string, MsgListener>();	// 消息列表	private static List<IExtensible> msgList = new List<IExtensible>();	// 消息列表长度	private static int msgCount = 0;	// 每一次Update处理的消息量	private readonly static int MAX_MESSAGE_FIRE = 10;	// 增加事件监听	public static void AddListener(NetEvent netEvent, EventListener callback) {
		if (eventListeners.ContainsKey(netEvent)) {
			eventListeners[netEvent] += callback;
		} else {
			eventListeners.Add(netEvent, callback);
		}
	}	// 移除事件监听	public static void RemoveListener(NetEvent netEvent, EventListener callback) {
		if (eventListeners.ContainsKey(netEvent)) {
			eventListeners[netEvent] -= callback;			// 删除			if (eventListeners[netEvent] == null) {
				eventListeners.Remove(netEvent);
			}
		}
	}	// 分发事件消息	private static void FireEvent(NetEvent netEvent, string err) {
		if (eventListeners.ContainsKey(netEvent)) {
			eventListeners[netEvent].Invoke(err);
		}
	}	// 添加消息监听	public static void AddMsgListener(string msgName, MsgListener listener) {		// 添加		if (msgListeners.ContainsKey(msgName)) {
			msgListeners[msgName] += listener;
		} else {
			msgListeners.Add(msgName, listener);
		}
	}	// 移除消息监听	public static void RemoveMsgListener(string msgName, MsgListener listener) {
		if (msgListeners.ContainsKey(msgName)) {
			msgListeners[msgName] -= listener;
			if (msgListeners[msgName] == null) {
				msgListeners.Remove(msgName);
			}
		}
	}	// 分发消息	private static void FireMsg(string msgName, IExtensible msgBase) {
		Debug.Log("分发消息：" + msgName);
		if (msgListeners.ContainsKey(msgName)) {
			msgListeners[msgName].Invoke(msgBase);
		}
	}	// 连接服务器	public static void Connect(string ip, int port) {		// 状态判断		if (socket != null && socket.Connected) {
			Debug.Log("Connect fail, already connected! ");
			return;
		}
		if (isConnecting) {
			Debug.Log("Connect fail, isConnecting");
			return;
		}		// 初始化成员		InitState();		// 参数设置		socket.NoDelay = true;		// Connect		isConnecting = true;
		socket.BeginConnect(ip, port, ConnectCallback, socket);
	}	// 初始化状态	private static void InitState() {		// Socket		socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);		// 接收缓冲区		readBuff = new ByteArray();		// 写入队列		writeQueue = new Queue<ByteArray>();		// 是否正在连接		isConnecting = false;		// 是否正在关闭		isClosing = false;		// 消息列表		msgList = new List<IExtensible>();		// 消息列表长度		msgCount = 0;		// 上一次发送PING的时间		lastPingTime = Time.time;		// 上一次收到PONG的时间		lastPongTime = Time.time;		// 监听PONG协议		if (!msgListeners.ContainsKey("MsgPong")) {
			AddMsgListener("MsgPong", OnMsgPong);
		}
	}	// 连接回调	private static void ConnectCallback(IAsyncResult ar) {
		try {
			Socket socket = (Socket)ar.AsyncState;
			socket.EndConnect(ar);
			Debug.Log("Socket Connect Succ ");
			FireEvent(NetEvent.ConnectSucc, "");
			isConnecting = false;			// 开始接收			socket.BeginReceive(readBuff.bytes, readBuff.writeIdx, readBuff.remain, 0, RecieveCallback, socket);
		} catch (SocketException ex) {
			Debug.Log("Socket Connect fail " + ex.ToString());
			FireEvent(NetEvent.ConnectFail, ex.ToString());
			isConnecting = false;
		}
	}	// 关闭连接	public static void Close() {		// 状态判断		if (socket == null || !socket.Connected) {
			return;
		}
		if (isConnecting) {
			return;
		}		// 还有数据在发送		if (writeQueue.Count > 0) {
			isClosing = true;
		}		// 没有数据在发送		else {
			socket.Close();
			FireEvent(NetEvent.Close, "");
		}
	}	// ProtoBuf 发送数据	public static void Send(IExtensible msg) {		// 状态判断		if (socket == null || !socket.Connected) {			return;		}		if (isConnecting || isClosing) {			return;		}		// 数据编码		byte[] nameBytes = ProtoMsgBase.EncodeName(msg);		byte[] bodyBytes = ProtoMsgBase.Encode(msg);		int len = nameBytes.Length + bodyBytes.Length;		byte[] sendBytes = new byte[2 + len];		// 组装长度		sendBytes[0] = (byte)(len % 256);		sendBytes[1] = (byte)(len / 256);		// 组装名字		Array.Copy(nameBytes, 0, sendBytes, 2, nameBytes.Length);		// 组装消息体		Array.Copy(bodyBytes, 0, sendBytes, 2 + nameBytes.Length, bodyBytes.Length);		// 写入队列		ByteArray ba = new ByteArray(sendBytes);		int count = 0; // WriteQueue的长度		lock (writeQueue) {			writeQueue.Enqueue(ba);			count = writeQueue.Count;		}		// send		if (count == 1) {			socket.BeginSend(sendBytes, 0, sendBytes.Length, 0, SendCallback, socket);		}
	}	// 发送回调	private static void SendCallback(IAsyncResult ar) {		// 获取state、 EndSend的处理		Socket socket = (Socket)ar.AsyncState;		// 判断状态		if (socket == null || !socket.Connected) {
			return;
		}		// EndSend		int count = socket.EndSend(ar);		// 获取写入队列的第一条数据		ByteArray ba;
		lock (writeQueue) {
			ba = writeQueue.First();
		}		// 完整发送		ba.readIdx += count;
		if (ba.length == 0) {
			lock (writeQueue) {
				writeQueue.Dequeue();
				ba = writeQueue.First();
			}
		}		// 继续发送		if (ba != null) {
			socket.BeginSend(ba.bytes, ba.readIdx, ba.length, 0, SendCallback, socket);
		}		// 正在关闭		else if (isClosing) {
			socket.Close();
		}
	}	// 接收回调	private static void RecieveCallback(IAsyncResult ar) {
		try {
			Socket socket = (Socket)ar.AsyncState;			// 获取接收数据长度			int count = socket.EndReceive(ar);
			if (count == 0) {
				Close();
				return;
			}
			readBuff.writeIdx += count;			// 处理二进制消息			OnRecieveData();			// 继续接收数据			if (readBuff.remain < 8) {
				readBuff.MoveBytes();
				readBuff.ReSize(readBuff.length * 2);
			}
			socket.BeginReceive(readBuff.bytes, readBuff.writeIdx, readBuff.remain, 0, RecieveCallback, socket);
		} catch (SocketException ex) {
			Debug.Log("Socket Recieve Fail " + ex.ToString());
		}
	}	//  接收数据处理	private static void OnRecieveData() {		// 消息长度		if (readBuff.length <= 2) {
			return;
		}		// 获取消息体长度		int readIdx = readBuff.readIdx;
		byte[] bytes = readBuff.bytes;
		Int16 bodyLength = (Int16)((bytes[readIdx + 1] << 8) | bytes[readIdx]);
		if (readBuff.length < bodyLength) {
			return;
		}
		readBuff.readIdx += 2;		// 解析协议名		int nameCount = 0;
		string protoName = ProtoMsgBase.DecodeName(readBuff.bytes, readBuff.readIdx, out nameCount);
		if (protoName == "") {
			Debug.Log("OnRecieveData MsgBase.DecodeName Fail");
			return;
		}
		readBuff.readIdx += nameCount;		// 解析协议体		int bodyCount = bodyLength - nameCount;
		IExtensible msgBase = ProtoMsgBase.Decode(protoName, readBuff.bytes, readBuff.readIdx, bodyCount);
		readBuff.readIdx += bodyCount;
		readBuff.CheckAndMoveBytes();		// 添加到消息队列		lock (msgList) {
			msgList.Add(msgBase);
		}
		msgCount++;		// 继续读取消息		if (readBuff.length > 2) {
			OnRecieveData();
		}
	}	//Update, 需要外部驱动	public static void Update() {
		MsgUpdate();
		PingUpdate();
	}	// 分发消息	private static void MsgUpdate() {		// 初步判断， 提升效率		if (msgCount == 0) {
			return;
		}		// 重复处理消息		for (int i = 0; i < MAX_MESSAGE_FIRE; i++) {			// 获取第一条消息			IExtensible msgBase = null;
			lock (msgList) {
				if (msgList.Count > 0) {
					msgBase = msgList[0];
					msgList.RemoveAt(0);
					msgCount--;
				}
			}			// 分发消息			if (msgBase != null) {
				FireMsg(msgBase.ToString(), msgBase);
			}			// 没有消息了			else {
				break;
			}
		}
	}	// 发送PING协议	private static void PingUpdate() {		// 是否启用		if (!isUsePing) {
			return;
		}		// 发送PING		if (Time.time - lastPingTime > pingInterval) {			//MsgPing msgPing = new MsgPing();			//Send(msgPing);			lastPingTime = Time.time;
		}		// 检测PONG时间		if (Time.time - lastPongTime > pingInterval * 4) {
			Close();
		}
	}	// 监听PONG协议	private static void OnMsgPong(IExtensible msgBase) {
		lastPongTime = Time.time;
	}
}
