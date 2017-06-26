using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UniRx;
using System;
using UnityEngine.UI;

public class Reloader : MonoBehaviour
{
    [SerializeField]
    private Image _fade;

    private GameManager _gameManager;
    private GameObject _player;
    private GameObject _camera;
    private NodeManager _nodeManager;
    private AIGenerator _aiGenerator;
    private MapLoader _mapLoader;

    private int _selectStageNum;
    public int SelectStageNum { get { return _selectStageNum; } set { _selectStageNum = value; } }

    private bool _isReset;

    void Start()
    {
        _gameManager = GetComponent<GameManager>();

        var field = GameObject.Find("Field");
        _nodeManager = field.GetComponent<NodeManager>();
        _mapLoader = field.GetComponent<MapLoader>();

        var human_manager = GameObject.Find("HumanManager");
        _aiGenerator = human_manager.GetComponent<AIGenerator>();

        _camera = GameObject.Find("MainCamera");
        _player = GameObject.Find("Player");

        _fade.color = new Color(0, 0, 0, 0);

        _isReset = false;
    }

    public void StageSetup()
    {
        StageSetup(_selectStageNum);
    }

    public void StageSetup(int select_stage_num)
    {
        _gameManager.Load();

        _selectStageNum = select_stage_num;
        // マップを読み直す
        ChangeMap(select_stage_num);

        // 人間を表示する
        _aiGenerator.InstanceHumans(select_stage_num);

        // プレイヤーを扉の位置に移動させる
        PlayerSetup();

        if (select_stage_num >= 5)
            _nodeManager.CreateItem();
    }

    public void CameraSetup()
    {
        var player_pos = _player.transform.position;
        // カメラの位置をフィールドの中心に合わせる
        var mover = _camera.GetComponent<CameraMover>();
        var camera_pos = _nodeManager.GetNodesCenterPoint();
        _camera.transform.position = new Vector3(camera_pos.x, camera_pos.y, mover.StartPos.z + player_pos.z);
        _camera.transform.eulerAngles = Vector3.zero;
    }

    private void ChangeMap(int select_stage_num)
    {
        _mapLoader.LoadMap(select_stage_num);
        _nodeManager.Start();
    }

    private void PlayerSetup()
    {
        _player.transform.eulerAngles = Vector3.zero;
        _player.transform.position
            = new Vector3(_aiGenerator.StartNode.transform.position.x,
                          _aiGenerator.StartNode.transform.position.y + 10,
                          -2);
    }

    public void KillerSetup()
    {
        var ai_generator = GameObject.Find("HumanManager").GetComponent<AIGenerator>();
        var x = ai_generator.KillerPopCell(_selectStageNum).x;
        var y = ai_generator.KillerPopCell(_selectStageNum).y;
        ai_generator.KillerPopNodeCell((int)x, (int)y);
    }

    public void GameStart()
    {
        StartCoroutine(Callback(1.0f, () =>
        {
            GetComponent<GameManager>().CurrentGameState = GameState.GAMEMAIN;

            _aiGenerator.MoveStartHumans();
        }));
    }

    private IEnumerator Callback(float time, Action action)
    {
        yield return new WaitForSeconds(time);
        action();
    }

    public void StageResetStart()
    {
        if (_isReset == false)
        {
            _isReset = true;
            StartCoroutine(StageResetStaging());
        }
    }

    private IEnumerator StageResetStaging()
    {
        yield return new WaitForSeconds(0.2f);

        while (true)
        {
            yield return null;

            var color = _fade.color;
            color.a += Time.deltaTime * 2;
            color.a = Mathf.Clamp(color.a, 0, 1);
            _fade.color = color;

            if (color.a < 1)
                continue;

            StageSetup();
            CameraSetup();
            GameStart();
            KillerSetup();

            SoundManager.Instance.StopBGM();
            SoundManager.Instance.PlayBGM("ingame");

            _camera.GetComponent<Rotater>().Setup();
            _player.GetComponent<Rotater>().Setup();

            while (_fade.color.a > 0.0f)
            {
                color.a -= Time.deltaTime * 2;
                _fade.color = color;
                yield return null;
            }

            _isReset = false;
            break;
        }
    }
}