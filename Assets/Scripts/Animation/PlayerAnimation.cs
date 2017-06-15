using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerAnimationStatus
{
    USETRAP,
    WALK,
    IDOL,
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

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical") * -1.0f;

        if (horizontal != 0.0f || vertical != 0.0f)
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
}
