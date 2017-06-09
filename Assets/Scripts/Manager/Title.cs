using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Title : MonoBehaviour
{
    [SerializeField]
    string _startButton = "Action";


    [SerializeField]
    SceneChanger _sceneChanger;

    void Start()
    {

    }

    void Update()
    {
        if (Input.GetButtonDown(_startButton))
        {
            _sceneChanger.ChangeGameMain();
            Destroy(this);
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            _sceneChanger.ChangeGameMain();
            Destroy(this);
        }
    }
}
