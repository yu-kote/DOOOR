using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UniRx;
using System;
using UnityEngine.UI;

public enum GameState
{
    SELECT,
    TUTORIAL,
    GAMEMAIN,
    STOP,
    GAMECLEAR,
    GAMEOVER,
}

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private string _selectEndButton = "Action";
    [SerializeField]
    private string _gameEndButton = "Action";

    [SerializeField]
    private string _gameStopButton;
    [SerializeField]
    private string _restartButton = "Sound";

    [SerializeField]
    private Image _help;

    private AIGenerator _aiGenerator;
    private GameMainUIController _uiController;
    private Reloader _reloader;
    [SerializeField]
    private TrapSelectUI _trapSelectUI;

    private GameState _currentGameState;
    public GameState CurrentGameState { get { return _currentGameState; } set { _currentGameState = value; } }
    private GameState _prevGameState;

    private bool _isGameEnd;
    public bool IsGameEnd { get { return _isGameEnd; } set { _isGameEnd = value; } }

    private bool _isStop = false;
    private bool _canHelpUpdate = true;

    void Start()
    {
        var human_manager = GameObject.Find("HumanManager");
        _aiGenerator = human_manager.GetComponent<AIGenerator>();
        _currentGameState = GameState.SELECT;

        var canvas = GameObject.Find("UICanvas");
        _uiController = canvas.GetComponent<GameMainUIController>();

        _reloader = GetComponent<Reloader>();

        StateChangeCallBack(() => _aiGenerator.MoveStartHumans(), GameState.GAMEMAIN);
        StateChangeCallBack(() => _uiController.UiStart(), GameState.GAMEMAIN);

        // 人間の動きを止める
        StateChangeCallBack(() => _aiGenerator.MoveEndHumans(), GameState.GAMEOVER);

        // UIを隠す
        StateChangeCallBack(() => _uiController.UiFadeAway(), GameState.GAMEOVER);
        StateChangeCallBack(() => _uiController.UiFadeAway(), GameState.GAMECLEAR);

        // リザルトに移行する
        StateChangeCallBack(() => StartCoroutine(ScenaChangeResult(3.0f)), GameState.GAMEOVER);
        StateChangeCallBack(() => StartCoroutine(ScenaChangeResult(3.0f)), GameState.GAMECLEAR);
    }

    void Update()
    {
        GameStateUpdate();
        GameEndUpdate();
        HelpUpdate();
    }

    // ゲームのステータスが切り替わった時にコールバックされる関数を登録する
    public void StateChangeCallBack(Action action, GameState state)
    {
        StartCoroutine(CallBack(action, state));
    }

    private IEnumerator CallBack(Action action, GameState state)
    {
        while (true)
        {
            yield return null;
            if (_currentGameState != state)
                continue;
            action();
            break;
        }
    }

    // クリアかゲームオーバーの判定をするところ
    void GameEndUpdate()
    {
        if (_isGameEnd)
        {
            if (Input.GetButtonDown(_gameEndButton))
                GameObject.Find("SceneChanger").GetComponent<SceneChanger>()
                    .SceneChange("Result", () => SoundManager.Instance.StopBGM());
            return;
        }

        // デバッグ用
        if (Input.GetKey(KeyCode.T) && Input.GetKey("1"))
        {
            GameObject.Find("SceneChanger").GetComponent<SceneChanger>()
                .SceneChange("Result", () => SoundManager.Instance.StopBGM());
            ShareData.instance.Status = ResultStatus.GAMECLEAR;
        }
        else if (Input.GetKey(KeyCode.T) && Input.GetKey("2"))
        {
            GameObject.Find("SceneChanger").GetComponent<SceneChanger>()
                .SceneChange("Result", () => SoundManager.Instance.StopBGM());
            ShareData.instance.Status = ResultStatus.GAMEOVER;
        }

        GameClear();
        GameOver();
    }

    // リザルトに移行する
    private IEnumerator ScenaChangeResult(float time)
    {
        yield return new WaitForSeconds(time);
        GameObject.Find("SceneChanger").GetComponent<SceneChanger>()
                .SceneChange("Result", () => SoundManager.Instance.StopBGM());
    }

    void GameStateUpdate()
    {
        if (_currentGameState == _prevGameState)
            return;
        _prevGameState = _currentGameState;

    }

    void GameClear()
    {
        var humans = _aiGenerator.Humans;
        if (humans.Count == 0) return;
        if (humans.FirstOrDefault(human => human.tag == "Victim") != null)
            return;

        _isGameEnd = true;
        _currentGameState = GameState.GAMECLEAR;
        ShareData.instance.Status = ResultStatus.GAMECLEAR;
    }

    void GameOver()
    {
        var humans = _aiGenerator.Humans;
        var goal_human = humans
            .Where(human => human.tag == "Victim")
            .FirstOrDefault(human =>
        {
            var item = human.GetComponent<AIItemController>().HaveItemCheck(ItemType.LASTKEY);
            if (item == false)
                return false;
            var current_node = human.GetComponent<AIController>().CurrentNode;
            if (current_node == null)
                return false;
            var deguti = current_node.GetComponent<Deguti>();
            if (deguti)
            {
                deguti.StartOpening();
                human.GetComponent<AIItemController>().UseItem(ItemType.LASTKEY);

                var animation = human.GetComponent<VictimAnimation>();
                animation.ChangeAnimation(VictimAnimationStatus.OPEN_DOOR, 0.5f);
                return true;
            }
            return false;
        });

        if (goal_human == null)
            return;

        _isGameEnd = true;
        _currentGameState = GameState.GAMEOVER;
        ShareData.instance.Status = ResultStatus.GAMEOVER;
    }

    public bool IsPushActionButton()
    {
        return Input.GetButtonDown(_selectEndButton);
    }

    //---------------------------------------------------------------------------------------------
    // ヘルプの処理
    //---------------------------------------------------------------------------------------------
    private bool HelpStartButton()
    {
        if (Input.GetButtonDown(_gameStopButton))
            return true;
        if (_isStop == false)
            return false;
        if (Input.GetButtonDown(_selectEndButton))
            return true;
        if (Input.GetButtonDown(_restartButton))
        {
            _reloader.StageResetStart();
            return true;
        }
        return false;
    }

    private void HelpUpdate()
    {
        if (_currentGameState != GameState.GAMEMAIN &&
           _currentGameState != GameState.STOP)
            return;

        if (_isStop)
            MovementAllStop();

        if (_canHelpUpdate == false)
            return;

        if (HelpStartButton())
        {
            _isStop = !_isStop;
            if (_isStop)
                StartCoroutine(ImageFadeIn(_help));
            else
                StartCoroutine(ImageFadeOut(_help));
        }
    }

    public IEnumerator ImageFadeIn(Image image, float speed = 3, float max_alpha = 1, bool is_child_trans = true)
    {
        _canHelpUpdate = false;
        image.color = new Color(1, 1, 1, 0);
        var color = image.color;
        while (color.a < max_alpha)
        {
            color.a += Time.deltaTime * speed;
            color.a = Mathf.Clamp(color.a, 0, 1);
            image.color = color;
            if (is_child_trans)
                SetImageChildColor(image, color);
            yield return null;
        }
        _canHelpUpdate = true;
    }

    public IEnumerator ImageFadeOut(Image image, float speed = 3, float min_alpha = 0, bool is_child_trans = true)
    {
        _canHelpUpdate = false;
        var color = image.color;
        while (color.a > min_alpha)
        {
            color.a -= Time.deltaTime * speed;
            color.a = Mathf.Clamp(color.a, 0, 1);
            image.color = color;
            if (is_child_trans)
                SetImageChildColor(image, color);
            yield return null;
        }
        _canHelpUpdate = true;
        MovementAllStart();
    }

    public void SetImageChildColor(Graphic image, Color color)
    {
        var t = image.transform;
        for (int i = 0; i < t.childCount; i++)
        {
            var child_image = t.GetChild(i).GetComponent<Image>();
            if (child_image)
            {
                child_image.color = color;
                SetImageChildColor(child_image, color);
            }
            var child_text = t.GetChild(i).GetComponent<Text>();
            if (child_text)
            {
                child_text.color = color;
                SetImageChildColor(child_text, color);
            }
        }
    }

    public void MovementAllStop()
    {
        _currentGameState = GameState.STOP;
        _aiGenerator.HumanMoveControll(false);
    }

    public void MovementAllStart()
    {
        _currentGameState = GameState.GAMEMAIN;
        _aiGenerator.HumanMoveControll(true);
    }
}
