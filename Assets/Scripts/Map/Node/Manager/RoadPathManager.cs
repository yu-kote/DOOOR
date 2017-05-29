using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadPathManager : MonoBehaviour
{
    private NodeManager _nodeManager;

    void Start()
    {
        _nodeManager = GetComponent<NodeManager>();
    }

    /// <summary>
    /// ルート検索のために使用した目印を全て消去する
    /// </summary>
    public void RoadGuideReset(GameObject human)
    {
        foreach (var y in _nodeManager.Nodes)
        {
            foreach (var node in y)
            {
                if (node == null)
                    continue;
                var roadpath = node.GetComponent<NodeGuide>();
                roadpath.NextPathRemove(human);
                roadpath.PrevPathRemove(human);
            }
        }
    }

    /// <summary>
    /// 標的の検索のために使用した目印を全て消去する
    /// </summary>
    public void SearchReset(GameObject human)
    {
        foreach (var y in _nodeManager.Nodes)
        {
            foreach (var node in y)
            {
                if (node == null)
                    continue;
                var roadpath = node.GetComponent<NodeGuide>();
                roadpath.SearchRemove(human);
            }
        }
    }


}
