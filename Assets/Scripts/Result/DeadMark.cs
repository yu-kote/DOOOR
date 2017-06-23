using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeadMark : MonoBehaviour
{
    private int _number;
    public int Number { get { return _number; } set { _number = value; } }

    Image _background;
    Transform _crossTransform;

    float _moveTime;
    void Start()
    {
        _background = GetComponent<Image>();
        _background.color = new Color(1, 1, 1, 0);
        _crossTransform = transform.GetChild(0);

        _crossTransform.localScale = new Vector3(3, 3, 0);
        _crossTransform.GetComponent<Image>().color = new Color(1, 1, 1, 0);

        _moveTime = 1.0f;
        StartCoroutine(Staging());
    }

    private IEnumerator Staging()
    {
        yield return new WaitForSeconds(0.5f);
        yield return new WaitForSeconds(_number * 0.3f);

        EasingInitiator.Add(_crossTransform.gameObject, Vector3.one, _moveTime,
                            EaseType.CircIn, EaseValue.SCALE);

        var color = _crossTransform.GetComponent<Image>().color;
        while (true)
        {
            if (EasingInitiator.IsEaseEnd(_crossTransform.gameObject, EaseValue.SCALE))
            {
                SoundManager.Instance.PlaySE("harituke");
                _background.color = new Color(0, 0, 0, 0.5f);
                yield break;
            }
            color.a += 0.01f;
            _crossTransform.GetComponent<Image>().color = color;
            yield return null;
        }
    }

    void Update()
    {

    }
}
