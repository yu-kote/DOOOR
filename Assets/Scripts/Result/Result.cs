﻿using System;
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

    private Image _imageText;


    void Start()
    {

        _gameClear.SetActive(false);
        _gameOver.SetActive(false);

        if (ShareData.instance.Status == ResultStatus.GAMECLEAR)
            GameClear();
        if (ShareData.instance.Status == ResultStatus.GAMEOVER)
            GameOver();

        _imageText.color = new Color(1, 1, 1, 0);

        StartCoroutine(ChangeTitle(3.0f));
    }

    void GameClear()
    {
        _gameClear.SetActive(true);
        _imageText = _gameClear.transform.GetChild(0).GetComponent<Image>();

        var image = _gameClear.GetComponent<Image>();

        if (ShareData.instance.WomanCount >= 1 &&
            ShareData.instance.TallManCount >= 1)
            image.sprite = Resources.Load<Sprite>("Texture/Result/clear01");

        if (ShareData.instance.WomanCount >= 1 &&
            ShareData.instance.TallManCount >= 1 &&
            ShareData.instance.FatCount >= 1)
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
            if (Input.GetButtonDown(_actionButton))
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