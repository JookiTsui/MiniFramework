﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
    /// 加载CSV表的基类
    /// 用法：比如加载一个角色数据表，定义2个结构
    /// 注意：
    /// 1.每一张表默认第一个字段为Int型的ID字段
    /// 2.表中某个字段的值如果是数组，请用 $ 分隔
    /* 