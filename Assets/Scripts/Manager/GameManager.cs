using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UniRx;
using System;
using UnityEngine.UI;
using System.IO;
using System.Text;


public enum GameState
{
    SELECT,         // セレクト中
    GAMEMAIN,       // ゲーム進行中
    STOP,           // ヘルプ画面や、チュートリアル中など
    STAGING,        // 演出中
    GAMECLEAR,      // クリア
    GAMEOVER,       // ゲームオーバー
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
    private GameTutorial _tutorial;

    [SerializeField]
    private MenuBarManager _menuBarManager;

    [SerializeField]
    private TrapSelectUI _trapSelectUI;

    private GameState _currentGameState;
    public GameState CurrentGameState { get { return _currentGameState; } set { _currentGameState = value; } }

    private bool _isGameEnd;
    public bool IsGameEnd { get { return _isGameEnd; } set { _isGameEnd = value; } }

    private bool _isStop = false;
    private bool _canHelpUpdate = true;

    private void Awake()
    {
        _menuBarManager.gameObject.SetActive(true);
    }

    void Start()
    {
        var human_manager = GameObject.Find("HumanManager");
        _aiGenerator = human_manager.GetComponent<AIGenerator>();
        _currentGameState = GameState.SELECT;

        var canvas = GameObject.Find("UICanvas");
        _uiController = canvas.GetComponent<GameMainUIController>();

        // リスタートするやつ取得
        _reloader = GetComponent<Reloader>();

        // チュートリアル取得
        _tutorial = GetComponent<GameTutorial>();

        // メニューのバー初期化
        _menuBarManager.SetBarAction(0, () => { });
        _menuBarManager.SetBarAction(1, () =>
        {
            _tutorial.TutorialStart(ShareData.Instance.SelectStage);
            _reloader.StageResetStart();
        });
        _menuBarManager.SetBarAction(2, () =>
        {
            GameObject.Find("SceneChanger").GetComponent<SceneChanger>()
            .SceneChange("GameMain", () => SoundManager.Instance.StopBGM());
        });
        _menuBarManager.gameObject.SetActive(false);

        StateChangeCallBack(() => _aiGenerator.MoveStartHumans(), GameState.GAMEMAIN);
        StateChangeCallBack(() => _uiController.UiStart(), GameState.GAMEMAIN);
        StateChangeCallBack(() => _tutorial.TutorialStart(ShareData.Instance.SelectStage), GameState.GAMEMAIN);

        // 人間の動きを止める
        StateChangeCallBack(() => _aiGenerator.MoveEndHumans(), GameState.GAMEOVER);
        StateChangeCallBack(() => _aiGenerator.MoveEndHumans(), GameState.GAMECLEAR);


        // UIを隠す
        StateChangeCallBack(() => _uiController.UiFadeAway(), GameState.GAMEOVER);
        StateChangeCallBack(() => _uiController.UiFadeAway(), GameState.GAMECLEAR);

        // リザルトに移行する
        StateChangeCallBack(() => StartCoroutine(ScenaChangeResult(6.0f)), GameState.GAMEOVER);
        StateChangeCallBack(() => StartCoroutine(ScenaChangeResult(5.0f)), GameState.GAMECLEAR);
    }

