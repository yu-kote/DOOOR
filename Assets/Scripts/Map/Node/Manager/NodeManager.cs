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
    public int Interval { get { return _interval; } }
    private int _heightInterval = 6;
    public int HeightInterval { get { return _heightInterval; } }


    private Vector2 _victimStartPos = Vector2.zero;
    public Vector2 VictimStartPos
    {
        get { return _victimStartPos; }
        set { _victimStartPos = value; }
    }

    void Awake()
    {
        MapLoader mapLoader = GetComponent<MapLoader>();
        List<string[]> mapDatas = mapLoader._mapDatas;
        NodesInitialize(mapDatas.Count, mapDatas[0].Length);
        NodesLink();
        CreateMap(mapLoader);
    }

    void Start()
    {

    }

    private void CreateMap(MapLoader mapLoader)
    {
        string[] items = mapLoader.items;
        var mapDatas = mapLoader._mapDatas;

        bool[] isSet = new bool[items.Length];
        for (int y = 0; y < _nodes.Count; y++)
        {
            for (int x = 0; x < _nodes[y].Count; x++)
            {
                var node = _nodes[y][x].GetComponent<Node>();

                if (mapLoader.lastKeyPos.x == x && mapLoader.lastKeyPos.y == y)
                    _nodes[y][x].GetComponent<ItemStatus>().AddPutItem((int)ItemType.LASTKEY);

				int mapID = int.Parse(mapDatas[y][x]);
                int randNum = 0;
                if (isSet.Contains(false) && (MapID)mapID == MapID.KYUKEISPACE)
                {
                    do
                    {
                        randNum = UnityEngine.Random.Range(0, items.Length);
                    } while (isSet[randNum] == true);
                    isSet[randNum] = true;
                }

                // 角
                if (IsCorner(x))
                    node.gameObject.AddComponent<Corner>();

                switch ((MapID)mapID)
                {
                    case MapID.FLOOR:
                        break;

                    case MapID.LEFTSTAIRS:

                        {
                            var next_node = _nodes[y - 1][x - 2].GetComponent<Node>();
                            node.Link(next_node);
                            var stairs = node.gameObject.AddComponent<Stairs>();
                            next_node.gameObject.AddComponent<Stairs>().IsInstanceAttribute = false;
                            stairs.DirectionTag = "Left";
                        }
                        break;

                    case MapID.RIGHTSTAIRS:

                        {
                            var next_node = _nodes[y - 1][x + 2].GetComponent<Node>();
                            node.Link(next_node);
                            var stairs = node.gameObject.AddComponent<Stairs>();
                            next_node.gameObject.AddComponent<Stairs>().IsInstanceAttribute = false;
                            stairs.DirectionTag = "Right";
                        }
                        break;

                    case MapID.WALL:

                        var wall = node.gameObject.AddComponent<Wall>();
                        _nodes[y][x].GetComponent<TrapStatus>().CanSetTrapStatus = 0;

                        wall.NodeManager = this;
                        wall.MyNode = node;

                        break;

                    case MapID.LEFTDOOR:

                        node.gameObject.AddComponent<Door>();
                        node.gameObject.GetComponent<Door>().RotateAngle = new Vector3(0, 180, 0);
                        _nodes[y][x].GetComponent<TrapStatus>().CanSetTrapStatus = 0;
                        break;

                    case MapID.RIGHTDOOR:

                        node.gameObject.AddComponent<Door>();
                        _nodes[y][x].GetComponent<TrapStatus>().CanSetTrapStatus = 0;
                        break;

                    case MapID.START:

                        _victimStartPos = new Vector2(node.CellX, node.CellY);
                        break;

                    case MapID.KYUKEISPACE:

                        node.gameObject.AddComponent<Kyukeispace>();
                        _nodes[y][x].GetComponent<TrapStatus>().CanSetTrapStatus = 0;
                        _nodes[y][x].GetComponent<ItemStatus>().AddPutItem(uint.Parse(items[randNum]));

                        break;

                    case MapID.CANBREAKEWALL:

                        node.gameObject.AddComponent<DummyWall>();
                        _nodes[y][x].GetComponent<TrapStatus>().CanSetTrapStatus = 0;
                        break;

                    case MapID.DEGUTI:

                        node.gameObject.AddComponent<Deguti>();
                        _nodes[y][x].GetComponent<TrapStatus>().CanSetTrapStatus = 0;
                        break;
                }
            }
        }
    }

    public void NodesInitialize(int topFloor, int loadNum)
    {
        _topFloor = topFloor;
        _loadNum = loadNum;

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
        for (int y = _topFloor - 1; y >= 0; y--)
        {
            for (int x = 0; x < _loadNum; x++)
            {
                var node = _nodes[y][x].GetComponent<Node>();
                node.CellX = x;
                node.CellY = y;

                var direction = SurfaceDirection(WhichSurfaceNum(x));

                node.transform.position = pos;
                node.transform.Rotate(SurfaceRotation(WhichSurfaceNum(x)));
                pos += new Vector3(_interval * direction.x, 0, _interval * direction.z);
            }
            pos += new Vector3(0, _heightInterval, 0);
        }
    }

    public int WhichSurfaceNum(int x)
    {
        return x / (_loadNum / _surfaceNum);
    }

    private bool IsCorner(int value)
    {
        if (value % ((_loadNum / _surfaceNum)) == 0)
            return true;
        return false;
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

    public Node SearchOnNodeHuman(GameObject human)
    {
        foreach (var y in _nodes)
        {
            foreach (var node in y)
            {
                if (node.GetComponent<FootPrint>().HumansOnNode.Contains(human))
                    return node.GetComponent<Node>();
            }
        }
        return null;
    }

    public Vector3 GetNodesCenterPoint()
    {
        var x = (_loadNum / _surfaceNum) * _interval / 2.0f;
        var y = (_topFloor * _heightInterval) / 2.0f;
        var z = x;
        return new Vector3(x, y, z);
    }

    public Node NearNode(Vector3 position)
    {
        float near_dist = float.MaxValue;
        GameObject target_node = null;

        foreach (var y in _nodes)
        {
            foreach (var node in y)
            {
                var distance = Vector3.Distance(node.transform.position, position);
                if (near_dist > distance)
                {
                    near_dist = distance;
                    target_node = node;
                }
            }
        }
        if (target_node)
            return target_node.GetComponent<Node>();
        return null;
    }

}
