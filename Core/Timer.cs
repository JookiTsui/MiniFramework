using System;

/// <summary>
/// 自制定时器
/// </summary>
public class TimerMgr : Singleton<TimerMgr>
{
    private Action<float> _loopCallbacks;
    // 暂停所有计算器
    private bool _isPause;
    
    /// <summary>
    /// repeatTimes小于等于0，代表无限循环
    /// </summary>
    /// <param name="delayTime"></param>
    /// <param name="repeatTimes"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    public Timer CreateTimer(float delayTime, int repeatTimes, Action callback)    {        Timer timer = new Timer(delayTime, repeatTimes, callback);        _loopCallbacks += timer.Loop;        return timer;    }

    public void Start(Timer timer)    {        timer.Start();    }

    /// <summary>
    /// repeatTimes小于等于0，代表无限循环
    /// </summary>
    /// <param name="delayTime"></param>
    /// <param name="repeatTimes"></param>
    /// <param name="callback"></param>
    public void CreateTimerAndStart(float delayTime, Action callback, int repeatTimes = 1)    {        Start(CreateTimer(delayTime, repeatTimes, callback));    }

    /// <summary>
    /// Loop需要通过外部的MonoBehavior的Update来驱动
    /// </summary>
    /// <param name="deltaTime"></param>
    public void Loop(float deltaTime)    {        if (_loopCallbacks != null && !_isPause)        {            _loopCallbacks.Invoke(deltaTime);        }    }

    /// <summary>
    /// 暂停或恢复所有计时器
    /// </summary>
    public void PauseAll()	{        _isPause = !_isPause;	}

    /// <summary>
    /// 停止所有计时器
    /// </summary>
    public void StopAll()	{        _loopCallbacks = null;	}

    /// <summary>
    /// 暂停指定的计时器
    /// </summary>
    /// <param name="timer"></param>
    public void Pause(Timer timer)	{        timer.Pause();	}

    /// <summary>
    /// 停止指定的计时器
    /// </summary>
    /// <param name="timer"></param>
    public void Stop(Timer timer)    {        Pause(timer);        if(_loopCallbacks != null)        {            _loopCallbacks -= timer.Loop;        }    }
}

public class Timer{
    // 执行的间隔时间
    private float _delayTime;
    // 方法要重复的次数
    private int _repeatTimes;
    private Action _method;

    // 是否开始计时
    private bool _isStart = false;
    // 是否暂停计时器
    private bool _isPause = false;
    // 当前持续的时间
    private float _durationTime = 0;
    // 已经重复的次数
    private int _repeatedTimes = 0;

    public Timer(float delayTime, int repeatTimes, Action callback)    {        _delayTime = delayTime;        _repeatTimes = repeatTimes;        _method = callback;    }

    public void Start()	{        _isStart = true;	}

    public void Loop(float detaTime)    {        if (_isStart && !_isPause)        {            _durationTime += detaTime;            if (_durationTime < _delayTime)            {                return;            }            _durationTime -= _delayTime;            _repeatedTimes++;            if (_method != null)            {                _method.Invoke();            }            if (_repeatTimes > 0 &&  _repeatedTimes >= _repeatTimes)            {                TimerMgr.Instance.Stop(this);            }        }    }

    public void Pause()	{        _isPause = true;	}

    public void Resume()	{        _isPause = false;	}
}

