using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum FadeType
{
    FADE_IN,
    FADE_OUT,
}

public class FadeController : MonoBehaviour
{
    float _alpha;
    [SerializeField]
    float _speed;

    Image _image;
    FadeType _type;

    void Start()
    {
        _alpha = 0.0f;
        _image = GetComponent<Image>();

        _type = FadeType.FADE_IN;
    }

    void Update()
    {
        _alpha = Mathf.Clamp(_alpha, 0.0f, 1.0f);

        if (_type == FadeType.FADE_OUT)
            _image.color += new Color(0, 0, 0, _speed);
        if (_type == FadeType.FADE_IN)
            _image.color -= new Color(0, 0, 0, _speed);
    }

    public void ChangeFade()
    {
        if (_type == FadeType.FADE_OUT)
            _type = FadeType.FADE_IN;
        else if (_type == FadeType.FADE_IN)
            _type = FadeType.FADE_OUT;
    }

    public void FadeOut()
    {
        _type = FadeType.FADE_OUT;
    }

    public void FadeIn()
    {
        _type = FadeType.FADE_IN;
    }
}
