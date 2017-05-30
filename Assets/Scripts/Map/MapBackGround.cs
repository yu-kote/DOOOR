using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class MapBackground : MonoBehaviour
{
    private NodeManager _nodeManager;

    [SerializeField]
    private GameObject _singleBackground;
    [SerializeField]
    private GameObject _doubleBackground;

    private List<GameObject> _backgrounds = new List<GameObject>();

    private Dictionary<string, Material> _backgroundMaterials = new Dictionary<string, Material>();
    public Dictionary<string, Material> BackgroundMaterials { get { return _backgroundMaterials; } set { _backgroundMaterials = value; } }

    void Start()
    {
        var field = GameObject.Find("Field");
        _nodeManager = field.GetComponent<NodeManager>();

        _singleBackground.transform.localScale
            = new Vector3(_nodeManager.Interval, _nodeManager.HeightInterval, 0);
        _doubleBackground.transform.localScale
            = new Vector3(_nodeManager.Interval * 2, _nodeManager.HeightInterval, 0);

        LoadMaterials();
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
                CreateBackground(_nodeManager.Nodes[y][x]);
            }
        }
    }

    void CreateBackground(GameObject node)
    {
        GameObject use_bg;

        // 部屋だけ2マスぶんの背景を使う
        var offset_kyukeispace = 0.0f;
        if (node.GetComponent<Kyukeispace>())
        {
            offset_kyukeispace = -0.1f;
            BackgroundMaterialChange("Room");
            use_bg = _doubleBackground;
        }
        else
        {
            BackgroundMaterialChange("NormalBackground");
            use_bg = _singleBackground;
        }
        
        var bg = Instantiate(use_bg, node.transform);

        // 親子関係ですでに回転と位置は出してあるのでローカルの値を初期化
        bg.transform.localPosition = Vector3.zero;
        bg.transform.localEulerAngles = Vector3.zero;

        var offset_pos = new Vector3(0, _nodeManager.HeightInterval / 2, 0);
        float offset = _nodeManager.Interval / 2.0f;

        // ジャスタウェイ
        OffsetSurface(node.GetComponent<Node>().CellX, ref offset_pos, offset);
        OffsetSurface(node.GetComponent<Node>().CellX, ref offset_pos, offset_kyukeispace);

        bg.transform.position += offset_pos;

        _backgrounds.Add(bg);
        
        // 階段があったら階段のテクスチャを生成します
        CreateStairs(node.GetComponent<Node>());
    }

    void CreateStairs(Node node)
    {
        if (node.GetComponent<Stairs>() == null)
            return;

        Node stairs_node = null;
        stairs_node = node.LinkNodes.FirstOrDefault(n => n.GetComponent<Stairs>());

        var direction = node.transform.position - stairs_node.transform.position;
        // 上への階段以外ははじく
        if (direction.y < 0)
            return;

        var offset = _nodeManager.SurfaceDirection(node.CellX) * _nodeManager.Interval;


    }

    void OffsetSurface(int x, ref Vector3 offset_pos, float value)
    {
        if (_nodeManager.WhichSurfaceNum(x) == 0)
            offset_pos += new Vector3(0, 0, value);
        if (_nodeManager.WhichSurfaceNum(x) == 1)
            offset_pos += new Vector3(-value, 0, 0);
        if (_nodeManager.WhichSurfaceNum(x) == 2)
            offset_pos += new Vector3(0, 0, -value);
        if (_nodeManager.WhichSurfaceNum(x) == 3)
            offset_pos += new Vector3(value, 0, 0);
    }

    /// <summary>
    /// TODO:背景の識別がintなのでstringに修正予定
    /// </summary>
    void BackgroundMaterialChange(string name)
    {
        _singleBackground.GetComponent<MeshRenderer>().material
            = _backgroundMaterials[name];
        _doubleBackground.GetComponent<MeshRenderer>().material
            = _backgroundMaterials[name];
    }

    void LoadMaterials()
    {
        var materials = Resources.LoadAll<Material>("Prefabs/BG/Materials");
        for (int i = 0; i < materials.Count(); i++)
        {
            _backgroundMaterials.Add(materials[i].name, materials[i]);
        }
    }
}
