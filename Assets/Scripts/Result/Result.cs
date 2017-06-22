using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Result : MonoBehaviour
{

    [SerializeField]
    private string _actionButton = "Action";
    [SerializeField]
    private GameObject _gameClear;
    [SerializeField]
    private GameObject _gameOver;

    [SerializeField]
    private MenuBarManager _menuBarManager;

    private Image _imageText;

    private void Awake()
    {
        _menuBarManager.gameObject.SetActive(true);
    }

    void Start()
    {
        _gameClear.SetActive(false);
        _gameOver.SetActive(false);

        if (ShareData.Instance.Status == ResultStatus.GAMECLEAR)
            GameClear();
        if (ShareData.Instance.Status == ResultStatus.GAMEOVER)
            GameOver();

        _imageText.color = new Color(1, 1, 1, 0);

        _menuBarManager.SetBarAction(0, () =>
                    GameObject.Find("SceneChanger").GetComponent<SceneChanger>()
                    .SceneChange("GameMain", () => SoundManager.Instance.StopBGM()));
        _menuBarManager.SetBarAction(1, () =>
                    GameObject.Find("SceneChanger").GetComponent<SceneChanger>()
                    .SceneChange("Title", () => SoundManager.Instance.StopBGM()));

        StartCoroutine(ChangeTitle(3.0f));
    }

    void GameClear()
    {
        _gameClear.SetActive(true);
        _imageText = _gameClear.transform.GetChild(0).GetComponent<Image>();

        var image = _gameClear.GetComponent<Image>();

        if (ShareData.Instance.WomanCount >= 1 &&
            ShareData.Instance.TallManCount >= 1)
            image.sprite = Resources.Load<Sprite>("Texture/Result/clear01");

        if (ShareData.Instance.WomanCount >= 1 &&
            ShareData.Instance.TallManCount >= 1 &&
            ShareData.Instance.FatCount >= 1)
            image.sprite = Resources.Load<Sprite>("Texture/Result/clear02");
        SoundManager.Instance.PlayBGM("gameclear");
    }

    void GameOver()
    {
        _gameOver.SetActive(true);
        _imageText = _gameOver.transform.GetChild(0).GetComponent<Image>();
        SoundManager.Instance.PlayBGM("gameover");
    }

    private IEnumerator ChangeTitle(float time)
    {
        yield return new WaitForSeconds(time);
        while (true)
        {
            if (Input.GetButtonDown(_actionButton))
            {
                GameObject.Find("SceneChanger").GetComponent<SceneChanger>()
                    .SceneChange("Title", () => SoundManager.Instance.StopBGM());
            }
            if (Input.GetKeyDown(KeyCode.Return))
            {
                GameObject.Find("SceneChanger").GetComponent<SceneChanger>()
                    .SceneChange("Title", () => SoundManager.Instance.StopBGM());
            }

            var color = _imageText.color;
            color.a += Time.deltaTime;
            _imageText.color = color;

            yield return null;
        }
    }

    void Update()
    {

    }
}
