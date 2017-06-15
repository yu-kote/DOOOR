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
        SoundManager.Instance.PlayBGM("title");
    }

    void Update()
    {
        if (Input.GetButtonDown(_startButton))
        {
            //ChangeGamemain();
            ChangeTutorial();
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            //ChangeGamemain();
            ChangeTutorial();
        }
    }

    void ChangeGamemain()
    {
        _sceneChanger.SceneChange("GameMain");
        SoundManager.Instance.StopBGM();
    }

    void ChangeTutorial()
    {
        _sceneChanger.SceneChange("Tutorial");
        SoundManager.Instance.StopBGM();
    }
}
