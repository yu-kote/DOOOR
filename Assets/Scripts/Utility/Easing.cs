using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class Easing : MonoBehaviour
{
    void Start()
    {
        // 使い方
        // 現状の設計ではlocalPositionしか動かせませんｍ（_ _）ｍ
        // EasingInitiator.Add(gameObject, new Vector3(10,10,10), 1, EaseType.BackIn);

        // テスト
        EasingInitiator.Add(gameObject, new Vector3(3, 3, -3), 1, EaseType.BackIn);
        EasingInitiator.Add(gameObject, new Vector3(-3, -3, 3), 1, EaseType.BackIn);
        EasingInitiator.Add(gameObject, new Vector3(3, 3, -3), 1, EaseType.BackIn);
        EasingInitiator.Add(gameObject, new Vector3(-3, -3, 3), 1, EaseType.BackIn);
    }

    void Update()
    {
        EasingInitiator.EaseUpdate();
    }
}

public static class EasingInitiator
{
    static Dictionary<GameObject, RunEase> _ease = new Dictionary<GameObject, RunEase>();

    /// <summary>
    /// 指定したオブジェクトをイージングします
    /// </summary>
    /// <param name="target">動かすオブジェクト</param>
    /// <param name="end">動かしたい位置</param>
    /// <param name="time">何秒で動かすか</param>
    /// <param name="type">使うイージングの種類</param>
    public static void Add(GameObject target, Vector3 end, float time, EaseType type)
    {
        if (_ease.ContainsKey(target))
            _ease[target].Add(target, end, time, type);
        else
            _ease.Add(target, new RunEase(target, end, time, type));
    }

    public static void Wait(GameObject target, float time)
    {
        Add(target, target.transform.localPosition, time, EaseType.NONE);
    }

    public static bool IsEaseEnd(GameObject target)
    {
        if (_ease.ContainsKey(target))
            if (_ease[target].IsEaseEnd())
                return true;
        return false;
    }

    public static void EaseUpdate()
    {
        var remove_list = _ease.Where(e => e.Value.IsEaseEnd()).ToList();
        foreach (var item in remove_list)
        {
            _ease.Remove(item.Key);
        }

        foreach (var it in _ease)
        {
            it.Value.Update();
        }
    }
}

class RunEase
{
    Queue<EaseOrigin> _easeAccum = new Queue<EaseOrigin>();
    GameObject _target;

    public RunEase(GameObject target, Vector3 end, float time, EaseType type)
    {
        Add(target, end, time, type);
    }

    public void Add(GameObject target, Vector3 end, float time, EaseType type)
    {
        _target = target;
        _easeAccum.Enqueue(new EaseOrigin(target.transform.localPosition, end, time, type));
    }

    public void Update()
    {
        if (_easeAccum.First().IsDone())
        {
            _easeAccum.Dequeue();
            if (_easeAccum.Count() > 0)
                _easeAccum.First().Begin = _target.transform.localPosition;
        }
        else
        {
            _target.transform.localPosition = _easeAccum.First().CurrentTargetValue();
            _easeAccum.First().Update();
        }
    }

    public void Crear()
    {
        _easeAccum.Clear();
    }

    public bool IsEaseEnd()
    {
        return _easeAccum.Count <= 0;
    }
}

class EaseOrigin
{
    public EaseOrigin(Vector3 begin, Vector3 end, float end_time, EaseType type)
    {
        _begin = begin;
        _end = end;
        _endTime = end_time;
        _easeFunc = EaseList.getEaseFunc(type);
    }

    public Vector3 CurrentTargetValue()
    {
        if (_easeFunc == null)
            return _begin;
        var x = _easeFunc(_count / (_endTime * 60), _begin.x, _end.x);
        var y = _easeFunc(_count / (_endTime * 60), _begin.y, _end.y);
        var z = _easeFunc(_count / (_endTime * 60), _begin.z, _end.z);
        return new Vector3(x, y, z);
    }

    public bool IsDone()
    {
        return _count > (_endTime * 60);
    }

    public void Update()
    {
        _count++;
    }

    private Vector3 _begin;
    public Vector3 Begin { get { return _begin; } set { _begin = value; } }
    private Vector3 _end;
    private float _endTime;
    EasingFunction _easeFunc;
    private int _count;
}

delegate float EasingFunction(float t, float b, float e);

public enum EaseType
{
    NONE,
    Linear,
    BackIn,
    BackOut,
    BackInOut,
    BounceOut,
    BounceIn,
    BounceInOut,
    CircIn,
    CircOut,
    CircInOut,
    CubicIn,
    CubicOut,
    CubicInOut,
    ElasticIn,
    ElasticOut,
    ElasticInOut,
    ExpoIn,
    ExpoOut,
    ExpoInOut,
    QuadIn,
    QuadOut,
    QuadInOut,
    QuartIn,
    QuartOut,
    QuartInOut,
    QuintIn,
    QuintOut,
    QuintInOut,
    SineIn,
    SineOut,
    SineInOut,
};

