using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Title : MonoBehaviour
{
    [SerializeField]
    string _startButton = "Action";
    [SerializeField]
    string _creditButton = "Sound";

    [SerializeField]
    Image _aButtonImage;
    [SerializeField]
    Image _bButtonImage;


    bool _isChangeScene;

    SceneChanger _sceneChanger;

    void Start()
    {
        _sceneChanger = GameObject.Find("SceneChanger").GetComponent<SceneChanger>();
        SoundManager.Instance.PlayBGM("title");
        _isChangeScene = false;
        StartCoroutine(SceneChange());
    }

    private IEnumerator SceneChange()
    {
        yield return new WaitForSeconds(1.0f);

        while (true)
        {
            if (_isChangeScene)
                break;
            if (Input.GetButtonDown(_startButton))
            {
                ChangeTutorial();
                EasingInitiator.Add(_aButtonImage.gameObject, Vector3.one * 1.3f,
                                    0.5f, EaseType.BackOut, EaseValue.SCALE);
            }
            if (Input.GetKeyDown(KeyCode.Return))
            {
                ChangeTutorial();
                EasingInitiator.Add(_aButtonImage.gameObject, Vector3.one * 1.3f,
                        0.5f, EaseType.BackOut, EaseValue.SCALE);
            }
            if (Input.GetButtonDown(_creditButton))
            {
                ChangeCredit();
                EasingInitiator.Add(_bButtonImage.gameObject, Vector3.one * 1.3f,
                        0.5f, EaseType.BackOut, EaseValue.SCALE);
            }
            if (_isChangeScene)
                SoundManager.Instance.ChoiceSE();
            yield return null;
        }
    }

    void ChangeGamemain()
    {
        _sceneChanger.SceneChange("GameMain", () => SoundManager.Instance.StopBGM());
        SoundManager.Instance.StopBGM();
        _isChangeScene = true;
    }

    void ChangeTutorial()
    {
        _sceneChanger.SceneChange("Tutorial", () => SoundManager.Instance.StopBGM());
        SoundManager.Instance.StopBGM();
        _isChangeScene = true;
    }
    void ChangeCredit()
    {
        _sceneChanger.SceneChange("Credit", () => SoundManager.Instance.StopBGM());
        SoundManager.Instance.StopBGM();
        _isChangeScene = true;
    }
}
