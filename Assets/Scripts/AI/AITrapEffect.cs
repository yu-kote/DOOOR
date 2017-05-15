using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Linq;
using System;

public class AITrapEffect : MonoBehaviour
{
    //private NodeManager _nodeManager;
    private Node _currentNode;
    private AIController _aiController;

    void Start()
    {
        //var field = GameObject.Find("Field");
        //_nodeManager = field.GetComponent<NodeManager>();
        _aiController = GetComponent<AIController>();
    }

    // 今のところは瞬間移動になる
    public void ToMove(Node target_node)
    {
        var ai_controller = GetComponent<AIController>();
        _currentNode = ai_controller.CurrentNode;

        var foot_print = _currentNode.GetComponent<FootPrint>();
        foot_print.StepOut(gameObject);

        foot_print = target_node.GetComponent<FootPrint>();
        foot_print.StepIn(gameObject);

        gameObject.transform.position =
                new Vector3(target_node.transform.position.x,
                            target_node.transform.position.y + transform.localScale.y,
                            target_node.transform.position.z);
        
        var movement = GetComponent<AIController>().GetMovement();
        if (movement == null) return;
        movement.MoveSetup();
    }

    //ロープの罠にかかった時の処理
    public void ToOverturn()
    {
        //人が転ぶアニメーション記述
        //未実装

        var movement = GetComponent<AIController>().GetMovement();
        if (movement == null) return;

        movement.CanMove = false;
        // 時間はまだ決め打ち　　　　　　　　　　↓
        Observable.Timer(TimeSpan.FromSeconds(2)).Subscribe(_ =>
        {
            movement.CanMove = true;
        }).AddTo(this);
    }

    private void Update()
    {
        DoorControl();
    }

    private void DoorControl()
    {
        if (tag != "Victim") return;

        var door = _aiController.CurrentNode.GetComponent<Door>();
        if (door == null) return;

        Observable.Timer(TimeSpan.FromSeconds(2)).Subscribe(_ =>
        {
            door.StartClosing();
        }).AddTo(this);

        if (door._doorStatus == Door.DoorStatus.OPEN) return;

        door.StartOpening();
    }

}
