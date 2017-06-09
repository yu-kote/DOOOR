using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
using System.Linq;
using UnityEngine.SceneManagement;

public class AIGenerator : MonoBehaviour
{

    [SerializeField]
    private GameObject _view3dCamera;
    public GameObject View3dCamera { get { return _view3dCamera; } }

    private Node _startNode;
    public Node StartNode { get { return _startNode; } }

    public enum VictimType
    {
        WOMAN,
        TALLMAN,
        FAT,
    }

    private GameObject _field;
    private List<GameObject> _humans = new List<GameObject>();
    public List<GameObject> Humans { get { return _humans; } set { _humans = value; } }

    private int _generateCount = 0;

    void Start()
    {
        StartCoroutine(Setup());
    }

    private void StartNodeSetup()
    {
        _field = GameObject.Find("Field");
        _startNode = _field.GetComponent<NodeManager>().StartNode;
    }

    private IEnumerator Setup()
    {
        yield return null;

        StartNodeSetup();
        _view3dCamera = Instantiate(_view3dCamera, transform);

        if (SceneManager.GetSceneByName("Title").name != null)
            TitlePopHuman();
    }

    GameObject CreateHuman(GameObject human)
    {
        var create_human = Instantiate(human, transform);

        var start_node = _startNode;
        var start_pos = start_node.transform.position
                        + new Vector3(0, 0, -5);

        create_human.transform.position = start_pos;
        create_human.transform.position += new Vector3(0, create_human.transform.localScale.y, 0);

        var my_number = create_human.GetComponent<MyNumber>();
        my_number.Number = _generateCount;

        start_node.GetComponent<FootPrint>().StepIn(create_human);

        _humans.Add(create_human);

        _generateCount++;
        return create_human;
    }

    public string GetVictimName(VictimType type)
    {
        switch (type)
        {
            case VictimType.WOMAN:
                return "Woman";
            case VictimType.TALLMAN:
                return "TallMan";
            case VictimType.FAT:
                return "Fat";
        }
        return null;
    }

    public GameObject GetKiller()
    {
        return _humans.First(human => human.tag == "Killer");
    }

    GameObject CreateVictim(string name)
    {
        return CreateHuman(Resources.Load<GameObject>("Prefabs/Human/" + name));
    }

    GameObject CreateKiller()
    {
        return CreateHuman(Resources.Load<GameObject>("Prefabs/Human/Killer"));
    }
    

    private void TitlePopHuman()
    {
        Observable.Timer(TimeSpan.FromSeconds(1.0f)).Subscribe(_ =>
        {
            CreateVictim(GetVictimName(VictimType.WOMAN)).GetComponent<AIBeginMove>().BeginMoveStart();
        }).AddTo(this);
        Observable.Timer(TimeSpan.FromSeconds(4.0f)).Subscribe(_ =>
        {
            CreateVictim(GetVictimName(VictimType.TALLMAN)).GetComponent<AIBeginMove>().BeginMoveStart();
        }).AddTo(this);
        Observable.Timer(TimeSpan.FromSeconds(7.0f)).Subscribe(_ =>
        {
            CreateVictim(GetVictimName(VictimType.FAT)).GetComponent<AIBeginMove>().BeginMoveStart();
        }).AddTo(this);
    }

    public void InstanceHumans(int stage_num)
    {
        OnDestroy();

        StartNodeSetup();
        StartCoroutine(CreateHumans(stage_num));
    }

    private IEnumerator CreateHumans(int stage_num)
    {
        yield return null;
        
        var text = Resources.Load<TextAsset>
            ("PlannerData/MapData/Stage" + stage_num + "/Human");

        var human_data = JsonUtility.FromJson<StageDataJson>(text.text);

        for (int i = 0; i < human_data.woman; i++)
            CreateVictim(GetVictimName(VictimType.WOMAN));
        for (int i = 0; i < human_data.tallman; i++)
            CreateVictim(GetVictimName(VictimType.TALLMAN));
        for (int i = 0; i < human_data.fat; i++)
            CreateVictim(GetVictimName(VictimType.FAT));
    }

    public void MoveStartHumans()
    {
        foreach (var human in _humans)
        {
            human.GetComponent<AIBeginMove>().BeginMoveStart();
        }

        Observable.Timer(TimeSpan.FromSeconds(7.0f)).Subscribe(_ =>
        {
            var killer = CreateKiller();
            killer.GetComponent<AIBeginMove>().BeginMoveStart();
        }).AddTo(gameObject);
    }

    private void OnDestroy()
    {
        foreach (var human in _humans.ToList())
            Destroy(human);
        _humans.Clear();
    }
}
