using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Credit : MonoBehaviour
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
    }

    void Update()
    {
        if (_isChangeScene)
            return;

        if (Input.GetButtonDown(_startButton))
        {
            ChangeTitle();
            EasingInitiator.Add(_aButtonImage.gameObject, Vector3.one * 1.3f,
                                0.5f, EaseType.BackOut, EaseValue.SCALE);
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            ChangeTitle();
            EasingInitiator.Add(_aButtonImage.gameObject, Vector3.one * 1.3f,
                    0.5f, EaseType.BackOut, EaseValue.SCALE);
        }
        if (Input.GetButtonDown(_creditButton))
        {
            ChangeTitle();
            EasingInitiator.Add(_bButtonImage.gameObject, Vector3.one * 1.3f,
                    0.5f, EaseType.BackOut, EaseValue.SCALE);
        }

        if (_isChangeScene)
            SoundManager.Instance.PlaySE("kettei");
    }

    void ChangeTitle()
    {
        _sceneChanger.SceneChange("Title", () => SoundManager.Instance.StopBGM());
        SoundManager.Instance.StopBGM();
        _isChangeScene = true;
    }
}
