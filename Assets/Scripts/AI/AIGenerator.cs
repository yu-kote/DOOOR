using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

public class AIGenerator : MonoBehaviour
{
    private GameObject _field;

    private int _generateCount = 0;

    void Start()
    {
        _field = GameObject.Find("Field");
        for (int i = 0; i < 1; i++)
            CreateVictim();

        Observable.Timer(TimeSpan.FromSeconds(3)).Subscribe(_ =>
        {
            CreateKiller();
        }).AddTo(this);
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

        _generateCount++;
    }

    void CreateVictim()
    {
        CreateHuman(Resources.Load<GameObject>("Prefabs/Human/Victim"));
    }

    void CreateKiller()
    {
        CreateHuman(Resources.Load<GameObject>("Prefabs/Human/Killer"));
    }
}
