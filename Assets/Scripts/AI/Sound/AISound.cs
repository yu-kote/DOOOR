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
    private GameObject _testSphere = null;
    private float _range;
    public float Range { get { return _range; } set { _range = value; } }

    private IDisposable _updateDisposable;

    public void MakeSound(Vector3 pos, float range, int effect_time)
    {
        SoundSetup(pos, range);
        Observable.Timer(TimeSpan.FromSeconds(effect_time)).Subscribe(_ =>
        {
            Destroy(this);
        }).AddTo(this);
    }

    public void MakeSound(GameObject obj, float range)
    {
        _targetObject = obj;
        SoundSetup(obj.transform.position, range, obj);
        _updateDisposable = this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                transform.position = _targetObject.transform.position;
                transform.localScale = new Vector3(_range, _range, _range);
            }).AddTo(_targetObject);
    }

    void SoundSetup(Vector3 pos, float range, GameObject obj = null)
    {
        _testSphere = Resources.Load<GameObject>("Prefabs/Map/TestViewSphere");
        _testSphere = Instantiate(_testSphere, gameObject.transform);

        transform.localPosition = Vector3.zero;
        transform.localEulerAngles = Vector3.zero;

        _range = range;
    }

    private void OnDestroy()
    {
        if (_updateDisposable != null)
            _updateDisposable.Dispose();
    }

}
