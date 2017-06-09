using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Title : MonoBehaviour
{
    [SerializeField]
    string _startButton = "Action";

    SceneChanger _sceneChanger;

    void Start()
    {
        _sceneChanger = GameObject.Find("SceneChanger").GetComponent<SceneChanger>();
    }

    void Update()
    {
        if (Input.GetButtonDown(_startButton))
        {
            _sceneChanger.SceneChange("GameMain");
            Destroy(this);
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            _sceneChanger.SceneChange("GameMain");
            Destroy(this);
        }
    }
}
