using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AIBeware : MonoBehaviour
{
    private RoadPathManager _roadPathManager;

    [SerializeField]
    private int _searchLimit = 5;
    public int SearchLimit { get { return _searchLimit; } set { _searchLimit = value; } }

    private int _searchCount;
    //private bool _isFindHuman = false;
    private GameObject _targetHuman;

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

            if (GetComponent<AISearchMove>() == null)
                continue;
            if (_targetHuman == null)
            {
                var find_humans = SearchHuman(GetComponent<AIController>().CurrentNode);
                _roadPathManager.RoadPathReset(gameObject);
                if (find_humans != null &&
                    find_humans.First() != null)
                {
                    var find_human = find_humans.First();
                    _targetHuman = find_human;
                }
            }
            if (GetComponent<AIController>().GetMovement().MoveComplete() == false)
                continue;
            if (_targetHuman == null)
                continue;

            if (gameObject.tag == "Killer")
            {
                if (GetComponent<AITargetMove>() == null)
                {
                    var mover = gameObject.AddComponent<AITargetMove>();
                    mover.SetTargetNode(_targetHuman.GetComponent<AIController>().CurrentNode);
                    mover.Speed = GetComponent<AIController>().HurryUpSpeed;
                    if (GetComponent<AISearchMove>())
                        Destroy(GetComponent<AISearchMove>());
                }
            }
            if (gameObject.tag == "Victim")
            {
                if (GetComponent<AIRunAway>() == null)
                {
                    var mover = gameObject.AddComponent<AIRunAway>();
                    mover.SetTargetNode(_targetHuman);
                    mover.Speed = GetComponent<AIController>().HurryUpSpeed;
                    if (GetComponent<AISearchMove>())
                        Destroy(GetComponent<AISearchMove>());
                }
            }
            _targetHuman = null;
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
            foreach (var human in humans)
            {
                if (human == null) continue;
                if (gameObject.tag == "Victim")
                    if (human.tag == "Victim")
                        continue;
                return humans;
            }
        }

        var loadpath = current_node.gameObject.GetComponent<RoadPath>();

        foreach (var node in current_node.LinkNodes)
        {
            // 検索済みは飛ばし
            if (node.gameObject.GetComponent<RoadPath>().PathCheck(gameObject) == true)
                continue;
            // 壁は探索しない
            if (node.gameObject.GetComponent<Wall>())
                continue;
            // ほかの階は探索しない
            if (current_node.gameObject.GetComponent<Stairs>() &&
                node.gameObject.GetComponent<Stairs>())
            {
                var stairs_humans = node.gameObject.GetComponent<FootPrint>().HumansOnNode;
                if (!stairs_humans.Contains(gameObject) &&
                     stairs_humans.Count > 0)
                    return stairs_humans;
                continue;
            }
            // 角の場合は終了
            if (node.gameObject.GetComponent<Corner>())
                return null;

            loadpath.Add(gameObject, node);
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