class EaseList
{
    public static EasingFunction getEaseFunc(EaseType ease_type)
    {
        switch (ease_type)
        {
            case EaseType.NONE:
                return null;
            case EaseType.Linear:
                return EaseLinear;
            case EaseType.BackIn:
                return EaseBackIn;
            case EaseType.BackOut:
                return EaseBackOut;
            case EaseType.BackInOut:
                return EaseBackInOut;
            case EaseType.BounceOut:
                return EaseBounceOut;
            case EaseType.BounceIn:
                return EaseBounceIn;
            case EaseType.BounceInOut:
                return EaseBounceInOut;
            case EaseType.CircIn:
                return EaseCircIn;
            case EaseType.CircOut:
                return EaseCircOut;
            case EaseType.CircInOut:
                return EaseCircInOut;
            case EaseType.CubicIn:
                return EaseCubicIn;
            case EaseType.CubicOut:
                return EaseCubicOut;
            case EaseType.CubicInOut:
                return EaseCubicInOut;
            case EaseType.ElasticIn:
                return EaseElasticIn;
            case EaseType.ElasticOut:
                return EaseElasticOut;
            case EaseType.ElasticInOut:
                return EaseElasticInOut;
            case EaseType.ExpoIn:
                return EaseExpoIn;
            case EaseType.ExpoOut:
                return EaseExpoOut;
            case EaseType.ExpoInOut:
                return EaseExpoInOut;
            case EaseType.QuadIn:
                return EaseQuadIn;
            case EaseType.QuadOut:
                return EaseQuadOut;
            case EaseType.QuadInOut:
                return EaseQuadInOut;
            case EaseType.QuartIn:
                return EaseQuartIn;
            case EaseType.QuartOut:
                return EaseQuartOut;
            case EaseType.QuartInOut:
                return EaseQuartInOut;
            case EaseType.QuintIn:
                return EaseQuintIn;
            case EaseType.QuintOut:
                return EaseQuintOut;
            case EaseType.QuintInOut:
                return EaseQuintInOut;
            case EaseType.SineIn:
                return EaseSineIn;
            case EaseType.SineOut:
                return EaseSineOut;
            case EaseType.SineInOut:
                return EaseSineInOut;
        }
        return null;
    }

