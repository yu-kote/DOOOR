using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 殺人鬼のモーション
/// </summary>
public enum KillerAnimationStatus
{
    IDOL,       // 待機
    WALK,       // 歩き
    RUN,        // 走り
    ATTACK,     // 攻撃
}

public class KillerAnimation : MonoBehaviour
{
    [SerializeField]
    private KillerAnimationStatus _animStatus;
    public KillerAnimationStatus AnimStatus { get { return _animStatus; } set { _animStatus = value; } }

    private AIController _aiController;

    void Start()
    {
        _aiController = GetComponent<AIController>();
    }

    void Update()
    {
        AnimStatusUpdate();
    }

    // 歩きのモーションのみ更新する
    void AnimStatusUpdate()
    {
        if (_animStatus == KillerAnimationStatus.ATTACK)
            return;

        var move_mode = _aiController.MoveMode;

        _animStatus = KillerAnimationStatus.IDOL;
        if (GetComponent<AISearchMove>() || GetComponent<AITargetMove>())
            _animStatus = KillerAnimationStatus.WALK;
        if (GetComponent<AIChace>() && move_mode == AIController.MoveEmotion.DEFAULT)
            _animStatus = KillerAnimationStatus.WALK;
        if (GetComponent<AIChace>() &&
            (move_mode == AIController.MoveEmotion.HURRY_UP || move_mode == AIController.MoveEmotion.REACT_SOUND))
            _animStatus = KillerAnimationStatus.RUN;
    }

    public void KillAnimation()
    {
        _animStatus = KillerAnimationStatus.ATTACK;
    }

}
