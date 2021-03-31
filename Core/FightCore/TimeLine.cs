﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace MiniFramework
    /// 主要用于控制播放动画的时间点及相应的函数响应
    /// </summary>
    public class TimeLine
        /// 添加时间线
        /// </summary>
        /// <param name="time">当前时间线时间开始执行的时间</param>
        /// <param name="ID">动画对应的ID</param>
        /// <param name="callBack">对应的时间点要执行的方法</param>
        public void AddEvent(float time, int ID, Action<int> callBack)
        /// 时间线驱动，需要从外部的Update函数中驱动
        /// </summary>
        /// <param name="deltaTime"></param>
        public void Loop(float deltaTime)
        /// 暂停或恢复时间线
        /// </summary>
        public void Pause()
        /// 恢复时间线
        /// </summary>
        public void Resume()
        /// 停止时间线
        /// </summary>
        public void Stop()