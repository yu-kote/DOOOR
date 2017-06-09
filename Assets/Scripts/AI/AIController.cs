using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UniRx;
using System;
using UnityEngine.SceneManagement;

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
    }

    void Update()
    {
        MoveSpeedChange();
        NodeUpdate();
        AimForExit();

        if (SceneManager.GetSceneByName("Title").name == null)
            SoundUpdate();
    }

    private void SoundUpdate()
    {
        var movement = GetMovement();
        if (movement == null)
            return;
        // すでに音を鳴らしていたらはじく
        if (_aiSoundManager.CheckSound(gameObject))
            return;
        // 移動できる状態なら音を鳴らす。移動できない場合は音を消す
        if (movement.CanMove == false)
        {
            _aiSoundManager.RemoveSound(gameObject);
            return;
        }

        _aiSound = _aiSoundManager.MakeSound(gameObject, _soundRange);
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

    float _stopTime;

    public void StopMovement(float time)
    {
        var movement = GetMovement();
        if (movement == null)
            return;
        _stopTime = time;
        StartCoroutine(StopMove());
        Observable.Timer(TimeSpan.FromSeconds(time)).Subscribe(_ =>
        {
            movement.CanMove = true;
        }).AddTo(gameObject);
    }

    private IEnumerator StopMove()
    {
        int count = 0;
        while (count < (_stopTime * 60))
        {
            count++;
            var movement = GetMovement();
            if (movement)
                movement.CanMove = false;
            yield return null;
        }
        GetMovement().MoveSetup();
        GetMovement().CanMove = true;
    }

    public void StopMovement(float time, Action action)
    {
        var movement = GetMovement();
        if (movement == null)
            return;
        movement.MoveSetup();

        _stopTime = time;
        StartCoroutine(StopMove());
        Observable.Timer(TimeSpan.FromSeconds(time)).Subscribe(_ =>
        {
            movement.CanMove = true;
            action();
        }).AddTo(gameObject);
    }

    public void BeKilled()
    {
        StopMovement(1.1f, () => Destroy(gameObject));
        OnDisable();
        GetComponent<VictimAnimation>().DeadAnimation();
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