    void Update()
    {
        GameEndUpdate();
        HelpUpdate();

#if DEBUG
        if (Input.GetKeyDown(KeyCode.P))
        {
            ShareData.Instance.CanSelectStageMax = 5;
        }
#endif
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
#if DEBUG
        // デバッグ用
        if (Input.GetKey(KeyCode.T) && Input.GetKeyDown("1"))
        {
            GameObject.Find("SceneChanger").GetComponent<SceneChanger>()
                .SceneChange("Result", () => SoundManager.Instance.StopBGM());
            ShareData.Instance.Status = ResultStatus.GAMECLEAR;
            StageOpen();
        }
        else if (Input.GetKey(KeyCode.T) && Input.GetKeyDown("2"))
        {
            GameObject.Find("SceneChanger").GetComponent<SceneChanger>()
                .SceneChange("Result", () => SoundManager.Instance.StopBGM());
            ShareData.Instance.Status = ResultStatus.GAMEOVER;
        }
#endif
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

    void GameClear()
    {
        var humans = _aiGenerator.Humans;
        if (humans.Count == 0) return;
        if (humans.FirstOrDefault(human => human.tag == "Victim") != null)
            return;

        _isGameEnd = true;
        _currentGameState = GameState.GAMECLEAR;
        ShareData.Instance.Status = ResultStatus.GAMECLEAR;

        StageOpen();
    }

    void StageOpen()
    {
        // クリアしたステージを保存する
        ShareData.Instance.ClearStages.Add(ShareData.Instance.SelectStage);
        // ステージの最大数を増やす処理
        if (ShareData.Instance.SelectStage >= ShareData.Instance.CanSelectStageMax)
        {
            ShareData.Instance.CanSelectStageMax = ShareData.Instance.SelectStage + 1;
            ShareData.Instance.SelectStage += 1;
            if (ShareData.Instance.SelectStage > 5)
                ShareData.Instance.CanSelectStageMax = ShareData.Instance.StageMax;
        }

        ShareData.Instance.CanSelectStageMax =
            Mathf.Clamp(ShareData.Instance.CanSelectStageMax, 1, ShareData.Instance.StageMax);
        ShareData.Instance.SelectStage =
            Mathf.Clamp(ShareData.Instance.SelectStage, 1, ShareData.Instance.StageMax);

        StageDataSave();
    }

    private void StageDataSave()
    {
        SaveDataJson save_data = new SaveDataJson();

        // 上書き
        save_data.CanSelectStageMax = ShareData.Instance.CanSelectStageMax;
        save_data.ClearStages = ShareData.Instance.ClearStages;

        //var path = Application.dataPath + "/Resources/SaveData/SaveData.json";
        var item = JsonUtility.ToJson(save_data);

        StreamWriter sw = new StreamWriter("Assets/Resources/SaveData/SaveData.json", false);

        sw.WriteLine(item);
        sw.Flush();
        sw.Close();
        //{"CanSelectStageMax":1,"ClearStages":[0]}
    }

    public void Load()
    {
        using (StreamReader sr = new StreamReader("Assets/Resources/SaveData/SaveData.json"))
        {
            string line = sr.ReadLine();
            var data = JsonUtility.FromJson<SaveDataJson>(line);
            ShareData.Instance.CanSelectStageMax = data.CanSelectStageMax;

            foreach (var item in data.ClearStages)
            {
                if (ShareData.Instance.ClearStages.Contains(item) == false)
                    ShareData.Instance.ClearStages.Add(item);
            }
        }
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

        Staging(goal_human.GetComponent<AIController>().CurrentNode);
        goal_human.AddComponent<AIEndMove>();

        _isGameEnd = true;
        _currentGameState = GameState.GAMEOVER;
        ShareData.Instance.Status = ResultStatus.GAMEOVER;
    }

    private void Staging(Node target)
    {
        var app_pos = target.transform.position + new Vector3(0, 4, -2);

        var node_manager = GameObject.Find("Field").GetComponent<NodeManager>();
        var side = node_manager.WhichSurfaceNum(target.CellX);
        GameObject.Find("MainCamera").GetComponent<KillApproach>().Reverberation = 5.0f;
        GameObject.Find("MainCamera").GetComponent<KillApproach>().StartApproach(app_pos, side);
    }

    public bool IsPushActionButton()
    {
        return Input.GetButtonDown(_selectEndButton);
    }

    public bool IsPushHelpButton()
    {
        return Input.GetButtonDown(_gameStopButton);
    }

    //---------------------------------------------------------------------------------------------
    // ヘルプの処理
    //---------------------------------------------------------------------------------------------
    private bool HelpStartButton()
    {
        // ヘルプを開いていなかったら
        if (Input.GetButtonDown(_gameStopButton))
            return true;

        // ヘルプを開いていたら
        if (_isStop == false)
            return false;
        if (Input.GetButtonDown(_gameStopButton))
            return true;
        if (Input.GetButtonDown(_restartButton))
            return true;
        if (_menuBarManager.IsBarUpdate())
            return true;
        return false;
    }

    private void HelpUpdate()
    {
        if (_currentGameState != GameState.GAMEMAIN &&
           _currentGameState != GameState.STOP)
            return;

        if (_isStop)
        {
            _currentGameState = GameState.STOP;
            MovementAllStop();
        }

        if (_canHelpUpdate == false)
            return;

        if (HelpStartButton())
        {
            _isStop = !_isStop;
            if (_isStop)
            {
                _menuBarManager.gameObject.SetActive(true);
                StartCoroutine(ImageFadeIn(_help));
            }
            else
            {
                _menuBarManager.gameObject.SetActive(false);
                StartCoroutine(ImageFadeOut(_help));
            }
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
        _currentGameState = GameState.GAMEMAIN;
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
            //if (child_text.name == "TrapText")
            //    continue;


            if (child_text)
            {
                if (child_text.name == "TrapText")
                {
                    child_text.color = color;
                    child_text.color = new Color(0, 0, 0, color.a);
                    continue;
                }
                child_text.color = color;
                SetImageChildColor(child_text, color);
            }
        }
    }

    public void MovementAllStop()
    {
        _aiGenerator.HumanMoveControll(false);
    }

    public void MovementAllStart()
    {
        _aiGenerator.HumanMoveControll(true);
    }
}
