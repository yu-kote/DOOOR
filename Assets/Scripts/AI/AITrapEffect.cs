using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AITrapEffect : MonoBehaviour
{
    private NodeManager _nodeManager;
    private Node _currentNode;

    void Start()
    {
        var field = GameObject.Find("Field");
        _nodeManager = field.GetComponent<NodeManager>();
    }

    // 今のところは瞬間移動になる
    public void ToMove(Node target_node)
    {
        _currentNode = _nodeManager.SearchOnNodeHuman(gameObject);

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


    }
}
