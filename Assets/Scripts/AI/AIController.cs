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
    private MapBackgrounds _mapBackGrounds;

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

    private bool _moveStop;
    public bool MoveStop { get { return _moveStop; } set { _moveStop = value; } }


    void Start()
    {
        var field = GameObject.Find("Field");
        _nodeManager = field.GetComponent<NodeManager>();
        _nodeController = field.GetComponent<NodeController>();
        _roadPathManager = field.GetComponent<RoadPathManager>();
        _mapBackGrounds = field.GetComponent<MapBackgrounds>();
        _aiGenerator = gameObject.transform.parent.gameObject.GetComponent<AIGenerator>();

        _currentNode = _nodeManager.SearchOnNodeHuman(gameObject);
        GetMovement().Speed = _defaultSpeed;
    }

    void Update()
    {
        MoveSpeedChange();
        NodeUpdate();
        AimForExit();
        BlackOutEffect();

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

#if DEBUG
        if (Input.GetKey(KeyCode.H))
        {
            GetMovement().Speed = 10.0f;
        }
#endif
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
        StartCoroutine(ExitStart());
    }

    private IEnumerator ExitStart()
    {
        for (int i = 0; i < 3; i++)
        {
            yield return new WaitForSeconds(0.5f);

            if (_moveMode != MoveEmotion.TARGET_MOVE)
                yield break;


            while (GetComponent<AITargetMove>())
            {
                if (GetComponent<AITargetMove>())
                    Destroy(GetComponent<AITargetMove>());
                yield return null;
            }
            while (GetComponent<AISearchMove>())
            {
                if (GetComponent<AISearchMove>())
                    Destroy(GetComponent<AISearchMove>());
                yield return null;
            }
        }

        var exit_node = ExitNode();
        if (exit_node == null)
            yield break;
        
        if (_moveMode != MoveEmotion.TARGET_MOVE)
            yield break;
        var mover = gameObject.AddComponent<AITargetMove>();
        mover.SetTargetNode(exit_node);
        mover.Speed = GetComponent<AIController>()._hurryUpSpeed;
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

        if (exit == null)
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
        StartCoroutine(Callback(_stopTime, () => movement.CanMove = true));
    }

    public void StopMovement(float time, Action action)
    {
        var movement = GetMovement();
        if (movement == null)
            return;
        movement.MoveSetup();

        _stopTime = time;
        StartCoroutine(StopMove());
        StartCoroutine(Callback(_stopTime, action));

        //Observable.Timer(TimeSpan.FromSeconds(_stopTime)).Subscribe(
        //    _ => movement.CanMove = true);
    }

    private IEnumerator StopMove()
    {
        float count = 0.0f;
        while (count < _stopTime)
        {
            count += Time.deltaTime;
            var movement = GetMovement();
            if (movement)
                movement.CanMove = false;
            yield return null;
        }
        if (GetMovement())
        {
            GetMovement().MoveSetup();
            GetMovement().CanMove = true;
        }
    }

    private IEnumerator Callback(float time, Action action)
    {
        yield return new WaitForSeconds(time);
        action();
    }

    bool _currentLightOn = true;
    // 停電したときに動きを止める処理
    private void BlackOutEffect()
    {
        if (_moveStop)
            if (GetMovement())
                GetMovement().CanMove = false;

        // 停電の時は一度だけ通る
        if (_mapBackGrounds.IsLightOn == _currentLightOn)
            return;

        if (GetMovement().MoveComplete() == false)
            return;
        _currentLightOn = _mapBackGrounds.IsLightOn;

        if (tag == "Killer")
            return;

        if (GetMovement() == null)
            return;

        if (_currentLightOn == false)
        {
            GetMovement().CanMove = false;
            _moveStop = true;
            GetMovement().MoveSetup();
        }
        else
        {
            _moveStop = false;
            GetMovement().CanMove = true;
            GetMovement().MoveSetup();
        }
    }

    public void BeKilled()
    {
        StopMovement(2f, () => Destroy(gameObject));
        OnDisable();
        GetComponent<VictimAnimation>().DeadAnimation();

        if (GetComponent<AIItemController>().HaveItemCheck(ItemType.LASTKEY))
        {
            CurrentNode.gameObject.GetComponent<ItemStatus>().AddPutItem((int)ItemType.LASTKEY);
            SoundManager.Instance.StopBGM();
            SoundManager.Instance.PlayBGM("ingame");
        }
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
        EasingInitiator.DestoryEase(gameObject);
    }
}
