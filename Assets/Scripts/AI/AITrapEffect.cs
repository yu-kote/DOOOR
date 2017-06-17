using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Linq;
using System;
using UnityEngine.SceneManagement;

public class AITrapEffect : MonoBehaviour
{
    private Node _currentNode;
    private AIController _aiController;
    private AIGenerator _aiGenerator;
    private VictimAnimation _victimAnimation;
    private HumanAnimController _humanAnimController;
    private NodeManager _nodeManager;

    void Start()
    {
        _aiGenerator = GameObject.Find("HumanManager").GetComponent<AIGenerator>();
        _aiController = GetComponent<AIController>();
        _victimAnimation = GetComponent<VictimAnimation>();
        _humanAnimController = GetComponent<HumanAnimController>();

        _nodeManager = GameObject.Find("Field").GetComponent<NodeManager>();
    }

    // 落とし穴に落ちる（イージング）
    public void ToMove(Node target_node)
    {
        _currentNode = _aiController.CurrentNode;

        var foot_print = _currentNode.GetComponent<FootPrint>();
        foot_print.StepOut(gameObject);

        foot_print = target_node.GetComponent<FootPrint>();
        foot_print.StepIn(gameObject);

        var target_pos = new Vector3(target_node.transform.position.x,
                                     target_node.transform.position.y + transform.localScale.y,
                                     target_node.transform.position.z);

        float effect_time = 1.0f;

        EasingInitiator.DestoryEase(gameObject);
        EasingInitiator.Add(gameObject, target_pos, effect_time, EaseType.BounceOut);

        _victimAnimation.ChangeAnimation(VictimAnimationStatus.STAGGER, effect_time);

        var movement = _aiController.GetMovement();
        if (movement)
            movement.MoveSetup();
    }

    //ロープの罠にかかった時の処理
    public void ToOverturn()
    {
        //人が転ぶアニメーション記述
        //未実装
        _victimAnimation.ChangeAnimation(VictimAnimationStatus.STAGGER);
        StartCoroutine(Deceleration());
    }

    private IEnumerator Deceleration()
    {
        var defalut_speed = _aiController.DefaultSpeed;
        var hurry_up_speed = _aiController.HurryUpSpeed;

        var killer = _aiGenerator.GetKiller().GetComponent<AIController>();

        _aiController.DefaultSpeed = killer.DefaultSpeed;
        _aiController.HurryUpSpeed = killer.HurryUpSpeed;

        yield return new WaitForSeconds(2.0f);
        if (_aiController == null)
            yield break;

        _victimAnimation.AnimStatus = VictimAnimationStatus.IDOL;
        _aiController.DefaultSpeed = defalut_speed;
        _aiController.HurryUpSpeed = hurry_up_speed;
    }

    private void Update()
    {
        DoorControl();
    }

    // ドアの開き方を反対にするかどうか
    bool _isReverseDoor = false;

    private void DoorControl()
    {
        if (tag != "Victim") return;
        if (_aiController.GetMovement().CanMove == false)
            return;

        var current_node = _aiController.CurrentNode;
        if (current_node == null)
            return;

        var prev_node = _aiController.PrevNode;
        if (prev_node == null)
            return;

        var door = current_node.GetComponent<Door>();
        if (door == null)
            return;

        var side = _nodeManager.WhichSurfaceNum(current_node.CellX);
        var direction = current_node.transform.position - prev_node.transform.position;

        if (side == 0 || side == 2)
            if (direction.x > 0)
                _isReverseDoor = false;
            else
                _isReverseDoor = true;
        if (side == 1 || side == 3)
            if (direction.z < 0)
                _isReverseDoor = false;
            else
                _isReverseDoor = true;

        if (SceneManager.GetSceneByName("Title").name != null)
            if (side == 0)
                if (direction.x > 0)
                    _isReverseDoor = true;
                else
                    _isReverseDoor = false;

        Observable.Timer(TimeSpan.FromSeconds(1f)).Subscribe(_ =>
        {
            door.StartClosing();
        }).AddTo(gameObject);

        if (door._doorStatus == Door.DoorStatus.OPEN)
            return;

        door.StartOpening(_isReverseDoor);
        if (door.IsDoorLock())
            return;

        if (SceneManager.GetSceneByName("Title").name != null)
        {
            _victimAnimation.ChangeAnimation(VictimAnimationStatus.OPEN_DOOR, 0.5f);
            _humanAnimController.Rotation(current_node.gameObject);
        }
    }

}
