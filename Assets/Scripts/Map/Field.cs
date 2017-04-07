using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Field : MonoBehaviour
{
    [SerializeField]
    private GameObject _node;

    // 面の、高さの、横
    private List<List<GameObject>> _nodes = new List<List<GameObject>>();

    public List<List<GameObject>> Nodes { get { return _nodes; } set { _nodes = value; } }

    private int _topFloor = 3;

    private int _loadNum = 40;


    private int _surfaceNum = 4;
    private int _interval = 3;
    private int _heightInterval = 6;

    void Start()
    {
        NodesInitialize();
        NodesLink();
    }

    private void NodesInitialize()
    {
        var pos = new Vector3();

        for (int y = 0; y < _topFloor; y++)
        {
            List<GameObject> floor = new List<GameObject>();
            var surface_num = 0;
            for (int x = 0; x < _loadNum; x++)
            {
                var node = _node;

                var direction = SurfaceDirection(surface_num);
                pos += new Vector3(_interval * direction.x, 0, _interval * direction.z);

                node.transform.position = pos;

                floor.Add(Instantiate(node, transform));

                // 面のカウントを増やす
                if (x % (_loadNum / _surfaceNum) == (_loadNum / _surfaceNum) - 1)
                    surface_num++;
            }
            pos += new Vector3(0, _heightInterval, 0);
            _nodes.Add(floor);
            //floor.Clear();
        }
    }

    private Vector3 SurfaceDirection(int surface_num)
    {
        if (surface_num == 0)
            return new Vector3(1, 1, 0);
        if (surface_num == 1)
            return new Vector3(0, 1, 1);
        if (surface_num == 2)
            return new Vector3(-1, 1, 0);
        if (surface_num == 3)
            return new Vector3(0, 1, -1);
        return Vector3.zero;
    }

    private void NodesLink()
    {
        for (int y = 0; y < _nodes.Count; y++)
        {
            for (int x = 0; x < _nodes[y].Count; x++)
            {
                var node = _nodes[y][x].GetComponent<Node>();

                if (x < _loadNum - 1)
                {
                    var next_node = _nodes[y][x + 1].GetComponent<Node>();
                    node.Link(next_node);
                }
                else
                {
                    var next_node = _nodes[y][0].GetComponent<Node>();
                    node.Link(next_node);
                }

                // 階段お試し
                if (x % 6 == 0 && y < _nodes.Count - 1)
                {
                    var next_node = _nodes[y + 1][x + 2].GetComponent<Node>();
                    node.Link(next_node);
                }
            }
        }
    }

    void Update()
    {

    }
}
