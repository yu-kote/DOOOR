using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Linq;
using System;

public class NodeManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _node;

    // 高さの、横
    private List<List<GameObject>> _nodes = new List<List<GameObject>>();
    public List<List<GameObject>> Nodes { get { return _nodes; } set { _nodes = value; } }

    private int _topFloor = 3;

    private int _loadNum = 60;

    private int _surfaceNum = 4;
    private int _interval = 3;
    private int _heightInterval = 6;

    void Awake()
    {
        NodesInitialize();
        NodesLink();
        AttributeTest();

        Debug.Log(GetNodesCenterPoint());
    }

    private void NodesInitialize()
    {
        // nodeのインスタンス
        for (int y = 0; y < _topFloor; y++)
        {
            List<GameObject> floor = new List<GameObject>();
            for (int x = 0; x < _loadNum; x++)
            {
                floor.Add(Instantiate(_node, transform));
            }
            _nodes.Add(floor);
        }

        var pos = new Vector3();

        // プレハブが直接いじられてしまうので、別枠でfor文を回す
        for (int y = 0; y < _topFloor; y++)
        {
            for (int x = 0; x < _loadNum; x++)
            {
                var node = _nodes[y][x].GetComponent<Node>();

                var direction = SurfaceDirection(WhichSurfaceNum(x));

                node.transform.position = pos;
                node.transform.Rotate(SurfaceRotation(WhichSurfaceNum(x)));
                pos += new Vector3(_interval * direction.x, 0, _interval * direction.z);
            }
            pos += new Vector3(0, _heightInterval, 0);
        }
    }

    private int WhichSurfaceNum(int x)
    {
        return x / (_loadNum / _surfaceNum);
    }

    private bool IsCorner(int value)
    {
        return value % (_loadNum / _surfaceNum) == (_loadNum / _surfaceNum) - 1;
    }

    public Vector3 SurfaceDirection(int surface_num)
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

    public Vector3 SurfaceRotation(int surface_num)
    {
        if (surface_num == 0)
            return new Vector3(0, 0, 0);
        if (surface_num == 1)
            return new Vector3(0, -90, 0);
        if (surface_num == 2)
            return new Vector3(0, -180, 0);
        if (surface_num == 3)
            return new Vector3(0, -270, 0);
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
            }
        }
    }

    /// <summary>
    /// 障害物を試しにおいてみる
    /// </summary>
    private void AttributeTest()
    {
        for (int y = 0; y < _nodes.Count; y++)
        {
            for (int x = 0; x < _nodes[y].Count; x++)
            {
                var node = _nodes[y][x].GetComponent<Node>();

                if (IsCorner(x))
                {
                    node.gameObject.AddComponent<Corner>();
                    continue;
                }
                if (AttributeRandom())
                    node.gameObject.AddComponent<Wall>();

                // 階段お試し
                if (x % 7 == 0 && y < _nodes.Count - 1)
                {
                    var next_node = _nodes[y + 1][x + 2].GetComponent<Node>();
                    node.Link(next_node);
                    node.gameObject.AddComponent<Stairs>();
                    next_node.gameObject.AddComponent<Stairs>();
                }
            }
        }
    }

    private bool AttributeRandom(int max = 100)
    {
        var r = UnityEngine.Random.Range(0, max);
        return r < 10;
    }

    public Vector3 GetNodesCenterPoint()
    {
        var x = (_loadNum / _surfaceNum) * _interval / 2.0f;
        var y = (_topFloor * _heightInterval) / 2.0f;
        var z = x;
        return new Vector3(x, y, z);
    }

}
