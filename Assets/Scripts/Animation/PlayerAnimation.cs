using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerAnimationStatus
{
    IDOL,
    WALK,
    USETRAP,
}

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField]
    private PlayerAnimationStatus _animStatus;
    public PlayerAnimationStatus AnimStatus { get { return _animStatus; } set { _animStatus = value; } }

    private PlayerAction _playerAction;

    void Start()
    {
        _playerAction = GetComponent<PlayerAction>();
    }

    void Update()
    {
        if (_playerAction.IsDoorLock())
        {
            _animStatus = PlayerAnimationStatus.USETRAP;
            return;
        }

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical") * -1.0f;

        if (horizontal != 0.0f || vertical != 0.0f)
            _animStatus = PlayerAnimationStatus.WALK;
        else
            _animStatus = PlayerAnimationStatus.IDOL;

    }
}