    static float EaseLinear(float t, float b, float e)
    {
        return (e - b) * t + b;

    }
    static float EaseBackIn(float t, float b, float e)
    {
        float s = 1.70158f;
        return (e - b) * t * t * ((s + 1) * t - s) + b;

    }
    static float EaseBackOut(float t, float b, float e)
    {
        float s = 1.70158f;
        t -= 1.0f;
        return (e - b) * (t * t * ((s + 1) * t + s) + 1) + b;

    }
    static float EaseBackInOut(float t, float b, float e)
    {
        float s = 1.70158f * 1.525f;
        if ((t /= 0.5f) < 1)
            return (e - b) / 2 * (t * t * ((s + 1) * t - s)) + b;
        t -= 2;
        return (e - b) / 2 * (t * t * ((s + 1) * t + s) + 2) + b;
    }
    static float EaseBounceOut(float t, float b, float e)
    {
        if (t < (1 / 2.75f))
        {
            return (e - b) * (7.5625f * t * t) + b;
        }
        else if (t < (2 / 2.75f))
        {
            t -= (1.5f / 2.75f);
            return (e - b) * (7.5625f * t * t + 0.75f) + b;
        }
        else if (t < (2.5 / 2.75))
        {
            t -= (2.25f / 2.75f);
            return (e - b) * (7.5625f * t * t + 0.9375f) + b;
        }
        else
        {
            t -= (2.625f / 2.75f);
            return (e - b) * (7.5625f * t * t + 0.984375f) + b;
        }
    }
    static float EaseBounceIn(float t, float b, float e)
    {
        return (e - b) - EaseBounceOut(1.0f - t, 0.0f, e - b) + b;
    }
    static float EaseBounceInOut(float t, float b, float e)
    {
        if (t < 0.5f)
            return EaseBounceIn(t * 2.0f, 0.0f, e - b) * 0.5f + b;
        else
            return EaseBounceOut(t * 2.0f - 1.0f, 0.0f, e - b) * 0.5f + (e - b) * 0.5f + b;
    }
    static float EaseCircIn(float t, float b, float e)
    {
        return -(e - b) * (Mathf.Sqrt(1 - t * t) - 1) + b;
    }
    static float EaseCircOut(float t, float b, float e)
    {
        t -= 1.0f;
        return (e - b) * Mathf.Sqrt(1 - t * t) + b;
    }
    static float EaseCircInOut(float t, float b, float e)
    {
        if ((t /= 0.5f) < 1)
            return -(e - b) / 2 * (Mathf.Sqrt(1 - t * t) - 1) + b;
        t -= 2;
        return (e - b) / 2 * (Mathf.Sqrt(1 - t * t) + 1) + b;
    }
    static float EaseCubicIn(float t, float b, float e)
    {
        return (e - b) * t * t * t + b;
    }
    static float EaseCubicOut(float t, float b, float e)
    {
        t -= 1.0f;
        return (e - b) * (t * t * t + 1) + b;
    }
    static float EaseCubicInOut(float t, float b, float e)
    {
        if ((t /= 0.5f) < 1)
            return (e - b) / 2 * t * t * t + b;
        t -= 2;
        return (e - b) / 2 * (t * t * t + 2) + b;
    }
    static float EaseElasticIn(float t, float b, float e)
    {
        if (t == 0) return b;
        if (t == 1) return e;

        float p = 0.3f;
        float a = e - b;
        float s = p / 4.0f;
        t -= 1.0f;
        return -(a * Mathf.Pow(2.0f, 10.0f * t) * Mathf.Sin((t - s) * (2.0f * Mathf.PI) / p)) + b;
    }
    static float EaseElasticOut(float t, float b, float e)
    {
        if (t == 0) return b;
        if (t == 1) return e;

        float p = 0.3f;
        float a = e - b;
        float s = p / 4.0f;
        return (a * Mathf.Pow(2.0f, -10.0f * t) * Mathf.Sin((t - s) * (2.0f * Mathf.PI) / p) + a + b);
    }
    static float EaseElasticInOut(float t, float b, float e)
    {
        if (t == 0) return b;
        if ((t /= 0.5f) == 2) return e;

        float p = 0.3f * 1.5f;
        float a = e - b;
        float s = p / 4.0f;
        if (t < 1.0f)
        {
            t -= 1.0f;
            return -0.5f * (a * Mathf.Pow(2.0f, 10.0f * t) * Mathf.Sin((t - s) * (2.0f * Mathf.PI) / p)) + b;
        }
        t -= 1;
        return a * Mathf.Pow(2.0f, -10.0f * t) * Mathf.Sin((t - s) * (2.0f * Mathf.PI) / p) * 0.5f + a + b;
    }
    static float EaseExpoIn(float t, float b, float e)
    {
        return (t == 0) ? b : (e - b) * Mathf.Pow(2.0f, 10.0f * (t - 1.0f)) + b;
    }
    static float EaseExpoOut(float t, float b, float e)
    {
        return (t == 1.0f) ? e : (e - b) * (-Mathf.Pow(2.0f, -10.0f * t) + 1.0f) + b;
    }
    static float EaseExpoInOut(float t, float b, float e)
    {
        if (t == 0) return b;
        if (t == 1) return e;
        if ((t /= 0.5f) < 1)
            return (e - b) / 2.0f * Mathf.Pow(2.0f, 10.0f * (t - 1.0f)) + b;
        return (e - b) / 2.0f * (-Mathf.Pow(2.0f, -10.0f * --t) + 2.0f) + b;
    }
    static float EaseQuadIn(float t, float b, float e)
    {
        return (e - b) * t * t + b;
    }
    static float EaseQuadOut(float t, float b, float e)
    {
        return -(e - b) * t * (t - 2) + b;
    }
    static float EaseQuadInOut(float t, float b, float e)
    {
        if ((t /= 0.5f) < 1)
            return (e - b) / 2 * t * t + b;
        --t;
        return -(e - b) / 2 * (t * (t - 2) - 1) + b;
    }
    static float EaseQuartIn(float t, float b, float e)
    {
        return (e - b) * t * t * t * t + b;
    }
    static float EaseQuartOut(float t, float b, float e)
    {
        t -= 1.0f;
        return -(e - b) * (t * t * t * t - 1) + b;
    }
    static float EaseQuartInOut(float t, float b, float e)
    {
        if ((t /= 0.5f) < 1)
            return (e - b) / 2 * t * t * t * t + b;
        t -= 2;
        return -(e - b) / 2 * (t * t * t * t - 2) + b;
    }
    static float EaseQuintIn(float t, float b, float e)
    {
        return (e - b) * t * t * t * t * t + b;
    }
    static float EaseQuintOut(float t, float b, float e)
    {
        t -= 1.0f;
        return (e - b) * (t * t * t * t * t + 1) + b;
    }
    static float EaseQuintInOut(float t, float b, float e)
    {
        if ((t /= 0.5f) < 1)
            return (e - b) / 2 * t * t * t * t * t + b;
        t -= 2;
        return (e - b) / 2 * (t * t * t * t * t + 2) + b;
    }
    static float EaseSineIn(float t, float b, float e)
    {
        return -(e - b) * Mathf.Cos(t * (Mathf.PI / 2.0f)) + (e - b) + b;
    }
    static float EaseSineOut(float t, float b, float e)
    {
        return (e - b) * Mathf.Sin(t * (Mathf.PI / 2.0f)) + b;
    }
    static float EaseSineInOut(float t, float b, float e)
    {
        return -(e - b) / 2.0f * (Mathf.Cos(Mathf.PI * t) - 1.0f) + b;
    }
}