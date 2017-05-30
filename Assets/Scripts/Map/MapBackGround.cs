using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class MapBackGround : MonoBehaviour
{
    private NodeManager _nodeManager;

    [SerializeField]
    private GameObject _singleBackGround;
    [SerializeField]
    private GameObject _doubleBackGround;

    private List<GameObject> _backGrounds = new List<GameObject>();

    void Start()
    {
        var field = GameObject.Find("Field");
        _nodeManager = field.GetComponent<NodeManager>();

        _singleBackGround.transform.localScale
            = new Vector3(_nodeManager.Interval, _nodeManager.HeightInterval, 0);
        _doubleBackGround.transform.localScale
            = new Vector3(_nodeManager.Interval * 2, _nodeManager.HeightInterval, 0);

        StartCoroutine(Create());
    }

    private IEnumerator Create()
    {
        yield return null;
        for (int y = 0; y < _nodeManager.Nodes.Count; y++)
        {
            for (int x = 0; x < _nodeManager.Nodes[y].Count; x++)
            {
                if (_nodeManager.Nodes[y][x].GetComponent<Corner>())
                    continue;
                CreateBackGround(_nodeManager.Nodes[y][x]);
            }
        }
    }

    void CreateBackGround(GameObject node)
    {
        var sprites = Resources.LoadAll<Sprite>("Texture/MapTexture");
        var bg = Instantiate(_singleBackGround, node.transform);

        bg.transform.localPosition = Vector3.zero;
        bg.transform.localEulerAngles = Vector3.zero;

        var offset_pos = new Vector3(0, _nodeManager.HeightInterval / 2, 0);

        float offset = _nodeManager.Interval / 2.0f;

        if (_nodeManager.WhichSurfaceNum(node.GetComponent<Node>().CellX) == 0)
            offset_pos += new Vector3(0, 0, offset);
        if (_nodeManager.WhichSurfaceNum(node.GetComponent<Node>().CellX) == 1)
            offset_pos += new Vector3(-offset, 0, 0);
        if (_nodeManager.WhichSurfaceNum(node.GetComponent<Node>().CellX) == 2)
            offset_pos += new Vector3(0, 0, -offset);
        if (_nodeManager.WhichSurfaceNum(node.GetComponent<Node>().CellX) == 3)
            offset_pos += new Vector3(offset, 0, 0);

        bg.transform.position += offset_pos;

        _backGrounds.Add(bg);
    }
}
