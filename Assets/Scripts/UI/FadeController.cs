using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum FadeState
{
    FADE_IN,
    ACTION,
    FADE_OUT,
}

public class FadeController : MonoBehaviour
{
    [SerializeField]
    float _speed;

    Image _image;

    private FadeState _state;
    public FadeState State
    {
        get { return _state; }
        set { _state = value; }
    }


    void Awake()
    {
        _image = GetComponent<Image>();

        _state = FadeState.FADE_IN;
    }

    void Update()
    {
        ColorUpdate();
    }

    private void ColorUpdate()
    {
        var color = _image.color;

        if (_state == FadeState.FADE_OUT)
            color.a += _speed;
        if (_state == FadeState.FADE_IN)
            color.a -= _speed;
        color.a = Mathf.Clamp(color.a, 0.0f, 1.0f);

        _image.color = color;
    }

    public void ChangeFade()
    {
        if (_state == FadeState.FADE_OUT)
            _state = FadeState.FADE_IN;
        else if (_state == FadeState.FADE_IN)
            _state = FadeState.FADE_OUT;
    }

    public void FadeOut()
    {
        _state = FadeState.FADE_OUT;
        ColorUpdate();
    }

    public void FadeIn()
    {
        _state = FadeState.FADE_IN;
        ColorUpdate();
    }

    public bool IsFadeComplete()
    {
        if (_state == FadeState.FADE_OUT)
            return _image.color.a >= 1.0f;
        else if (_state == FadeState.FADE_IN)
            return _image.color.a <= 0.0f;
        return false;
    }
}
