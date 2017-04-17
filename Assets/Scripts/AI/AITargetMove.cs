using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AITargetMove : AIBasicsMovement
{
    private RoadPathManager _roadPathManager;
    private NodeManager _nodeManager;

    private Node _targetNode;
    public Node TargetNode { get { return _targetNode; } set { _targetNode = value; } }
    public void SetTargetNode(Node target_node) { _targetNode = target_node; }

    private Node _searchNode;

    private int searchLimit = 1000;
    private int searchCount = 0;

    private GameObject _testSymbol;
    private List<GameObject> _testSymbolList = new List<GameObject>();

    void Start()
    {
        var field = GameObject.Find("Field");
        _roadPathManager = field.GetComponent<RoadPathManager>();
        _nodeManager = field.GetComponent<NodeManager>();
        _nodeController = field.GetComponent<NodeController>();
        _testSymbol = Resources.Load<GameObject>("Prefabs/Map/Node/Symbol");

        Speed = GetComponent<AIController>().HurryUpSpeed;

        MoveSetup();
    }

    public override void MoveSetup()
    {
        _currentNode = GetComponent<AIController>().CurrentNode;
        _searchNode = _currentNode;

        TargetMoveStart(_targetNode);
    }

    // 引数のノードに移動を始める
    private void TargetMoveStart(Node target_node)
    {
        _targetNode = target_node;
        if (_targetNode == null)
        {
            _targetNode = _currentNode;
            return;
        }

        _nodeController.ReFootPrint(gameObject, _currentNode);

        // 目標地点までたどり着けなかった場合このスクリプトを消して普通の移動を開始させる
        if (WriteRoadPath(_searchNode) == false)
        {
            gameObject.AddComponent<AISearchMove>();
            Destroy(this);
        }

        _searchNode = _currentNode;

        // 道を可視化してみる
        StartCoroutine(SearchRoad(_searchNode));
        _searchNode = _currentNode;
    }

    // ランダムに道を選んでみる 
    private void TargetMoveRandomTest()
    {
        _targetNode = TargetRandomSelect();

        _nodeController.ReFootPrint(gameObject, _currentNode);

        // 目標地点までたどり着けなかった場合このスクリプトを消して普通の移動を開始させる
        if (WriteRoadPath(_searchNode) == false)
        {
            gameObject.AddComponent<AISearchMove>();
            Destroy(this);
        }

        _searchNode = _currentNode;

        // 道を可視化してみる
        StartCoroutine(SearchRoad(_searchNode));
        _searchNode = _currentNode;
    }

    Node TargetRandomSelect()
    {
        while (true)
        {
            var y = UnityEngine.Random.Range(0, _nodeManager.Nodes.Count - 1);
            var x = UnityEngine.Random.Range(0, _nodeManager.Nodes[0].Count - 1);
            if (_nodeManager.Nodes[y][x].GetComponent<Wall>() != null)
                continue;
            return _nodeManager.Nodes[y][x].GetComponent<Node>();
        }
    }

    private IEnumerator SearchRoad(Node current_node)
    {
        while (current_node != _targetNode)
        {
            var nextnode = current_node.GetComponent<RoadPath>().Direction(gameObject);
            if (nextnode == null ||
                _currentNode == nextnode)
            {
                yield return new WaitForSeconds(0.5f);
                continue;
            }
            yield return new WaitForSeconds(0.05f);

            _testSymbolList.Add(Instantiate(_testSymbol, nextnode.transform));
            _testSymbolList.Last().transform.position = nextnode.gameObject.transform.position;
            yield return SearchRoad(nextnode);
        }
        while (true)
        {
            yield return null;
            if (_currentNode != _targetNode) continue;
            SymbolDestroy();
            break;
        }
    }

    private void SymbolDestroy()
    {
        foreach (var symbol in _testSymbolList)
            Destroy(symbol);
        _testSymbolList.Clear();
    }

    void Update()
    {
        if (MoveComplete() &&
            _currentNode == _targetNode)
        {
            gameObject.AddComponent<AISearchMove>();
            Destroy(this);
        }
    }

    protected override void NextNodeSearch()
    {
        _nextNode = _currentNode.GetComponent<RoadPath>().Direction(gameObject);
    }

    bool WriteRoadPath(Node current_node)
    {
        searchCount++;
        if (searchCount > searchLimit)
        {
            searchCount = 0;
            Debug.Log("search limit");
            return false;
        }

        //if (current_node == _targetNode)
        //return true;

        var loadpath = current_node.gameObject.GetComponent<RoadPath>();

        // 目標地点までの長さを評価点とする
        // まだ階段が考慮されてないので、階段があったら距離の評価点が狂う
        SortByNodeLength(_targetNode, current_node.LinkNodes);

        foreach (var node in current_node.LinkNodes)
        {
            if (node.gameObject.GetComponent<RoadPath>().PathCheck(gameObject) == true)
                continue;
            if (node.gameObject.GetComponent<Wall>() != null)
                continue;

            loadpath.Add(gameObject, node);
            if (node == _targetNode)
                return true;
            return WriteRoadPath(node);
        }
        return false;
    }

    // 目標地点のノードまでの距離を短い順でソートする（バブルソート）
    static public void SortByNodeLength(Node targetnode, List<Node> linknodes)
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

    private void OnDestroy()
    {
        SymbolDestroy();
    }
}
