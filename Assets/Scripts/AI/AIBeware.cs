using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBeware : MonoBehaviour
{
    private RoadPathManager _roadPathManager;
    private NodeManager _nodeManager;

    [SerializeField]
    private int _searchLimit = 5;
    private int _searchCount;

    void Start()
    {
        var field = GameObject.Find("Field");
        _roadPathManager = field.GetComponent<RoadPathManager>();
        _nodeManager = field.GetComponent<NodeManager>();

        StartCoroutine(Search());
    }

    private IEnumerator Search()
    {
        while (true)
        {
            var find_humans = SearchHuman(GetComponent<AIController>().CurrentNode);
            if (find_humans != null)
            {
                //find_humans[0].gameObject.transform.position += new Vector3(1, 1, 1);
            }
            _roadPathManager.AllUnDone();
            yield return null;
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
            Debug.Log("look human");
            return humans;
        }

        var loadpath = current_node.gameObject.GetComponent<RoadPath>();

        foreach (var node in current_node.LinkNodes)
        {
            // 検索済みは飛ばし
            if (node.gameObject.GetComponent<RoadPath>()._isDone == true)
                continue;
            // 壁は探索しない
            if (node.gameObject.GetComponent<Wall>() != null)
                continue;
            // ほかの階は探索しない
            if (current_node.gameObject.GetComponent<Stairs>() != null &&
                node.gameObject.GetComponent<Stairs>() != null)
                continue;
            // 角の場合は終了
            if (node.gameObject.GetComponent<Corner>() != null)
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
