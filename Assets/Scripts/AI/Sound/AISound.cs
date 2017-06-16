using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UniRx;
using UniRx.Triggers;

public class AISound : MonoBehaviour
{
    private GameObject _targetObject = null;
    private GameObject _soundWave = null;
    private float _range;
    public float Range { get { return _range; } set { _range = value; } }

    private IDisposable _updateDisposable;

    // 一時的な音
    public void MakeSound(Vector3 pos, float range, int effect_time, Vector3 angle)
    {
        SoundSetup(pos, range, angle);
        StartCoroutine(Effect(effect_time));
    }

    private IEnumerator Effect(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(this);
    }

    // 継続的にならす音
    public void MakeSound(GameObject obj, float range, Vector3 angle)
    {
        _targetObject = obj;
        SoundSetup(obj.transform.position, range, angle);
        _updateDisposable = this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                if (_targetObject == null)
                {
                    Destroy(this);
                    return;
                }
                transform.position = _targetObject.transform.position;
                transform.localScale = new Vector3(range / 10.0f, range / 10.0f, range / 10.0f);
            });
    }



    void SoundSetup(Vector3 pos, float range, Vector3 angle)
    {
        _soundWave = Resources.Load<GameObject>("Prefabs/Trap/Sound/SoundWave");
        _soundWave = Instantiate(_soundWave, gameObject.transform);

        transform.localPosition = Vector3.zero;
        transform.localEulerAngles = angle;
        transform.position = pos;
        transform.localScale = Vector3.zero;

        _range = range;
        var scale = new Vector3(range / 10.0f, range / 10.0f, range / 10.0f);
        EasingInitiator.Add(gameObject, scale, 1, EaseType.ExpoOut, EaseValue.SCALE);
    }

    private void OnDestroy()
    {
        if (_updateDisposable != null)
            _updateDisposable.Dispose();
        if (_soundWave)
            Destroy(_soundWave);
        if (transform.parent)
            Destroy(gameObject);
    }

}
