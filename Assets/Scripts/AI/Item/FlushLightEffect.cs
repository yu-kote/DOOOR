using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlushLightEffect : MonoBehaviour
{
    GameObject _flushLight;

    private float _effectTime = 10;
    public float EffectTime
    {
        get { return _effectTime; }
        set { _effectTime = value; }
    }

    void Start()
    {
        _flushLight = Instantiate(Resources.Load<GameObject>("Prefabs/Item/FlushLightObject"), transform);
        _flushLight.transform.localPosition = Vector3.zero;
        _flushLight.transform.localScale = Vector3.one;
        _flushLight.transform.localRotation = Quaternion.identity;

        //StartCoroutine(Effect());
    }

    private IEnumerator Effect()
    {
        yield return new WaitForSeconds(_effectTime);
        Destroy(this);
    }

    private void Update()
    {
        _flushLight.transform.position = transform.position + new Vector3(0, 1.5f, 0);
        _flushLight.transform.eulerAngles = transform.GetChild(0).eulerAngles;
        _flushLight.transform.eulerAngles += new Vector3(0, 270, 0);
        _effectTime -= Time.deltaTime;
        if (_effectTime < 0)
            Destroy(this);
    }

    private void OnDestroy()
    {
        if (_flushLight)
            Destroy(_flushLight);
    }
}
