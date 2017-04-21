using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AIRunAway : AIBasicsMovement
{
    private RoadPathManager _roadPathManager;

    private float _endDistance = 20;
    public float EndDistance { get { return _endDistance; } set { _endDistance = value; } }

    private GameObject _targetHuman;
    public GameObject TargetNode { get { return _targetHuman; } set { _targetHuman = value; } }
    public void SetTargetNode(GameObject target_node) { _targetHuman = target_node; }

    private bool _isEscape = false;
    private int _routeCount;


    void Start()
    {
        var field = GameObject.Find("Field");
        _roadPathManager = field.GetComponent<RoadPathManager>();
        _nodeController = field.GetComponent<NodeController>();

        MoveSetup();
    }

    public override void MoveSetup()
    {
        _currentNode = GetComponent<AIController>().CurrentNode;
        _isEscape = false;

        MoveReset();
    }

    bool RunAway()
    {
        if (_isEscape) return true;
        if (_targetHuman == null) return true;
        //if (_targetHuman.GetComponent<AITargetMove>() == null) return true;
        var vec = _targetHuman.transform.position - _currentNode.transform.position;
        var distance = vec.magnitude;

        // 逃げ切ったら終わり
        if (distance > _endDistance)
            return true;

        if (_currentNode.GetComponent<Stairs>())
        {
            StairsPoint();
        }
        else
        {
            GoAway();
        }
        return false;
    }

    void GoAway()
    {
        // 離れる方のノードに逃げるため、短い順にソートしてLastを選ぶ
        AITargetMove.SortByNodeLength(
            _targetHuman.GetComponent<AIController>().CurrentNode,
            _currentNode.LinkNodes);

        var next_candidate_node = _currentNode.LinkNodes.Last();

        // 壁かどうか
        if (next_candidate_node.GetComponent<Wall>() != null)
            return;

        // ドアの鍵が閉まっているかどうか
        if (IsDoorLock(next_candidate_node))
            return;

        _nextNode = _currentNode.LinkNodes.Last();
    }

    // 階段が来たらどっちに進めば壁がないか調べる
    void StairsPoint()
    {
        // つながっているノードを見る
        // ある方向に逃げた場合どのぐらいの距離逃げることが出来るのかを割り出す
        // それを比べて、逃げるノードを選択する

        int select_node_num = -1;
        int most_node_route = -1;
        for (int i = 0; i < _currentNode.LinkNodes.Count; i++)
        {
            var roadpath = _currentNode.GetComponent<RoadPath>();
            roadpath.Add(gameObject, _currentNode);

            int route_count = 0;

            SearchNumOfEscapeRoutes(_currentNode.LinkNodes[i], route_count);

            route_count = _routeCount;
            _routeCount = 0;
            //Debug.Log(i + "本目の" + "逃げ道の数:" + route_count);

            _roadPathManager.RoadPathReset(gameObject);


            // 逃げる道の数が同じだった場合は敵から遠ざかる方に逃げる
            if (route_count == most_node_route)
            {
                var candidate_pos = _currentNode.LinkNodes[i].transform.position;
                var candidate_distance = candidate_pos - _targetHuman.transform.position;

                var best_pos = _currentNode.LinkNodes[select_node_num].transform.position;
                var best_distance = best_pos - _targetHuman.transform.position;

                if (best_distance.magnitude > candidate_distance.magnitude)
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

        var next_candidate_node = _currentNode.LinkNodes[select_node_num];
        // ドアの鍵が閉まっているかどうか
        if (IsDoorLock(next_candidate_node))
            return;

        if (select_node_num > -1 &&
            next_candidate_node.GetComponent<Wall>() == null)
        {
            _nextNode = _currentNode.LinkNodes[select_node_num];
        }
    }

    int SearchNumOfEscapeRoutes(Node current_node, int route_count)
    {
        _routeCount++;
        var roadpath = current_node.GetComponent<RoadPath>();

        foreach (var node in current_node.LinkNodes)
        {
            if (node.GetComponent<RoadPath>().PathCheck(gameObject))
                continue;
            if (node == _targetHuman.GetComponent<AIController>().CurrentNode)
                return 0;
            if (node.GetComponent<Wall>() != null)
                return 0;

            roadpath.Add(gameObject, node);
            int count = SearchNumOfEscapeRoutes(node, route_count);
            if (count == 0)
                return 0;
        }
        return route_count;
    }

    bool IsDoorLock(Node node)
    {
        var door = node.GetComponent<Door>();
        if (door)
        {
            if (door.IsDoorLock())
            {
                Debug.Log("通れません");
                return true;
            }
        }
        return false;
    }

    protected override void NextNodeSearch()
    {
        _isEscape = RunAway();
    }

    void Update()
    {
        if (MoveComplete() && _isEscape)
        {
            _roadPathManager.RoadPathReset(gameObject);
            gameObject.AddComponent<AISearchMove>();
            Destroy(this);
        }
    }
}
