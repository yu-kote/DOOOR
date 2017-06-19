using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PitFall : MonoBehaviour
{
    public FootPrint _footPrint = null;
    private NodeManager _nodeManager = null;
    private Vector2 _nodeCell = Vector2.zero;
    public Vector2 NodeCell
    {
        get { return _nodeCell; }
        set { _nodeCell = value; }
    }

    private bool _isUsed = false;
    public bool IsUsed
    {
        get { return _isUsed; }
        set { _isUsed = value; }
    }

    void Start()
    {
        if (_footPrint == null)
            Debug.Log("_footPrint is null");

        _nodeManager = GameObject.Find("Field").GetComponent<NodeManager>();
        if (_nodeManager == null)
            Debug.Log("_nodeManager is null");
    }

    void Update()
    {
        //発動済みだったらはじく
        if (_isUsed)
            return;
        //一階だったらはじく
        if (_nodeCell.y == _nodeManager.Nodes.Count - 1)
            return;
        //ノードに人が一人もいなければはじく
        if (_footPrint.HumansOnNode.Count == 0)
            return;
        if (_footPrint.HumansOnNode.Count == 1)
        {
            if (_footPrint.HumansOnNode[0].tag == "Killer")
                return;
        }

        //ここに落とし穴のアニメーション開始処理を記述する

        List<List<GameObject>> nodes = _nodeManager.Nodes;
        GameObject underNode = nodes[(int)_nodeCell.y + 1][(int)_nodeCell.x];
        //下に壁かドアがあれば落ちない
        if (underNode.GetComponent<Wall>() != null ||
            underNode.GetComponent<Door>() != null)
            return;

        int fallNum = 0;
        for (int i = 0; i < _footPrint.HumansOnNode.Count; i++)
        {
            if (_footPrint.HumansOnNode[i].tag == "Killer")
                continue;
            var movement = _footPrint.HumansOnNode[i].GetComponent<AIController>().GetMovement();
            if (movement.MoveComplete())
            {
                _footPrint.HumansOnNode[i].GetComponent<AITrapEffect>().ToMove(underNode.GetComponent<Node>());
                fallNum++;
                _footPrint.gameObject.GetComponent<TrapStatus>().IsSpawn = false;
                break;
            }
        }

        if (fallNum == 0)
            return;

        _isUsed = true;
        Destroy(gameObject);
    }
}
