using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace MiniFramework{ /* * 时间线的使用方法示例： * 1.在相应的脚本中声明一个时间线： TimeLine timeLine = new TimeLine(); * 2.在合适的位置组装时间线，即指定某个时间点执行某个方法，方法默认带一个参数，为动画的ID，： timeLine.AddEvent(1, 1, OnTimeLineStart); * 3.在合适的时候启动时间线： timeLine.Start(); * 4.最后最重要的一点，时间线需要外部驱动，可以再某个Update里面调用 timeLine.Loop(Time.deltaTime); 来驱动 */    /// <summary>
    /// 主要用于控制播放动画的时间点及相应的函数响应
    /// </summary>
    public class TimeLine    {        private Action<float> _update;        private Action _reset;        // 当前计时        private float _curTime = 0;        // 是否开始        private bool _isStart = false;        // 是否暂停        private bool _isPause = false;        public void Start()        {            Reset();            _isStart = true;            _isPause = false;        }        /// <summary>
        /// 添加时间线
        /// </summary>
        /// <param name="time">当前时间线时间开始执行的时间</param>
        /// <param name="ID">动画对应的ID</param>
        /// <param name="callBack">对应的时间点要执行的方法</param>
        public void AddEvent(float time, int ID, Action<int> callBack)        {            LineEvent param = new LineEvent(time, ID, callBack);            _update += param.Invoke;            _reset += param.Reset;        }        /// <summary>
        /// 时间线驱动，需要从外部的Update函数中驱动
        /// </summary>
        /// <param name="deltaTime"></param>
        public void Loop(float deltaTime)        {            if (!_isStart || _isPause)            {                return;            }            _curTime += deltaTime;            if (_update != null)            {                _update(_curTime);            }        }        /// <summary>
        /// 暂停或恢复时间线
        /// </summary>
        public void Pause()        {            _isPause = true;        }        /// <summary>
        /// 恢复时间线
        /// </summary>
        public void Resume()        {            _isPause = false;        }        /// <summary>
        /// 停止时间线
        /// </summary>
        public void Stop()        {            Reset();            _update = null;            _reset = null;        }        public void Reset()        {            _curTime = 0;            _isStart = false;            _isPause = false;            if (_reset != null)            {                _reset.Invoke();            }        }    }    public class LineEvent    {        public float Delay; // 调用时间点        public int ID; // 调用的动画ID        public Action<int> Method; // 动画动作完成的响应        private bool _isInvoke = false;        public LineEvent(float time, int animID, Action<int> callback)        {            Delay = time;            ID = animID;            Method = callback;            Reset();        }        public void Invoke(float time)        {            if (time < Delay)            {                return;            }            if (!_isInvoke && Method != null)            {                Method(ID);                _isInvoke = true;            }        }        public void Reset()        {            _isInvoke = false;        }    }}
