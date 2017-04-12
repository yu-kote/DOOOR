using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AITargetMove : MonoBehaviour
{
    private RoadPathManager _roadPathManager;
    private NodeManager _nodeManager;
    private MyNumber _myNumber;

    private Node _targetNode;
    public Node TargetNode { get { return _targetNode; } set { _targetNode = value; } }

    private Node _currentNode;

    private int searchLimit = 1000;
    private int searchCount = 0;

    private GameObject _testSymbol;

    void Start()
    {
        var field = GameObject.Find("Field");
        _roadPathManager = field.GetComponent<RoadPathManager>();
        _nodeManager = field.GetComponent<NodeManager>();
        _myNumber = GetComponent<MyNumber>();
        _currentNode = _nodeManager.SearchOnNodeHuman(gameObject);
        _targetNode = _nodeManager.Nodes[2][32].GetComponent<Node>();

        _roadPathManager.RoadPathReset();

        if (WriteRoadPath(_currentNode) == false)
        {
            gameObject.AddComponent<AIMovement>();
            Destroy(GetComponent<AITargetMove>());
        }

        _currentNode = _nodeManager.SearchOnNodeHuman(gameObject);
        _testSymbol = Resources.Load<GameObject>("Prefabs/Map/Node/Symbol");

        // テスト
        StartCoroutine(SearchRoad(_currentNode));
    }

    private IEnumerator SearchRoad(Node current_node)
    {
        while (current_node != _targetNode)
        {
            _testSymbol = Instantiate(_testSymbol);

            _testSymbol.transform.position = current_node.gameObject.transform.position;
            var nextnode = current_node.GetComponent<RoadPath>().Direction(gameObject);

            yield return new WaitForSeconds(0.05f);
            yield return SearchRoad(nextnode);
        }
    }

    void Update()
    {

    }
    bool WriteRoadPath(Node current_node)
    {
        searchCount++;
        if (searchCount > searchLimit)
        {
            Debug.Log("search limit");
            return false;
        }

        if (current_node == _targetNode)
        {
            Debug.Log("goal");
            return true;
        }

        var loadpath = current_node.gameObject.GetComponent<RoadPath>();

        // 目標地点までの長さを評価点とする
        // まだ階段が考慮されてないので、階段があったら距離の評価点が狂う
        SortByNodeLength(_targetNode, current_node.LinkNodes);

        //for (int i = 0; i < current_node.LinkNodes.Count; i++)
        foreach (var node in current_node.LinkNodes)
        {
            //var node = current_node.LinkNodes[i];
            if (node.gameObject.GetComponent<RoadPath>()._isDone == true)
                continue;
            if (node.gameObject.GetComponent<Wall>() != null)
                continue;

            loadpath.Add(gameObject, node);
            loadpath._isDone = true;
            if (WriteRoadPath(node))
                return true;
        }

        Debug.Log("search missing");
        return false;
    }

    // 目標地点のノードまでの距離を短い順でソートする（バブルソート）
    void SortByNodeLength(Node targetnode, List<Node> linknodes)
    {
        var tpos = targetnode.transform.position;
        for (int i = 0; i < linknodes.Count; i++)
        {
            for (int k = 0; k < linknodes.Count - i - 1; k++)
            {
                var vec1 = linknodes[k].transform.position - tpos;
                var length1 = vec1.magnitude;
                var vec2 = linknodes[k + 1].transform.position - tpos;
                var length2 = vec2.magnitude;

                if (length1 > length2)
                    Swap(linknodes, k, k + 1);
            }
        }
    }

    public static void Swap<T>(List<T> list, int indexA, int indexB)
    {
        T temp = list[indexA];
        list[indexA] = list[indexB];
        list[indexB] = temp;
    }

    void BubbleSort(int[] value)
    {
        for (int i = 0; i < value.GetLength(0); i++)
        {
            for (int k = 0; k < value.GetLength(0) - i - 1; k++)
            {
                if (value[k] > value[k + 1])
                {
                    Swap(ref value[k], ref value[k + 1]);
                }
            }
        }
    }

    public static void Swap<T>(ref T lhs, ref T rhs)
    {
        T temp = lhs;
        lhs = rhs;
        rhs = temp;
    }
}
