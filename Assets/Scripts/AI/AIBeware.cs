using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBeware : MonoBehaviour
{
    private NodeManager _nodeManager;

    [SerializeField]
    private int _searchLimit = 5;
    private int _searchCount;

    void Start()
    {
        var field = GameObject.Find("Field");
        _nodeManager = field.GetComponent<NodeManager>();

        StartCoroutine(Search());
    }

    private IEnumerator Search()
    {
        while (true)
        {
            var find_humans = SearchHuman(GetComponent<AIController>().CurrentNode);
            Debug.Log(find_humans.Count);
            yield return new WaitForSeconds(2);
        }
    }

    List<GameObject> SearchHuman(Node current_node)
    {
        _searchCount++;
        //if (_searchCount > _searchLimit)
        //{
        // _searchCount = 0;
        //return null;
        //}
        
        var humans = current_node.gameObject.GetComponent<FootPrint>().HumansOnNode;
        if (humans.Count > 0)
        {
            _searchCount = 0;
            return humans;
        }

        foreach (var node in current_node.LinkNodes)
        {
            // 壁は探索しない
            if (node.gameObject.GetComponent<Wall>() != null)
                continue;

            // ほかの階は探索しない
            if (current_node.gameObject.GetComponent<Stairs>() != null &&
                node.gameObject.GetComponent<Stairs>() != null)
                continue;
            if (SearchHuman(node) == null)
            {
                _searchCount = 0;
                return null;
            }
        }
        return null;
    }



    void Update()
    {

    }
}
