﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using System.Linq;
using ProtoBuf;
using System.IO;

public class ProtoNetManager 
{
		if (eventListeners.ContainsKey(netEvent)) {
			eventListeners[netEvent] += callback;
		} else {
			eventListeners.Add(netEvent, callback);
		}
	}
		if (eventListeners.ContainsKey(netEvent)) {
			eventListeners[netEvent] -= callback;
				eventListeners.Remove(netEvent);
			}
		}
	}
		if (eventListeners.ContainsKey(netEvent)) {
			eventListeners[netEvent].Invoke(err);
		}
	}
			msgListeners[msgName] += listener;
		} else {
			msgListeners.Add(msgName, listener);
		}
	}
		if (msgListeners.ContainsKey(msgName)) {
			msgListeners[msgName] -= listener;
			if (msgListeners[msgName] == null) {
				msgListeners.Remove(msgName);
			}
		}
	}
		Debug.Log("分发消息：" + msgName);
		if (msgListeners.ContainsKey(msgName)) {
			msgListeners[msgName].Invoke(msgBase);
		}
	}
			Debug.Log("Connect fail, already connected! ");
			return;
		}
		if (isConnecting) {
			Debug.Log("Connect fail, isConnecting");
			return;
		}
		socket.BeginConnect(ip, port, ConnectCallback, socket);
	}
			AddMsgListener("MsgPong", OnMsgPong);
		}
	}
		try {
			Socket socket = (Socket)ar.AsyncState;
			socket.EndConnect(ar);
			Debug.Log("Socket Connect Succ ");
			FireEvent(NetEvent.ConnectSucc, "");
			isConnecting = false;
		} catch (SocketException ex) {
			Debug.Log("Socket Connect fail " + ex.ToString());
			FireEvent(NetEvent.ConnectFail, ex.ToString());
			isConnecting = false;
		}
	}
			return;
		}
		if (isConnecting) {
			return;
		}
			isClosing = true;
		}
			socket.Close();
			FireEvent(NetEvent.Close, "");
		}
	}
	}
			return;
		}
		lock (writeQueue) {
			ba = writeQueue.First();
		}
		if (ba.length == 0) {
			lock (writeQueue) {
				writeQueue.Dequeue();
				ba = writeQueue.First();
			}
		}
			socket.BeginSend(ba.bytes, ba.readIdx, ba.length, 0, SendCallback, socket);
		}
			socket.Close();
		}
	}
		try {
			Socket socket = (Socket)ar.AsyncState;
			if (count == 0) {
				Close();
				return;
			}
			readBuff.writeIdx += count;
				readBuff.MoveBytes();
				readBuff.ReSize(readBuff.length * 2);
			}
			socket.BeginReceive(readBuff.bytes, readBuff.writeIdx, readBuff.remain, 0, RecieveCallback, socket);
		} catch (SocketException ex) {
			Debug.Log("Socket Recieve Fail " + ex.ToString());
		}
	}
			return;
		}
		byte[] bytes = readBuff.bytes;
		Int16 bodyLength = (Int16)((bytes[readIdx + 1] << 8) | bytes[readIdx]);
		if (readBuff.length < bodyLength) {
			return;
		}
		readBuff.readIdx += 2;
		string protoName = ProtoMsgBase.DecodeName(readBuff.bytes, readBuff.readIdx, out nameCount);
		if (protoName == "") {
			Debug.Log("OnRecieveData MsgBase.DecodeName Fail");
			return;
		}
		readBuff.readIdx += nameCount;
		IExtensible msgBase = ProtoMsgBase.Decode(protoName, readBuff.bytes, readBuff.readIdx, bodyCount);
		readBuff.readIdx += bodyCount;
		readBuff.CheckAndMoveBytes();
			msgList.Add(msgBase);
		}
		msgCount++;
			OnRecieveData();
		}
	}
		MsgUpdate();
		PingUpdate();
	}
			return;
		}
			lock (msgList) {
				if (msgList.Count > 0) {
					msgBase = msgList[0];
					msgList.RemoveAt(0);
					msgCount--;
				}
			}
				FireMsg(msgBase.ToString(), msgBase);
			}
				break;
			}
		}
	}
			return;
		}
		}
			Close();
		}
	}
		lastPongTime = Time.time;
	}
}