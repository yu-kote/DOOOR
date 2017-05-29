using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class AIRouteSearch : AIBasicsMovement
{
    protected RoadPathManager _roadPathManager;

    /// <summary>
    /// ルート検索の指標となるノード
    /// </summary>
    protected Node _targetNode;
    public Node TargetNode { get { return _targetNode; } set { _targetNode = value; } }
    public void SetTargetNode(Node target_node) { _targetNode = target_node; }

    private int _searchLimit = 1000;
    public int SearchLimit { get { return _searchLimit; } set { _searchLimit = value; } }

    List<Node> _searchNodes = new List<Node>();

    private GameObject _testSymbol;
    private List<GameObject> _testSymbolList = new List<GameObject>();

    private int _searchNodeCount;
    public int SearchCount { get { return _searchNodeCount; } }

    // ルートを導き出す移動に切り替わった時に一番最初にルートを検索する関数
    protected override void StartNextNodeSearch()
    {
        var ai_controller = GetComponent<AIController>();
        if (ai_controller.MoveMode != AIController.MoveEmotion.HURRY_UP)
            return;

        NextNodeSearch();
        var tag = gameObject.tag;

        var ai = GetComponent<AIController>();
        var movement = ai.GetMovement();

        var next = movement.NextNode;
        var current = movement.CurrentNode;
        var prev = movement.PrevNode;

        if (ai.PrevNode == null || ai.CurrentNode == null)
            return;
        if (_nextNode == prev)
            return;
        _nextNode = movement.CurrentNode;
        _currentNode = ai.PrevNode;
    }

    /// <summary>
    /// ノードを全体操作するためのスクリプトをもらう
    /// </summary>
    protected void RouteSearchSetup()
    {
        var field = GameObject.Find("Field");
        _roadPathManager = field.GetComponent<RoadPathManager>();
        _testSymbol = Resources.Load<GameObject>("Prefabs/Map/Node/Symbol");
        _nodeController = field.GetComponent<NodeController>();
    }

    // ターゲットを探し出してそこまでのルートをノードに刻む
    protected bool Search()
    {
        SearchStart();
        int limit = 1000;
        for (int i = 0; i < limit; i++)
        {
            if (SearchRoadPath())
            {
                // ターゲットまでのノードの距離を出す
                _searchNodeCount = 0;
                SearchDirectionInvert(_targetNode);
                return true;
            }
        }
        _searchNodes.Clear();
        return false;
    }

    void SearchStart()
    {
        _searchNodes.Add(GetComponent<AIController>().CurrentNode);
    }

    ///<summary>
    /// 幅優先探索
    /// TODO: 最短は求められるけど処理速度はあまり出ない
    ///</summary>
    bool SearchRoadPath()
    {
        bool is_search_end = false;
        List<Node> temp_search_nodes = new List<Node>();

        foreach (var search_node in _searchNodes)
        {
            foreach (var node in search_node.LinkNodes)
            {
                if (node.gameObject.GetComponent<NodeGuide>().PrevPathCheck(gameObject) == true)
                    continue;
                if (node.gameObject.GetComponent<Wall>() != null)
                    continue;
                var ai = GetComponent<AIController>();
                // 殺人鬼が探索中の時は扉の向こうに行けなくする
                if (tag == "Killer" &&
                    ai.MoveMode == AIController.MoveEmotion.DEFAULT &&
                    node.gameObject.GetComponent<Door>() != null)
                    continue;

                temp_search_nodes.Add(node);

                var roadpath = node.gameObject.GetComponent<NodeGuide>();
                roadpath.AddPrevPath(gameObject, search_node);

                if (node == _targetNode)
                {
                    is_search_end = true;
                    break;
                }
            }
            if (temp_search_nodes.Contains(search_node))
                temp_search_nodes.Remove(search_node);
            if (is_search_end)
                break;
        }
        _searchNodes = temp_search_nodes;

        if (is_search_end)
        {
            temp_search_nodes.Clear();
            _searchNodes.Clear();
            return true;
        }
        return false;
    }

    // どのノードから来たのかを見て、
    // 来たノードに自分のノードを教える
    void SearchDirectionInvert(Node current_node)
    {
        _searchNodeCount++;
        if (current_node == GetComponent<AIController>().CurrentNode)
            return;
        var road_path = current_node.GetComponent<NodeGuide>();
        var prev_node = road_path.PrevNode(gameObject);

        prev_node.GetComponent<NodeGuide>().AddNextPath(gameObject, current_node);
        SearchDirectionInvert(prev_node);
    }

    /// <summary>
    /// 経路を表示してみる
    /// </summary>
    protected IEnumerator SearchRoadTestDraw(Node current_node)
    {
        while (current_node != _targetNode)
        {
            var nextnode = current_node.GetComponent<NodeGuide>().NextNode(gameObject);
            if (nextnode == null ||
                _currentNode == nextnode)
            {
                yield return new WaitForSeconds(0.5f);
                continue;
            }
            yield return new WaitForSeconds(0.05f);

            _testSymbolList.Add(Instantiate(_testSymbol, nextnode.transform));
            _testSymbolList.Last().transform.position = nextnode.gameObject.transform.position;
            yield return SearchRoadTestDraw(nextnode);
        }
        while (true)
        {
            yield return null;
            if (_currentNode != _targetNode) continue;
            SymbolDestroy();
            break;
        }
    }

    public void SymbolDestroy()
    {
        foreach (var symbol in _testSymbolList)
            Destroy(symbol);
        _testSymbolList.Clear();
    }

    bool WriteRoadPath(Node current_node)
    {
        _searchNodeCount++;
        if (_searchNodeCount > _searchLimit)
        {
            Debug.Log("search limit");
            return false;
        }

        var roadpath = current_node.gameObject.GetComponent<NodeGuide>();

        // 目標地点までの長さを評価点とする
        // まだ階段が考慮されてないので、階段があったら距離の評価点が狂う
        SortByNodeLength(_targetNode, current_node.LinkNodes);

        var is_done = false;

        foreach (var node in current_node.LinkNodes)
        {
            if (tag == "Killer" &&
                node.gameObject.GetComponent<Door>() != null)
                continue;
            if (node.gameObject.GetComponent<NodeGuide>().NextPathCheck(gameObject) == true)
                continue;
            if (node.gameObject.GetComponent<Wall>() != null)
                continue;

            roadpath.AddNextPath(gameObject, node);
            // 目標地点だったら終了
            if (node == _targetNode)
                return true;

            is_done = WriteRoadPath(node);
            if (is_done == true)
                return true;
        }
        return is_done;
    }

    /// <summary>
    /// ルート検索を終了して通常探索を再開させる
    /// </summary>
    protected void SearchMoveStart()
    {
        if (gameObject == null) return;
        gameObject.AddComponent<AISearchMove>();
        Destroy(this);
        
        var ai_controller = GetComponent<AIController>();
        if (ai_controller.MoveMode == AIController.MoveEmotion.HURRY_UP)
            ai_controller.MoveMode = AIController.MoveEmotion.DEFAULT;
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
}
