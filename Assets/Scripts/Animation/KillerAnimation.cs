using System;
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
    HAPPY,      // 喜び
    HIT,        // 攻撃された
}

public class KillerAnimation : MonoBehaviour
{
    [SerializeField]
    private KillerAnimationStatus _animStatus;
    public KillerAnimationStatus AnimStatus { get { return _animStatus; } set { _animStatus = value; } }

    private GameObject _soundReactImage;

    private AIController _aiController;
    private GameManager _gameManager;

    void Start()
    {
        _animStatus = KillerAnimationStatus.IDOL;
        _aiController = GetComponent<AIController>();
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        
    }
    
    void Update()
    {
        if (_aiController.MoveMode == AIController.MoveEmotion.REACT_SOUND)
            gameObject.transform.GetChild(0).FindChild("SoundEffect").gameObject.SetActive(true);
        else
            gameObject.transform.GetChild(0).FindChild("SoundEffect").gameObject.SetActive(false);

        if (_gameManager.CurrentGameState == GameState.GAMECLEAR)
        {
            if (_animStatus == KillerAnimationStatus.ATTACK)
                return;
            _animStatus = KillerAnimationStatus.HAPPY;
            return;
        }
        AnimStatusUpdate();
    }

    // 歩きのモーションのみ更新する
    void AnimStatusUpdate()
    {
        if (_animStatus == KillerAnimationStatus.ATTACK)
            return;
        if (_animStatus == KillerAnimationStatus.HIT)
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
        GetComponent<AIController>()
        .StopMovement(2f, () =>
        {
            _animStatus = KillerAnimationStatus.IDOL;
            if (_gameManager.CurrentGameState == GameState.GAMECLEAR)
                _animStatus = KillerAnimationStatus.HAPPY;
        });
    }
}
