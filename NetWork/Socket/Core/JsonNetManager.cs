﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Linq;
	ConnectSucc = 1,
	ConnectFail = 2,
	Close = 3,
}

public enum MsgEncodeType {
	Json = 1,
	Protobuf = 2,
}

public class JsonNetManager {