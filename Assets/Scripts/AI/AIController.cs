using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UniRx;
using System;

public class AIController : MonoBehaviour
{
    private Node _currentNode;
    public Node CurrentNode { get { return _currentNode; } set { _currentNode = value; } }
    private Node _nextNode;
    public Node NextNode { get { return _nextNode; } set { _nextNode = value; } }
    private Node _prevNode;
    public Node PrevNode { get { return _prevNode; } set { _prevNode = value; } }

    private AIGenerator _aiGenerator;
    private NodeManager _nodeManager;
    private NodeController _nodeController;
    private RoadPathManager _roadPathManager;
    private AISoundManager _aiSoundManager;

    public enum MoveEmotion
    {
        DEFAULT,
        HURRY_UP,
        REACT_SOUND,
        TARGET_MOVE,
    }

    [SerializeField]
    private MoveEmotion _moveMode = MoveEmotion.DEFAULT;
    public MoveEmotion MoveMode { get { return _moveMode; } set { _moveMode = value; } }

    /// <summary>
    /// 基本的に何かしらのモーションが終わった後は
    /// 待機モーションに戻せばバグらないはず
    /// </summary>
    public enum AnimationStatus
    {
        IDOL,       // 待機
        WALK,       // 歩き
        RUN = 3,    // 走り
        OPEN_DOOR,  // ドアを開ける
        STAGGER,    // 罠にかかる(ふらつく)
        CRISIS,     // 追いつめられる
        DEAD,       // 死亡
    }
    [SerializeField]
    private AnimationStatus _animStatus;
    public AnimationStatus AnimStatus { get { return _animStatus; } set { _animStatus = value; } }

    [SerializeField]
    private float _defaultSpeed;
    public float DefaultSpeed { get { return _defaultSpeed; } set { _defaultSpeed = value; } }
    [SerializeField]
    private float _hurryUpSpeed;
    public float HurryUpSpeed { get { return _hurryUpSpeed; } set { _hurryUpSpeed = value; } }

    [SerializeField]
    private float _soundRange = 5;
    private AISound _aiSound;

    void Start()
    {
        var field = GameObject.Find("Field");
        _nodeManager = field.GetComponent<NodeManager>();
        _nodeController = field.GetComponent<NodeController>();
        _roadPathManager = field.GetComponent<RoadPathManager>();
        _aiSoundManager = field.GetComponent<AISoundManager>();
        _aiGenerator = gameObject.transform.parent.gameObject.GetComponent<AIGenerator>();

        _currentNode = _nodeManager.SearchOnNodeHuman(gameObject);
        GetMovement().Speed = _defaultSpeed;
        _aiSound = _aiSoundManager.MakeSound(gameObject, _soundRange);
    }

    void Update()
    {
        MoveSpeedChange();
        NodeUpdate();
        AimForExit();
        SoundUpdate();
        AnimStatusUpdate();
    }

    private void MoveSpeedChange()
    {
        if (GetMovement() == null)
            return;

        if (_moveMode == MoveEmotion.DEFAULT)
            GetMovement().Speed = _defaultSpeed;
        if (_moveMode == MoveEmotion.HURRY_UP)
            GetMovement().Speed = _hurryUpSpeed;
        if (_moveMode == MoveEmotion.REACT_SOUND)
        {
            //if (tag == "Victim")
            GetMovement().Speed = _hurryUpSpeed;
            //if (tag == "Killer")
            //GetMovement().Speed = _defaultSpeed;
        }
    }

    public AIBasicsMovement GetMovement()
    {
        AIBasicsMovement movement = null;
        if (GetComponent<AISearchMove>())
            movement = GetComponent<AISearchMove>();
        if (GetComponent<AITargetMove>())
            movement = GetComponent<AITargetMove>();
        if (GetComponent<AIRunAway>())
            movement = GetComponent<AIRunAway>();
        if (GetComponent<AIChace>())
            movement = GetComponent<AIChace>();
        return movement;
    }

    private bool IsHurryUp()
    {
        if (GetComponent<AISearchMove>())
            return false;
        if (GetComponent<AITargetMove>())
            return false;
        if (GetComponent<AIRunAway>())
            return true;
        if (GetComponent<AIChace>())
            return true;
        return false;
    }

    public void NodeUpdate()
    {
        var movement = GetMovement();
        if (movement == null)
            return;

        if (movement.CurrentNode &&
            _currentNode != movement.CurrentNode)
            _currentNode = movement.CurrentNode;

        if (movement.NextNode &&
            _nextNode != movement.NextNode)
            _nextNode = movement.NextNode;

        if (movement.PrevNode &&
            _prevNode != movement.PrevNode)
            _prevNode = movement.PrevNode;
    }

