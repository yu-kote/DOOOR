using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    [SerializeField]
    private FadeController _fadeController;

    private bool _isSceneChange;

    void Awake()
    {
        _fadeController.gameObject.SetActive(true);
        _fadeController.FadeIn();

        GameObject[] obj = GameObject.FindGameObjectsWithTag("SceneChanger");
        if (obj.Length > 1)
            Destroy(gameObject);
        else
            DontDestroyOnLoad(gameObject);

        _isSceneChange = false;
    }

    void Update()
    {
    }

    public void SceneChange(string scene_name)
    {
        if (_fadeController.State == FadeState.FADE_IN)
            _fadeController.FadeOut();
        if (_isSceneChange == true)
            return;
        _isSceneChange = true;
        StartCoroutine(ChangeScene(scene_name));
    }

    private IEnumerator ChangeScene(string scene_name)
    {
        while (_fadeController.IsFadeComplete() == false)
        {
            yield return null;

            if (_fadeController.IsFadeComplete())
            {
                SceneManager.LoadScene(scene_name);
                _fadeController.FadeIn();
                _isSceneChange = false;
                break;
            }
        }
    }
}
