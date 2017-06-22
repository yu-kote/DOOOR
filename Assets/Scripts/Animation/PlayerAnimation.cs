using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UniRx;
using System;

public enum PlayerAnimationStatus
{
    USETRAP,
    WALK,
    USETRAP2,
    UP_TRAP,
    RIGHT_TRAP,
    LEFT_TRAP,
    DOWN_TRAP,
}

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField]
    private PlayerAnimationStatus _animStatus;
    public PlayerAnimationStatus AnimStatus { get { return _animStatus; } set { _animStatus = value; } }
    private PlayerAnimationStatus _currentPlayerAnimStatus;

    [SerializeField]
    private GameObject _playerPrefab;

    private Script_SpriteStudio_Root _root;

    private PlayerAction _playerAction;

    void Start()
    {
        _playerAction = GetComponent<PlayerAction>();
        _animStatus = PlayerAnimationStatus.WALK;
        _currentPlayerAnimStatus = _animStatus;

        _root = _playerPrefab.transform.GetChild(0)
                .gameObject.GetComponent<Script_SpriteStudio_Root>();
        _root.AnimationPlay((int)PlayerAnimationStatus.WALK);
    }

    void Update()
    {
        if (GameObject.Find("GameManager").GetComponent<GameManager>().CurrentGameState
            != GameState.GAMEMAIN)
            return;
        
        if (IsTrapAnimation())
            _animStatus = PlayerAnimationStatus.WALK;
        //else
        //_animStatus = PlayerAnimationStatus.IDOL;

        if (_playerAction.IsDoorLock())
            _animStatus = PlayerAnimationStatus.USETRAP;

        PlayerAnimationStatusUpdate();

        if (GetComponent<Rotater>().IsRotating == false)
            Rotation();
        else
            GetComponent<PlayerMover>().MovingAmount = Vector3.zero;
    }

    // キャラを進む方向に回転させる
    void Rotation()
    {
        var direction = GetComponent<PlayerMover>().MovingAmount;
        direction = new Vector3(direction.x, 0, direction.z);
        if (direction == Vector3.zero)
            return;

        var euler_angle = Quaternion.FromToRotation(new Vector3(0, 0, 1), direction.normalized).eulerAngles;
        euler_angle = new Vector3(0, euler_angle.y + 90.0f, 0);

        var rotation = _playerPrefab.transform.rotation;
        if (rotation.eulerAngles == euler_angle)
            return;
        rotation.eulerAngles = euler_angle;
        _playerPrefab.transform.rotation = rotation;
    }

    void PlayerAnimationStatusUpdate()
    {
        if (_currentPlayerAnimStatus == _animStatus)
            return;
        _currentPlayerAnimStatus = _animStatus;
        _root.AnimationPlay((int)_currentPlayerAnimStatus);
    }

    public void ChangeAnimation(PlayerAnimationStatus status, float time, bool is_stop = false)
    {
        _animStatus = status;
        if (is_stop)
            StopRotateAndMove(time);

        CallBack(time, () =>
        {
            _animStatus = PlayerAnimationStatus.WALK;
        });
    }

    public void StopRotateAndMove(float time)
    {
        GetComponent<Rotater>().StopRotating(time);
        GameObject.Find("MainCamera").GetComponent<Rotater>().StopRotating(time);
    }

    void CallBack(float time, Action action)
    {
        StartCoroutine(CallBackStart(time, action));
    }

    private IEnumerator CallBackStart(float time, Action action)
    {
        yield return new WaitForSeconds(time);
        action();
    }

    bool IsTrapAnimation()
    {
        return _animStatus != PlayerAnimationStatus.UP_TRAP &&
                _animStatus != PlayerAnimationStatus.DOWN_TRAP &&
                _animStatus != PlayerAnimationStatus.RIGHT_TRAP &&
                _animStatus != PlayerAnimationStatus.LEFT_TRAP &&
                _animStatus != PlayerAnimationStatus.USETRAP2;
    }
}
