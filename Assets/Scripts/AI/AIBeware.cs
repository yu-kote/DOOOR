using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AIBeware : MonoBehaviour
{
    private RoadPathManager _roadPathManager;

    private Node _currentNode;

    [SerializeField]
    private int _searchLimit = 5;
    public int SearchLimit { get { return _searchLimit; } set { _searchLimit = value; } }

    private int _searchCount;

    void Start()
    {
        var field = GameObject.Find("Field");
        _roadPathManager = field.GetComponent<RoadPathManager>();

        StartCoroutine(Search());
    }

    private IEnumerator Search()
    {
        while (true)
        {
            yield return null;
            AIBasicsMovement movement = null;
            if (GetComponent<AISearchMove>())
                movement = GetComponent<AISearchMove>();
            if (GetComponent<AITargetMove>())
                movement = GetComponent<AITargetMove>();
            if (movement)
            {
                if (movement.MoveComplete() == false)
                    continue;
            }

            var find_humans = SearchHuman(GetComponent<AIController>().CurrentNode);
            if (find_humans != null)
            {
                var find_human_node = find_humans.First().GetComponent<AIController>().CurrentNode;
                if (gameObject.tag == "Killer")
                {
                    if (GetComponent<AITargetMove>() == null)
                    {
                        var mover = gameObject.AddComponent<AITargetMove>();
                        mover.SetTargetNode(find_human_node);
                        mover.Speed = 1.5f;
                    }
                    if (GetComponent<AISearchMove>())
                        Destroy(GetComponent<AISearchMove>());
                }
                if (gameObject.tag == "Victim")
                {
                    if (GetComponent<AIRunAway>() == null)
                    {
                        var mover = gameObject.AddComponent<AIRunAway>();
                        mover.SetTargetNode(find_human_node);
                        mover.Speed = 2f;
                    }
                    if (GetComponent<AISearchMove>())
                        Destroy(GetComponent<AISearchMove>());
                }
            }
            _roadPathManager.AllUnDone();
        }
    }

    List<GameObject> SearchHuman(Node current_node)
    {
        _searchCount++;
        if (_searchCount > _searchLimit)
        {
            _searchCount = 0;
            return null;
        }

        var humans = current_node.gameObject.GetComponent<FootPrint>().HumansOnNode;
        if (!humans.Contains(gameObject) &&
            humans.Count > 0)
        {
            return humans;
        }

        var loadpath = current_node.gameObject.GetComponent<RoadPath>();

        foreach (var node in current_node.LinkNodes)
        {
            // 検索済みは飛ばし
            if (node.gameObject.GetComponent<RoadPath>()._isDone == true)
                continue;
            // 壁は探索しない
            if (node.gameObject.GetComponent<Wall>())
                continue;
            // ほかの階は探索しない
            if (current_node.gameObject.GetComponent<Stairs>() &&
                node.gameObject.GetComponent<Stairs>())
                continue;
            // 角の場合は終了
            if (node.gameObject.GetComponent<Corner>())
                return null;

            loadpath.Add(gameObject, node);
            loadpath._isDone = true;
            var found_human = SearchHuman(node);
            if (found_human != null)
                return found_human;
        }
        _searchCount = 0;
        return null;
    }



    void Update()
    {

    }
}
