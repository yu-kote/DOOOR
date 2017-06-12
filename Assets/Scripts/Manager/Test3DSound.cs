using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test3DSound : MonoBehaviour
{

    void Start()
    {
        //SoundManager.Instance.PlayBGM("BGM", gameObject);
    }
    int count = 0;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
            ReportBoard.Instance.Pop("テストaa" + count++);
        if (Input.GetKeyDown(KeyCode.O))
            ReportBoard.Instance.Pop("テストaa" + count++, true);

    }
}