    /// <summary>
    /// 出口の鍵を持っていたら出口を目指す
    /// </summary>
    void AimForExit()
    {
        var item_contoller = GetComponent<AIItemController>();
        if (item_contoller == null)
            return;
        // 出口の鍵をもっているか
        if (item_contoller.HaveItemCheck(ItemType.LASTKEY) == false)
            return;

        var exit = _currentNode.GetComponent<Deguti>();
        if (exit)
            return;

        if (ExitNode() == null)
            return;
        // 出口に足を踏み入れたことがあるかどうか
        if (ExitNode().GetComponent<FootPrint>().TraceCheck(gameObject) == false)
            return;

        // 探索している時だけ出口を目指せる
        if (_moveMode != MoveEmotion.DEFAULT)
            return;
        _moveMode = MoveEmotion.TARGET_MOVE;

        // 同フレームで探索移動をdestroyしているので未定義で死ぬのを防御するため
        // 数フレーム遅らせる。
        // TODO:設計見直し箇所
        Observable.Timer(TimeSpan.FromSeconds(0.1f)).Subscribe(_ =>
        {
            var exit_node = ExitNode();
            if (exit_node == null)
                return;
            if (GetComponent<AITargetMove>())
                Destroy(GetComponent<AITargetMove>());
            if (GetComponent<AISearchMove>())
                Destroy(GetComponent<AISearchMove>());
            var mover = gameObject.AddComponent<AITargetMove>();
            mover.SetTargetNode(exit_node);
            mover.Speed = GetComponent<AIController>()._hurryUpSpeed;

        }).AddTo(gameObject);
    }

    // 出口のノードを返す関数
    public Node ExitNode()
    {
        var exit_list = _nodeManager.Nodes
        .FirstOrDefault(nodes => nodes
        .FirstOrDefault(node => node.GetComponent<Deguti>() != null));

        if (exit_list == null)
            return null;
        var exit = exit_list
            .FirstOrDefault(node => node.GetComponent<Deguti>() != null);

        if (exit)
            return null;
        var exit_node = exit.GetComponent<Node>();
        return exit_node;
    }

    void SoundUpdate()
    {
        var resound = CurrentNode;//.GetComponent<Resound>();
        if (resound == null)
            return;

    }

    // 歩きのモーションのみ更新する
    void AnimStatusUpdate()
    {
        // 死んだら更新を止める
        if (_animStatus == AnimationStatus.DEAD)
            return;

        // 追いつめられモーション
        AnimCrisis();

        if (_animStatus == AnimationStatus.OPEN_DOOR ||
            _animStatus == AnimationStatus.STAGGER ||
            _animStatus == AnimationStatus.CRISIS)
            return;

        _animStatus = AnimationStatus.IDOL;
        if (GetComponent<AISearchMove>() || GetComponent<AITargetMove>())
            _animStatus = AnimationStatus.WALK;
        if (GetComponent<AIRunAway>() && _moveMode == MoveEmotion.DEFAULT)
            _animStatus = AnimationStatus.WALK;
        if (GetComponent<AIRunAway>() && _moveMode == MoveEmotion.HURRY_UP)
            _animStatus = AnimationStatus.RUN;
    }


    void AnimCrisis()
    {
        if (_nextNode == null ||
            _currentNode == null ||
            _prevNode == null)
            return;

        // 移動先がある場合ははじく(追いつめられていたらモーションを終了する)
        if (_nextNode != _currentNode ||
            _nextNode != _prevNode ||
            _currentNode != _prevNode)
        {
            if (_animStatus == AnimationStatus.CRISIS)
                _animStatus = AnimationStatus.IDOL;
            return;
        }
        _animStatus = AnimationStatus.CRISIS;
    }

    public void StopMovement(float time)
    {
        var movement = GetMovement();
        if (movement == null)
            return;

        movement.CanMove = false;
        Observable.Timer(TimeSpan.FromSeconds(time)).Subscribe(_ =>
        {
            movement.CanMove = true;
        }).AddTo(gameObject);
    }

    public void StopMovement(float time, Action action)
    {
        var movement = GetMovement();
        if (movement == null)
            return;

        movement.CanMove = false;
        Observable.Timer(TimeSpan.FromSeconds(time)).Subscribe(_ =>
        {
            movement.CanMove = true;
            action();
        }).AddTo(gameObject);
    }

    public void BeKilled()
    {
        StopMovement(3, () => Destroy(gameObject));
        OnDisable();
    }

    private void OnDisable()
    {
        // この世界に残した跡をすべて消し去る
        _nodeController.EraseTraces(GetComponent<MyNumber>());
        _roadPathManager.RoadGuideReset(gameObject);
        _roadPathManager.SearchReset(gameObject);

        if (_currentNode != null)
            _currentNode.GetComponent<FootPrint>().EraseHumanOnNode(gameObject);

        _aiGenerator.Humans.Remove(gameObject);
    }
}
