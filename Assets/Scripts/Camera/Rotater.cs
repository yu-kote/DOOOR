﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotater : MonoBehaviour
{
    // 一度に回転する角度
    private const float _rotateAngle = 360.0f / 4.0f;
    public float RotateAngle { get { return _rotateAngle; } }
    private Vector3 _interestPoint = Vector3.zero;

    [SerializeField]
    private string _leftRotateButton = "L1";
    [SerializeField]
    private string _rightRotateButton = "R1";
    [SerializeField]
    private float _rotateTakeTime = 1.0f;
    private float _time = 0.0f;
    private float _angle = 0.0f;
    private Vector3 _beforePos = Vector3.zero;
    private Vector3 _beforeRotate = Vector3.zero;
    private bool _isRotating = false;
    public bool IsRotating { get { return _isRotating || _isRotateStop; } set { _isRotating = value; } }

    // 今どの面にいるのか
    private int _currentSide;
    public int CurrentSide { get { return _currentSide; } set { _currentSide = value; } }

    // 回転を止める
    private bool _isRotateStop;
    public bool IsRotateStop { get { return _isRotateStop; } set { _isRotateStop = value; } }

    private float _canRotateTime;

    public void StopRotating(float time)
    {
        StartCoroutine(Stop(time));
    }

    private IEnumerator Stop(float time)
    {
        _isRotateStop = true;
        yield return new WaitForSeconds(time);
        _isRotateStop = false;
    }

    void Start()
    {
        // マップの中心点を獲得
    }

    public void Setup()
    {
        _beforePos = Vector3.zero;
        _beforeRotate = Vector3.zero;
        _currentSide = 0;
        _isRotating = false;
        _canRotateTime = 0.5f;
    }

    void Update()
    {
        var interest_point = GameObject.Find("Field").GetComponent<NodeManager>().GetNodesCenterPoint();

        if (GameObject.Find("GameManager").GetComponent<GameManager>().CurrentGameState
            != GameState.GAMEMAIN)
        {
            return;
        }
        _canRotateTime -= Time.deltaTime;
        if (_canRotateTime > 0.0f)
            return;

        Rotating();
        if (Input.GetButton(_leftRotateButton))
            StartRotation(interest_point, _rotateAngle);
        if (Input.GetButton(_rightRotateButton))
            StartRotation(interest_point, -_rotateAngle);

        _currentSide = (int)Mathf.Repeat((float)_currentSide, 4);
    }

    public bool StartRotation(Vector3 interest_point, float rotateAngle)
    {
        if (IsRotating)
            return false;

        IsRotating = true;
        _angle = rotateAngle;

        if (rotateAngle > 0)
            _currentSide--;
        else if (rotateAngle < 0)
            _currentSide++;

        _time = 0.0f;
        _beforePos = transform.position;
        _beforeRotate = transform.eulerAngles;
        _interestPoint = interest_point;
        return true;
    }

    void Rotating()
    {
        if (!_isRotating)
            return;

        transform.position = _beforePos;
        transform.eulerAngles = _beforeRotate;
        _time = Mathf.Min(1.0f, Time.deltaTime / _rotateTakeTime + _time);
        float time_ = EaseLinear(_time, 0.0f, 1.0f);
        transform.RotateAround(_interestPoint, Vector3.up, _angle * time_);

        if (_time < 1.0f)
            return;

        _isRotating = false;
    }

    float EaseLinear(float t, float s, float e)
    {
        return s + (e - s) * t;
    }
}
