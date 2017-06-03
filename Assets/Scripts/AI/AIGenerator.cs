using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

public class AIGenerator : MonoBehaviour
{
    [SerializeField]
    private int _victimCount;
    [SerializeField]
    private int _killerCount;

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

        var node_manager = _field.GetComponent<NodeManager>();

        var start_node = node_manager.Nodes[0][0];
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
