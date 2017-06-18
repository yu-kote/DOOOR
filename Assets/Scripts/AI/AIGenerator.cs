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

    [SerializeField]
    HumanList _humanBoardlist;

    private Node _startNode;
    public Node StartNode { get { return _startNode; } }

    private Node _killerStartNode;

    public enum VictimType
    {
        WOMAN,
        TALLMAN,
        FAT,
    }

    private GameObject _field;
    private List<GameObject> _humans = new List<GameObject>();
    public List<GameObject> Humans { get { return _humans; } set { _humans = value; } }

    private bool _isStop;

    private int _generateCount = 0;

    void Start()
    {
        StartCoroutine(Setup());
    }

    private void StartNodeSetup()
    {
        _field = GameObject.Find("Field");
        _startNode = _field.GetComponent<NodeManager>().StartNode;
        _killerStartNode = _field.GetComponent<NodeManager>().Nodes[0][6].GetComponent<Node>();
    }

    public void KillerPopNodeCell(int x, int y)
    {
        _killerStartNode = _field.GetComponent<NodeManager>().Nodes[y][x].GetComponent<Node>();
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
        // 殺人鬼は指定されたノードに出す
        if (human.tag == "Killer")
            start_node = _killerStartNode;

        // 最初は探索者の位置を少し後ろに配置する
        var start_pos = start_node.transform.position;
        if (human.tag == "Victim")
            start_pos += new Vector3(0, 0, -5);

        // 探索者の位置をばらけさせる
        var r = UnityEngine.Random.Range(-2, 2);
        start_pos += new Vector3(r, 0, r);

        // 初期値が地面に接するようにするため半分浮かせる
        create_human.transform.position = start_pos;
        create_human.transform.position += new Vector3(0, create_human.transform.localScale.y, 0);

        var my_number = create_human.GetComponent<MyNumber>();
        my_number.Number = _generateCount;

        start_node.GetComponent<FootPrint>().StepIn(create_human);

        _humans.Add(create_human);

        if (_humanBoardlist)
            _humanBoardlist.HumanBoardInstantiate(create_human);

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
        Observable.Timer(TimeSpan.FromSeconds(5.0f)).Subscribe(_ =>
        {
            CreateVictim(GetVictimName(VictimType.TALLMAN)).GetComponent<AIBeginMove>().BeginMoveStart();
        }).AddTo(this);
        Observable.Timer(TimeSpan.FromSeconds(10.0f)).Subscribe(_ =>
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


        ShareData.instance.Reset();
        ShareData.instance.WomanCount = human_data.woman;
        ShareData.instance.TallManCount = human_data.tallman;
        ShareData.instance.FatCount = human_data.fat;

        if (_humanBoardlist)
            _humanBoardlist.HumanItemSetup();
    }

    public void MoveStartHumans()
    {
        foreach (var human in _humans)
            human.GetComponent<AIBeginMove>().BeginMoveStart();

        Observable.Timer(TimeSpan.FromSeconds(3.5f)).Subscribe(_ =>
        {
            var killer = CreateKiller();
            killer.GetComponent<AIBeginMove>().BeginMoveStart();
        }).AddTo(gameObject);
    }

    // 全ての人間の動きを止め続ける
    public void MoveEndHumans()
    {
        foreach (var human in _humans)
            StartCoroutine(MoveStop(human, false));
    }

    // 全ての人間の動きを動かすかどうか決める
    public void HumanMoveControll(bool can_move)
    {
        if (can_move == false)
        {
            _isStop = true;
            foreach (var human in _humans)
                StartCoroutine(MoveStop(human));
        }
        if (can_move)
            _isStop = false;
    }

    private IEnumerator MoveStop(GameObject human, bool can_move)
    {
        while (true)
        {
            human.GetComponent<AIController>().GetMovement().CanMove = can_move;
            yield return null;
        }
    }

    private IEnumerator MoveStop(GameObject human)
    {
        while (true)
        {
            human.GetComponent<AIController>().GetMovement().CanMove = false;

            yield return null;
            if (_isStop == false)
            {
                yield return null;
                break;
            }
        }
        human.GetComponent<AIController>().GetMovement().CanMove = true;
        _isStop = true;
    }


    private void OnDestroy()
    {
        foreach (var human in _humans.ToList())
            Destroy(human);
        _humans.Clear();

        if (_humanBoardlist)
            _humanBoardlist.OnDestroy();
    }

    public GameObject GetHumanFromMyNumber(int number)
    {
        var target = _humans.FirstOrDefault(human =>
                            human.GetComponent<MyNumber>().Number == number);
        return target;
    }
}
