using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AIRunAway : AIRouteSearch
{
    private float _endDistance = 20;
    public float EndDistance { get { return _endDistance; } set { _endDistance = value; } }
    [SerializeField]
    private int _endNodeDistance = 7;

    private int _endFlame;

    private Node _approachNode;
    public Node ApproachNode { get { return _approachNode; } set { _approachNode = value; } }

    private GameObject _targetHuman;
    public GameObject TargetHuman { get { return _targetHuman; } set { _targetHuman = value; } }
    public void SetTargetHuman(GameObject target_node) { _targetHuman = target_node; }

    private bool _isEscape = false;
    private int _routeCount;

    private bool _isDoorCaught;
    public bool IsDoorCaught { get { return _isDoorCaught; } set { _isDoorCaught = value; } }

    void Start()
    {
        RouteSearchSetup();
        MoveSetup();
        _isDoorCaught = false;
    }

    public override void MoveSetup()
    {
        _currentNode = GetComponent<AIController>().CurrentNode;
        _isEscape = false;
        _endFlame = 300;

        MoveReset();
    }

    /// <summary>
    /// 敵に近づく以外のルートを返す
    /// </summary>
    List<Node> EscapeNodes()
    {
        _approachNode = SearchApproachNode();
        return _currentNode.LinkNodes
            .Where(node => node.GetComponent<Wall>() == null)
            .Where(node => _approachNode != node)
            .ToList();
    }

    // 近づくルート
    Node SearchApproachNode()
    {
        if (Search())
            return _currentNode.GetComponent<NodeGuide>().NextNode(gameObject);
        return null;
    }

    bool RunAway()
    {
        if (_isEscape) return true;

        // 逃げ切りの距離を出すための位置を取得
        var escape_pos = Vector3.zero;
        if (_targetHuman)
            escape_pos = _targetHuman.transform.position;
        else
            escape_pos = _targetNode.transform.position;

        var vec = escape_pos - _currentNode.transform.position;
        var distance = vec.magnitude;

        // 逃げ切り判定
        {
            // 逃げ切ったら終わり
            //if (distance > _endDistance)
            //    return true;
            if (SearchCount > _endNodeDistance)
                return true;
            if (_endFlame-- < 0)
                return true;
        }

        // 逃げる対象が人間だった場合は遠ざかるノードを更新する
        if (_targetHuman)
            _targetNode = _targetHuman.GetComponent<AIController>().CurrentNode;

        Node next_node = null;

        // 階段の時は逃げ道の数を数えてより多く逃げられる方向に進む
        next_node = StairsPoint(EscapeNodes());
        _roadPathManager.RoadGuideReset(gameObject);

        // 壁かどうか
        if (next_node.GetComponent<Wall>() != null)
            return false;

        // 階段がロックされていたら通れない
        if (IsStairsLock(next_node))
        {
            next_node = _currentNode.LinkNodes.Where(node => _approachNode != node &&
                                                next_node != node).FirstOrDefault();
            if (next_node == null)
                return false;
        }

        // ドアの鍵が閉まっているかどうか
        if (IsDoorLock(next_node))
        {
            if (_isDoorCaught == false)
            {
                SoundManager.Instance.PlaySE("akanaidoa", gameObject);
                GetComponent<VictimAnimation>().ChangeAnimation(VictimAnimationStatus.OPEN_DOOR, 0.7f);
                GetComponent<HumanAnimController>().Rotation(next_node.gameObject);
            }
            _isDoorCaught = true;
            return false;
        }
        _isDoorCaught = false;

        _nextNode = next_node;
        PrevNodeUpdate();

        return false;
    }

    // 遠くに逃げる
    Node GoAway()
    {
        // 離れる方のノードに逃げるため、短い順にソートしてLastを選ぶ
        SortByNodeLength(_targetNode, _currentNode.LinkNodes);

        var next_candidate_node = _currentNode.LinkNodes.Last();

        return next_candidate_node;
    }

    // どっちに進めば壁がないか調べる
    Node StairsPoint(List<Node> link_nodes)
    {
        // つながっているノードを見る
        // ある方向に逃げた場合どのぐらいの距離逃げることが出来るのかを割り出す
        // それを比べて、逃げるノードを選択する

        int select_node_num = -1;
        int most_node_route = -1;

        _roadPathManager.RoadGuideReset(gameObject);

        for (int i = 0; i < link_nodes.Count; i++)
        {
            var nodeguide = _currentNode.GetComponent<NodeGuide>();
            nodeguide.AddNextPath(gameObject, _currentNode);

            int route_count = 0;

            SearchNumOfEscapeRoutes(link_nodes[i]);

            _roadPathManager.RoadGuideReset(gameObject);

            route_count = _routeCount;
            _routeCount = 0;

            // 逃げる道の数が同じだった場合は敵から遠ざかる方に逃げる
            if (route_count == most_node_route)
            {
                var candidate_pos = link_nodes[i].transform.position;
                var candidate_distance = candidate_pos - _targetNode.transform.position;

                var best_pos = link_nodes[select_node_num].transform.position;
                var best_distance = best_pos - _targetNode.transform.position;

                if (best_distance.magnitude < candidate_distance.magnitude)
                {
                    select_node_num = i;
                    continue;
                }
            }

            // 最も逃げ道が多いものを選ぶ
            if (route_count > most_node_route)
            {
                most_node_route = route_count;
                select_node_num = i;
            }
        }

        if (select_node_num > -1 && most_node_route > -1)
            return link_nodes[select_node_num];
        return _currentNode;
    }

    int SearchNumOfEscapeRoutes(Node current_node)
    {
        // 道の数を数える
        _routeCount++;

        foreach (var node in current_node.LinkNodes)
        {
            // 検索済みは飛ばし
            if (node.GetComponent<NodeGuide>().NextPathCheck(gameObject))
                continue;
            // 敵がいるノードに到達した場合はその先の検索をやめる
            if (node == _targetNode)
                return 0;
            // 壁が来た場合はその先の検索をやめる
            if (node.GetComponent<Wall>() != null)
                return 0;

            var nodeguide = current_node.GetComponent<NodeGuide>();
            nodeguide.AddNextPath(gameObject, node);

            int count = SearchNumOfEscapeRoutes(node);
            if (count == 0)
                return 0;
        }
        return 0;
    }

    protected override void NextNodeSearch()
    {
        _isEscape = RunAway();
    }

    void Update()
    {
        if (_isEscape)
        {
            _roadPathManager.RoadGuideReset(gameObject);
            SearchMoveStart();
        }
    }
}
