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
    AIGenerator _aiGenerator;
    [SerializeField]
    Image _tutorial;
    [SerializeField]
    Image _tutorialImage;
    [SerializeField]
    Text _numText;
    [SerializeField]
    GameObject _textParent;
    [SerializeField]
    GameObject _skipText;

    GameObject _text;

    string[] _tutorialTexts;

    private int _stageNum;
    private int _tutorialNum;
    private int _tutorialNumMax;

    private GameManager _gameManager;

    Dictionary<string, Vector3> _uiStartPositions = new Dictionary<string, Vector3>();
    Dictionary<string, Vector3> _uiEndPositions = new Dictionary<string, Vector3>();

    float _easeTime = 1.0f;

    void Start()
    {
        Setup();
        UiSetup();
    }

    public void Setup()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        _tutorialNum = 0;
        _tutorialNumMax = 0;

        _tutorial.gameObject.SetActive(false);
    }

    void UiSetup()
    {
        _uiStartPositions.Clear();
        _uiEndPositions.Clear();
        for (int i = 0; i < _tutorial.transform.childCount; i++)
        {
            var ui = _tutorial.transform.GetChild(i).gameObject;
            _uiStartPositions.Add(ui.name, ui.transform.localPosition);

            var rect = ui.transform as RectTransform;
            rect.localPosition += new Vector3(-1700, 0, 0);

            _uiEndPositions.Add(ui.name, ui.transform.localPosition);
        }
    }

    public void TutorialStart(int stage_num)
    {
        Setup();
        _stageNum = stage_num;
        var num = GetTutorialNum(stage_num);
        if (num == 0)
            return;
        _tutorialNumMax = num;
        StartCoroutine(ImageFadeInAction());
    }

    // チュートリアルが出てくる演出を全部やる
    private IEnumerator ImageFadeInAction()
    {
        GameStop();

        _tutorial.gameObject.SetActive(true);

        yield return _gameManager.ImageFadeIn(_tutorial, 1, 0.75f, false);
        while (_tutorialNumMax > _tutorialNum)
        {
            _gameManager.CurrentGameState = GameState.STAGING;
            TutorialImageChange(_stageNum, _tutorialNum);

            // 湧かせる
            var path = "Prefabs/Tutorial/TutorialText" + _stageNum + "-" + _tutorialNum;
            var text = Resources.Load<GameObject>(path);
            text = Instantiate(text, _textParent.transform);
            text.transform.localPosition = Vector3.zero;
            text.transform.localScale = Vector3.one;

            // タイトルを入れる
            _numText.text = GetTutorialTitle(_stageNum, _tutorialNum);

            yield return EaseStart();

            while (Input.GetKeyDown(KeyCode.Return) == false &&
                   _gameManager.IsPushActionButton() == false)
            {
                _gameManager.MovementAllStop();
                _gameManager.CurrentGameState = GameState.STAGING;

                if (_gameManager.IsPushHelpButton())
                {
                    Destroy(text);
                    yield return EaseFadeAway();
                    yield return _gameManager.ImageFadeOut(_tutorial, 2, 0, false);
                    GameStart();
                    yield break;
                }
                yield return null;
            }

            yield return EaseFadeAway();

            _tutorialNum++;
            Destroy(text);

            yield return new WaitForSeconds(0.5f);
        }

        yield return _gameManager.ImageFadeOut(_tutorial, 2, 0, false);

        GameStart();
    }

    private void GameStop()
    {
        _gameManager.MovementAllStop();
        _gameManager.CurrentGameState = GameState.STAGING;
        _skipText.SetActive(true);
    }

    private void GameStart()
    {
        _skipText.SetActive(false);
        _gameManager.CurrentGameState = GameState.GAMEMAIN;
        _gameManager.MovementAllStart();
    }

    private IEnumerator EaseStart()
    {
        for (int i = 0; i < _tutorial.transform.childCount; i++)
        {
            var ui = _tutorial.transform.GetChild(i).gameObject;
            EaseUi(ui, _uiStartPositions[ui.name]);
            yield return new WaitForSeconds(_easeTime / 2);
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

    private void TutorialImageChange(int stage_num, int num)
    {
        var image = Resources.Load<Sprite>("Texture/GameMainUI/Tutorial/tutorial" + stage_num + "-" + num);
        _tutorialImage.sprite = image;
    }

    private int GetTutorialNum(int stage_num)
    {
        if (stage_num == 1)
            return 4;
        if (stage_num == 2)
            return 3;
        if (stage_num == 3)
            return 2;
        if (stage_num == 4 || stage_num == 5)
            return 1;
        return 0;
    }

    private string GetTutorialTitle(int stage_num, int num)
    {
        if (stage_num == 1)
            return "";
        if (stage_num == 2)
            return "< 能力解放「落とし穴」>";
        if (stage_num == 3)
            if (num == 0)
                return "< 能力解放「ロープ」>";
            else if (num == 1)
                return "< 能力解放「階段封じ」>";
        if (stage_num == 4)
            return "< 能力解放「停電」>";
        if (stage_num == 5)
            return "< 侵入者の反撃「武器」 >";
        return "";
    }

    private void OnDestroy()
    {
        _uiStartPositions.Clear();
        _uiEndPositions.Clear();
    }
}
