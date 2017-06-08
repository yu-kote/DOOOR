using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Linq;
using System;

public class AITrapEffect : MonoBehaviour
{
    private Node _currentNode;
    private AIController _aiController;
    private AIGenerator _aiGenerator;
    private VictimAnimation _victimAnimation;

    void Start()
    {
        _aiGenerator = GameObject.Find("HumanManager").GetComponent<AIGenerator>();
        _aiController = GetComponent<AIController>();
        _victimAnimation = GetComponent<VictimAnimation>();
    }

    // 今のところは瞬間移動になる
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

        EasingInitiator.Add(gameObject, target_pos, 2, EaseType.BounceOut);

        _victimAnimation.AnimStatus = VictimAnimationStatus.STAGGER;
        _aiController.StopMovement(2, () => _victimAnimation.AnimStatus = VictimAnimationStatus.IDOL);
    }

    //ロープの罠にかかった時の処理
    public void ToOverturn()
    {
        //人が転ぶアニメーション記述
        //未実装
        _victimAnimation.AnimStatus = VictimAnimationStatus.STAGGER;
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

    private void DoorControl()
    {
        if (tag != "Victim") return;

        var current_node = _aiController.CurrentNode;
        if (current_node == null) return;

        var door = current_node.GetComponent<Door>();
        if (door == null) return;

        Observable.Timer(TimeSpan.FromSeconds(1f)).Subscribe(_ =>
        {
            door.StartClosing();
        }).AddTo(gameObject);

        if (door._doorStatus == Door.DoorStatus.OPEN) return;

        door.StartOpening();
        _victimAnimation.AnimStatus = VictimAnimationStatus.OPEN_DOOR;
        _aiController.StopMovement(0.5f, () => _victimAnimation.AnimStatus = VictimAnimationStatus.IDOL);
    }

}
