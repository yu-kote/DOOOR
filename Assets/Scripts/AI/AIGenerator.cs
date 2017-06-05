using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
using System.Linq;

public class AIGenerator : MonoBehaviour
{
    [SerializeField]
    private int _victimCount;
    [SerializeField]
    private int _killerCount;

    [SerializeField]
    private GameObject _view3dCamera;
    public GameObject View3dCamera { get { return _view3dCamera; } }

    private Node _startNode;

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
        _field = GameObject.Find("Field");
        _startNode = _field.GetComponent<NodeManager>().StartNode;

        _view3dCamera = Instantiate(_view3dCamera, transform);

        Observable.Timer(TimeSpan.FromSeconds(0.5f)).Subscribe(_ =>
        {
            CreateVictim(GetVictimName(VictimType.WOMAN));
            CreateVictim(GetVictimName(VictimType.TALLMAN));
            CreateVictim(GetVictimName(VictimType.FAT));
        }).AddTo(gameObject);

        Observable.Timer(TimeSpan.FromSeconds(3.5f)).Subscribe(_ =>
        {
            for (int i = 0; i < _killerCount; i++)
                CreateKiller();
        }).AddTo(gameObject);
    }

    void CreateHuman(GameObject human)
    {
        human = Instantiate(human, transform);

        var start_node = _startNode;
        var start_pos = start_node.transform.position;

        human.transform.position = start_pos;
        human.transform.position += new Vector3(0, human.transform.localScale.y, 0);

        var my_number = human.GetComponent<MyNumber>();
        my_number.Number = _generateCount;

        start_node.GetComponent<FootPrint>().StepIn(human);

        _humans.Add(human);

        _generateCount++;
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

    void CreateVictim(string name)
    {
        CreateHuman(Resources.Load<GameObject>("Prefabs/Human/" + name));
    }

    void CreateKiller()
    {
        CreateHuman(Resources.Load<GameObject>("Prefabs/Human/Killer"));
    }

    private void OnDestroy()
    {
        foreach (var human in _humans)
            Destroy(human);
        _humans.Clear();
    }
}
