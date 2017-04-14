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

    public void RoadPathReset(GameObject human)
    {
        foreach (var y in _nodeManager.Nodes)
        {
            foreach (var node in y)
            {
                var roadpath = node.GetComponent<RoadPath>();
                roadpath.Remove(human);
                roadpath._isDone = false;
            }
        }
    }

    public void AllUnDone()
    {
        foreach (var y in _nodeManager.Nodes)
        {
            foreach (var node in y)
            {
                var roadpath = node.GetComponent<RoadPath>();
                roadpath._isDone = false;
            }
        }
    }

    public void Add(GameObject human, Node node)
    {
        var roadpath = node.GetComponent<RoadPath>();
        roadpath.Add(human, node);
    }

    public void RemoveRoadPath(GameObject human)
    {
        foreach (var y in _nodeManager.Nodes)
        {
            foreach (var node in y)
            {
                var roadpath = node.GetComponent<RoadPath>();
                roadpath.Remove(human);
            }
        }
    }

}
