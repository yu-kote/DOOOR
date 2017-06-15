using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.SceneManagement;

public class MapBackgrounds : MonoBehaviour
{
    private NodeManager _nodeManager;

    [SerializeField]
    private GameObject _singleBackground0;
    [SerializeField]
    private GameObject _singleBackground1;

    private bool _switchBackground = false;

    [SerializeField]
    private GameObject _doubleBackground;

    [SerializeField]
    private GameObject _ceiling;
    [SerializeField]
    private GameObject _light;

    private List<GameObject> _backgrounds = new List<GameObject>();
    private List<GameObject> _ceilings = new List<GameObject>();
    private List<GameObject> _lights = new List<GameObject>();

    private Dictionary<string, Material> _backgroundMaterials = new Dictionary<string, Material>();
    public Dictionary<string, Material> BackgroundMaterials { get { return _backgroundMaterials; } set { _backgroundMaterials = value; } }

    private bool _isLightOn;
    public bool IsLightOn { get { return _isLightOn; } set { _isLightOn = value; } }


    public void Start()
    {
        LoadMaterials();
        MaterialInit();
    }

    public void CreateMapBackgrond()
    {
        OnDestroy();

        var field = GameObject.Find("Field");
        _nodeManager = field.GetComponent<NodeManager>();

        _singleBackground0.transform.localScale
            = new Vector3(_nodeManager.Interval, _nodeManager.HeightInterval, 0.01f);
        _singleBackground1.transform.localScale
            = new Vector3(_nodeManager.Interval, _nodeManager.HeightInterval, 0.01f);
        _doubleBackground.transform.localScale
            = new Vector3(_nodeManager.Interval * 2, _nodeManager.HeightInterval, 0.01f);

        Create();
        OnLightSelect();
        LightAllControll();
    }

    private void Create()
    {
        for (int y = 0; y < _nodeManager.Nodes.Count; y++)
        {
            for (int x = 0; x < _nodeManager.Nodes[y].Count; x++)
            {
                CreateCeiling(_nodeManager.Nodes[y][x]);
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
            use_bg = _doubleBackground;
        }
        else
        {
            if (_switchBackground)
                use_bg = _singleBackground0;
            else
                use_bg = _singleBackground1;
            _switchBackground = !_switchBackground;
        }

        var bg = Instantiate(use_bg, node.transform);

        // 親子関係ですでに回転と位置は出してあるのでローカルの値を初期化
        bg.transform.localPosition = Vector3.zero;
        bg.transform.localEulerAngles = Vector3.zero;
        // マテリアルの関係で上下を反転させる必要があるため、反転させる
        bg.transform.localEulerAngles = new Vector3(0, 0, 180);

        var offset_pos = new Vector3(0, _nodeManager.HeightInterval / 2, 0);
        float offset = _nodeManager.Interval / 2.0f;

        // ジャスタウェイ
        OffsetSurface(node.GetComponent<Node>().CellX, ref offset_pos, offset + offset_kyukeispace);

        bg.transform.position += offset_pos;

        _backgrounds.Add(bg);
    }

    int _lightCount = 0;

    private void CreateCeiling(GameObject node)
    {
        if (SceneManager.GetSceneByName("Title").name != null)
            return;

        _lightCount++;

        // 壁とドアを見やすくするため、その隣にライトを設置する
        var light_node = node.GetComponent<Node>().LinkNodes.FirstOrDefault(n => n.GetComponent<Wall>());
        if (light_node == null)
            light_node = node.GetComponent<Node>().LinkNodes.FirstOrDefault(n => n.GetComponent<Door>());

        if (light_node)
            _light.SetActive(true);
        else
            _light.SetActive(false);

        // 壁とドアがない状態が続くと暗くなってしまうので、3つ間隔をあけて配置する
        if (_lightCount > 3)
            _light.SetActive(true);

        // 間隔をあけて配置した場合リセットする
        if (_light.activeSelf == true)
            _lightCount = 0;

        // 壁とドアにライトを設置しない
        if (node.GetComponent<Wall>() || node.GetComponent<Door>())
            _light.SetActive(false);

        var ceiling = Instantiate(_ceiling, node.transform);

        ceiling.transform.localPosition = Vector3.zero;
        ceiling.transform.localEulerAngles = Vector3.zero;

        var offset_pos = new Vector3(0, _nodeManager.HeightInterval - 0.2f, 0);
        float offset = 0.0f;

        OffsetSurface(node.GetComponent<Node>().CellX, ref offset_pos, offset);

        ceiling.transform.position += offset_pos;

        _ceilings.Add(ceiling);

        var light_bulb = ceiling.transform.FindChild("LightBulb").gameObject;
        _lights.Add(light_bulb);
    }

    public void OffsetSurface(int x, ref Vector3 offset_pos, float value)
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

    void LoadMaterials()
    {
        var materials = Resources.LoadAll<Material>("Prefabs/BG/Materials");
        for (int i = 0; i < materials.Count(); i++)
        {
            _backgroundMaterials[materials[i].name] = materials[i];
        }
    }

    void MaterialInit()
    {
        _singleBackground0.GetComponent<MeshRenderer>().material
            = _backgroundMaterials["NormalBackground" + "0"];
        _singleBackground1.GetComponent<MeshRenderer>().material
            = _backgroundMaterials["NormalBackground" + "1"];
        _doubleBackground.GetComponent<MeshRenderer>().material
            = _backgroundMaterials["Room"];
    }

    // ライトをすべてオンオフできる関数
    public void LightAllControll(bool is_on = true)
    {
        _isLightOn = is_on;
        foreach (var light in _lights)
            light.SetActive(is_on);
    }

    void OnLightSelect()
    {
        var lights = _lights.Where(l => l.activeInHierarchy == true).ToList();
        _lights.Clear();
        _lights = lights;
    }

    private void OnDestroy()
    {
        foreach (var item in _backgrounds)
            Destroy(item);
        _backgrounds.Clear();
        _backgroundMaterials.Clear();
        _lights.Clear();
    }
}
