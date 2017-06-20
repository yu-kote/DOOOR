using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillApproach : MonoBehaviour
{
    private NodeManager _nodeManager;
    private GameManager _gameManager;
    private Rotater _rotater;

    private float _approachTime;

    private float _startZ;

    private Vector3 _savePosition;
    private Vector3 _saveAngle;
    private int _saveSide;

    private Vector3 _interestPoint;

    void Start()
    {
        _rotater = GetComponent<Rotater>();
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _nodeManager = GameObject.Find("Field").GetComponent<NodeManager>();

        _approachTime = 2.0f;

        _gameManager.StateChangeCallBack(() => _startZ = transform.position.z, GameState.GAMEMAIN);
    }

    public void StartApproach(Vector3 approach_position, int side)
    {
        var offset = 10.0f;
        if (side == 0)
            approach_position += new Vector3(0, 0, -offset);
        if (side == 1)
            approach_position += new Vector3(offset, 0, 0);
        if (side == 2)
            approach_position += new Vector3(0, 0, offset);
        if (side == 3)
            approach_position += new Vector3(-offset, 0, 0);

        StartCoroutine(StartStaging(approach_position, side));
    }

    private IEnumerator StartStaging(Vector3 approach_position, int side)
    {
        _gameManager.CurrentGameState = GameState.STAGING;
        Save();

        // 面から開始位置を導き出す
        var start_position = GetStartPositionFromSide(side);
        transform.position = start_position;

        // 面から開始角度を導き出す
        var start_angle = GetStartAngleFromSide(side);
        transform.eulerAngles = start_angle;

        EasingInitiator.Add(gameObject, approach_position, _approachTime, EaseType.CircOut);

        yield return new WaitForSeconds(_approachTime);
        yield return new WaitForSeconds(3.0f);

        Load();

        yield return new WaitForSeconds(0.5f);
        _gameManager.CurrentGameState = GameState.GAMEMAIN;
    }

    private void Save()
    {
        _savePosition = transform.position;
        _saveAngle = transform.eulerAngles;
        _saveSide = _rotater.CurrentSide;
    }

    private void Load()
    {
        transform.position = _savePosition;
        transform.eulerAngles = _saveAngle;
        _rotater.CurrentSide = _saveSide;
    }

    private Vector3 GetStartPositionFromSide(int side)
    {
        var y = _interestPoint.y;
        if (side == 0)
            return new Vector3(_interestPoint.x, y, _startZ);
        if (side == 1)
            return new Vector3(_interestPoint.x * 2 - _startZ, y, _interestPoint.z);
        if (side == 2)
            return new Vector3(_interestPoint.x, y, _interestPoint.x * 2 - _startZ);
        if (side == 3)
            return new Vector3(_startZ, y, _interestPoint.z);

        return Vector3.zero;
    }

    private Vector3 GetStartAngleFromSide(int side)
    {
        for (int i = 0; i < 4; i++)
            if (side == i)
                return new Vector3(0, 360 - 90 * i, 0);

        return Vector3.zero;
    }


    void Update()
    {
        _interestPoint = _nodeManager.GetNodesCenterPoint();
    }
}
