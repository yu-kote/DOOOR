using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UniRx;
using System;
using System.IO;

public enum GameState
{
    SELECT,
    TUTORIAL,
    GAMEMAIN,
    GAMECLEAR,
    GAMEOVER,
}

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private string _selectEndButton = "Action";
    [SerializeField]
    private string _gameEndButton = "Action";

    private AIGenerator _aiGenerator;
    private GameMainUIController _uiController;

    [SerializeField]
    private GameObject _gameClear;
    [SerializeField]
    private GameObject _gameOver;

    private GameState _currentGameState;
    public GameState CurrentGameState { get { return _currentGameState; } set { _currentGameState = value; } }
    private GameState _prevGameState;

    private bool _isGameEnd;
    public bool IsGameEnd { get { return _isGameEnd; } set { _isGameEnd = value; } }


    void Start()
    {
        //var field = GameObject.Find("Field");
        var human_manager = GameObject.Find("HumanManager");
        _aiGenerator = human_manager.GetComponent<AIGenerator>();
        _currentGameState = GameState.SELECT;

        var canvas = GameObject.Find("UICanvas");
        _uiController = canvas.GetComponent<GameMainUIController>();

        StateChangeCallBack(() => _aiGenerator.MoveStartHumans(), GameState.GAMEMAIN);
        StateChangeCallBack(() => _uiController.UiStart(), GameState.GAMEMAIN);
        StateChangeCallBack(() => _uiController.UiFadeAway(), GameState.GAMEOVER);
        StateChangeCallBack(() => _uiController.UiFadeAway(), GameState.GAMECLEAR);
    }

    void Update()
    {
        GameEndUpdate();
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
                GameObject.Find("SceneChanger")
                    .GetComponent<SceneChanger>().SceneChange("Title");
            
            return;
        }
        GameClear();
        GameOver();
    }

    void GameClear()
    {
        var humans = _aiGenerator.Humans;
        if (humans.Count == 0) return;
        if (humans.FirstOrDefault(human => human.tag == "Victim") != null)
            return;
        _gameClear.SetActive(true);
        _isGameEnd = true;
        _currentGameState = GameState.GAMECLEAR;
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
            if (current_node.GetComponent<Deguti>())
            {
                human.GetComponent<AIItemController>().UseItem(ItemType.LASTKEY);

                var ai_controller = human.GetComponent<AIController>();
                var animation = human.GetComponent<VictimAnimation>();
                animation.AnimStatus = VictimAnimationStatus.OPEN_DOOR;
                ai_controller.StopMovement(0.5f, () => animation.AnimStatus = VictimAnimationStatus.IDOL);
                return true;
            }
            return false;
        });

        if (goal_human == null)
            return;

        _gameOver.SetActive(true);
        _isGameEnd = true;
        _currentGameState = GameState.GAMEOVER;
    }

    public bool IsPushActionButton()
    {
        return Input.GetButtonDown(_selectEndButton);
    }
}
