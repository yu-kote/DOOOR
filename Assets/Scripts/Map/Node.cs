using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{

    // ダイクストラ法 http://www.deqnotes.net/acmicpc/dijkstra/

    // 確定かどうか
    private bool _isDone;
    public bool IsDone { get { return _isDone; } set { _isDone = value; } }

    // このノードへの現時点での最小コスト
    private int _cost;
    public int Cost { get { return _cost; } set { _cost = value; } }



    void Start()
    {

    }

    void Update()
    {

    }
}
