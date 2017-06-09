using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 探索者のアニメーション
/// 待機モーションから派生させる
/// </summary>
public enum VictimAnimationStatus
{
    IDOL,       // 待機
    WALK,       // 歩き
    RUN,        // 走り
    DEAD,       // 死亡
    OPEN_DOOR,  // ドアを開ける
    STAGGER,    // 罠にかかる(ふらつく)
    CRISIS,     // 追いつめられる
    USE_ITEM,   // アイテム使用
}

public class VictimAnimation : MonoBehaviour
{
    // デバッグ用の表示です
    [SerializeField]
    private VictimAnimationStatus _animStatus;
    public VictimAnimationStatus AnimStatus { get { return _animStatus; } set { _animStatus = value; } }

    private AIController _aiController;
    private HumanAnimController _humanAnimController;

    void Start()
    {
        _aiController = GetComponent<AIController>();
        _humanAnimController = GetComponent<HumanAnimController>();
    }

    void Update()
    {
        AnimStatusUpdate();
    }

    // 歩きのモーションのみ更新する
    void AnimStatusUpdate()
    {
        // 死んだら更新を止める
        if (_animStatus == VictimAnimationStatus.DEAD)
            return;

        // 追いつめられモーション
        AnimCrisis();

        if (_animStatus == VictimAnimationStatus.OPEN_DOOR ||
            _animStatus == VictimAnimationStatus.STAGGER ||
            _animStatus == VictimAnimationStatus.CRISIS)
            return;

        _animStatus = VictimAnimationStatus.IDOL;
        var move_mode = _aiController.MoveMode;

        if (GetComponent<AISearchMove>() || GetComponent<AITargetMove>())
            _animStatus = VictimAnimationStatus.WALK;
        if (GetComponent<AIRunAway>() && move_mode == AIController.MoveEmotion.DEFAULT)
            _animStatus = VictimAnimationStatus.WALK;
        if (GetComponent<AIRunAway>() &&
            (move_mode == AIController.MoveEmotion.HURRY_UP || move_mode == AIController.MoveEmotion.REACT_SOUND))
            _animStatus = VictimAnimationStatus.RUN;

        // 探索を開始していなければは待機状態
        if (GetComponent<AIBeginMove>())
            if (GetComponent<AIBeginMove>().IsSearchStart == false)
                _animStatus = VictimAnimationStatus.IDOL;
    }

    // 追い詰められモーション
    void AnimCrisis()
    {
        // 移動停止の関数が呼ばれていたら更新しない
        if (_aiController.GetMovement().CanMove == false)
            return;

        var run_away = GetComponent<AIRunAway>();

        if (run_away == null &&
            _aiController.MoveMode == AIController.MoveEmotion.DEFAULT)
        {
            if (_animStatus == VictimAnimationStatus.CRISIS)
                _animStatus = VictimAnimationStatus.IDOL;
            return;
        }

        if (_aiController.NextNode == null ||
            _aiController.CurrentNode == null ||
            _aiController.PrevNode == null)
            return;

        // ドアが閉まっていたらいきどまる
        if (run_away)
            if (run_away.IsDoorCaught)
            {
                _animStatus = VictimAnimationStatus.CRISIS;
                return;
            }

        // 移動先がある場合ははじく(追いつめられていたらモーションを終了する)
        if (_aiController.NextNode != _aiController.CurrentNode ||
            _aiController.NextNode != _aiController.PrevNode ||
            _aiController.CurrentNode != _aiController.PrevNode)
        {
            if (_animStatus == VictimAnimationStatus.CRISIS)
                _animStatus = VictimAnimationStatus.IDOL;
            return;
        }
        _animStatus = VictimAnimationStatus.CRISIS;

        var approach_node = run_away.ApproachNode;
        if (approach_node)
            _humanAnimController.Rotation(approach_node.gameObject);
    }

    public void DeadAnimation()
    {
        _animStatus = VictimAnimationStatus.DEAD;
    }
}
