﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootPrint : MonoBehaviour
{
    // 足跡
    [SerializeField]
    private List<MyNumber> _traces = new List<MyNumber>();
    public List<MyNumber> Traces { get { return _traces; } set { _traces = value; } }

    // ノード上にいる人間
    [SerializeField]
    private List<GameObject> _humansOnNode = new List<GameObject>();
    public List<GameObject> HumansOnNode { get { return _humansOnNode; } set { _humansOnNode = value; } }

    public void StepIn(GameObject human)
    {
        if (_humansOnNode.Contains(human)) return;
        _humansOnNode.Add(human);

        // ノードに入った人間にこのノードにいるという情報を教える
        human.GetComponent<AIController>().CurrentNode = GetComponent<Node>();

        AddTrace(human);
    }

    public void AddTrace(GameObject human)
    {
        var num = human.GetComponent<MyNumber>();
        if (_traces.Contains(num)) return;
        _traces.Add(num);
    }

    public void StepOut(GameObject human)
    {
        _humansOnNode.Remove(human);
    }

    public bool TraceCheck(GameObject human)
    {
        var mynumber = human.GetComponent<MyNumber>();
        return _traces.Contains(mynumber);
    }

    public void EraseTrace(MyNumber mynumber)
    {
        _traces.Remove(mynumber);
    }

    public void EraseHumanOnNode(GameObject human)
    {
        _humansOnNode.Remove(human);
    }

    private void OnDestroy()
    {
        _traces.Clear();
        _humansOnNode.Clear();
    }
}
