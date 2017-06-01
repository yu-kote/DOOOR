using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test3DSound : MonoBehaviour
{

    void Start()
    {
        SoundManager.Instance.PlayBGM("BGM", gameObject);
    }

    void Update()
    {

    }
}
