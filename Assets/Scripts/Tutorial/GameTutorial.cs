using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UniRx;
using System;
using UnityEngine.UI;

public class GameTutorial : MonoBehaviour
{
    [SerializeField]
    Image _tutorial;
    [SerializeField]
    Image _tutorialImage;
    [SerializeField]
    Text _numText;
    [SerializeField]
    Text _tutorialText;

    [SerializeField]
    private float[] _tutorialPopTimings;
    private float _elapsedTime;
    private int _tutorialNum = 0;

    private bool _isEnable;
    public bool IsEnable { get { return _isEnable; } set { _isEnable = value; } }

    private GameManager _gameManager;
    private GameState _state;

    Dictionary<string, Vector3> _uiStartPositions = new Dictionary<string, Vector3>();
    Dictionary<string, Vector3> _uiEndPositions = new Dictionary<string, Vector3>();

    float _easeTime = 1.0f;

    void Start()
    {
        Setup();
    }

    public void Setup()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        UiSetup();
        _tutorial.gameObject.SetActive(false);
        _elapsedTime = 0.0f;
    }

    void UiSetup()
    {
        for (int i = 0; i < _tutorial.transform.childCount; i++)
        {
            var ui = _tutorial.transform.GetChild(i).gameObject;
            _uiStartPositions.Add(ui.name, ui.transform.localPosition);

            var rect = ui.transform as RectTransform;
            rect.localPosition += new Vector3(-1700, 0, 0);

            _uiEndPositions.Add(ui.name, ui.transform.localPosition);
        }
    }

    void Update()
    {
        if (_isEnable == false)
            return;

        TutorialUpdate();

        _state = _gameManager.CurrentGameState;
        // ゲーム中しか動かないようにする
        if (_state != GameState.GAMEMAIN)
            return;

        // ゲーム内時間を数える
        TutorialTimeUpdate();
    }

    private void TutorialUpdate()
    {
        // チュートリアルが終わっているかどうか
        if (IsTutorialEnd())
            return;

        // 番号が更新されていなかったらはじく
        int num = TutorialPopNum();
        if (_tutorialNum == num)
            return;
        _tutorialNum = num;
        num -= 1;

        TutorialImageChange(num);
        TutorialTextChange(num);

        StartCoroutine(ImageFadeInAction());
        _gameManager.MovementAllStop();
    }

    // チュートリアルが出てくる演出を全部やる
    private IEnumerator ImageFadeInAction()
    {
        _tutorial.gameObject.SetActive(true);
        yield return _gameManager.ImageFadeIn(_tutorial, 1, 0.75f, false);
        yield return EaseStart();

        while (Input.GetKeyDown(KeyCode.Return) == false &&
               _gameManager.IsPushActionButton() == false)
            yield return null;

        yield return EaseFadeAway();

        yield return _gameManager.ImageFadeOut(_tutorial, 2, 0, false);

        _gameManager.MovementAllStart();
    }

    private IEnumerator EaseStart()
    {
        for (int i = 0; i < _tutorial.transform.childCount; i++)
        {
            var ui = _tutorial.transform.GetChild(i).gameObject;
            EaseUi(ui, _uiStartPositions[ui.name]);
            yield return new WaitForSeconds(_easeTime);
        }
    }

    private IEnumerator EaseFadeAway()
    {
        for (int i = 0; i < _tutorial.transform.childCount; i++)
        {
            var ui = _tutorial.transform.GetChild(i).gameObject;
            EaseUi(ui, _uiEndPositions[ui.name]);
        }
        yield return new WaitForSeconds(_easeTime);
    }

    public void EaseUi(GameObject ui, Vector3 target_pos)
    {
        EasingInitiator.Add(ui, target_pos,
                    _easeTime, EaseType.CircOut);
    }

    private void TutorialTimeUpdate()
    {
        if (_state != GameState.GAMEMAIN)
            return;
        _elapsedTime += Time.deltaTime;
    }

    private bool IsTutorialEnd()
    {
        return _tutorialPopTimings.Last() < _elapsedTime;
    }

    private int TutorialPopNum()
    {
        for (int i = 0; i < _tutorialPopTimings.Count(); i++)
        {
            if (_elapsedTime < _tutorialPopTimings[i])
                return i;
        }
        return 0;
    }

    private void TutorialImageChange(int num)
    {
        var image = Resources.Load<Sprite>("Texture/Tutorial/tutorial" + num);
        _tutorialImage.sprite = image;
    }

    private void TutorialTextChange(int num)
    {
        _numText.text = "チュートリアル " + (num + 1);

        if (num == 0)
            _tutorialText.text = "殺人鬼を侵入者の元へ導け。彼は一人では追いつけない。";
        if (num == 1)
            _tutorialText.text = "君はⒷボタンでその場から音が出せる。押す長さで音の大きさを変えられる。\n" +
                                 "殺人鬼は音の場所に向かい、侵入者は音から逃げる。";
        if (num == 2)
            _tutorialText.text = "侵入者は部屋へ逃げ込む。殺人鬼はドアを開けられない。";
        if (num == 3)
            _tutorialText.text = "君はAボタンを押している間、ドアをロックできる。";
        //if (num == 4)
        //    _tutorialText.text = "Ⓐボタン長押しでドアを施錠することで\n　　殺人鬼と はさみうち できるぞ！";
    }


    private void OnDestroy()
    {
        _uiStartPositions.Clear();
        _uiEndPositions.Clear();
    }
}
