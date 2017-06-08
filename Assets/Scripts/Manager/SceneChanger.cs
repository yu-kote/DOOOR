using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    [SerializeField]
    private FadeController _fadeController;

    void Awake()
    {
        _fadeController.gameObject.SetActive(true);
        _fadeController.FadeIn();
        
        GameObject[] obj = GameObject.FindGameObjectsWithTag("SceneChanger");
        if (obj.Length > 1)
            Destroy(gameObject);
        else
            DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (_fadeController.State == FadeState.FADE_IN)
            if (Input.GetKey(KeyCode.Return))
                _fadeController.FadeOut();

        ChangeGameMain();
    }

    void ChangeGameMain()
    {
        if (_fadeController.State == FadeState.FADE_OUT)
            if (_fadeController.IsFadeComplete())
            {
                SceneManager.LoadScene("GameMain");
                _fadeController.FadeIn();
            }
    }

}
