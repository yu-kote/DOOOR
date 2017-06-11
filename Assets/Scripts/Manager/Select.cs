using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Select : MonoBehaviour
{
    [SerializeField]
    private GameObject _selectCanvas;
    [SerializeField]
    private GameObject _frame;
    [SerializeField]
    private GameObject _camera;
    [SerializeField]
    private GameObject _player;

    [SerializeField]
    private Text _stageNum;
    [SerializeField]
    private Image _startButton;

    [SerializeField]
    string _horizontalAxis = "Horizontal";
    bool _isAxisDown;

    private Vector3 _cameraStartPos;

    private MapLoader _mapLoader;
    private NodeManager _nodeManager;
    private MapBackgrounds _mapBackgrounds;
    private AIGenerator _aiGenerator;
    private GameManager _gameManager;

    int _selectStageNum;
    int _currentSelectStageNum;

    bool _isSelectEnd;

    void Awake()
    {
        _selectStageNum = 1;
        _currentSelectStageNum = _selectStageNum;

        var field = GameObject.Find("Field");
        _nodeManager = field.GetComponent<NodeManager>();
        _mapLoader = field.GetComponent<MapLoader>();
        _mapBackgrounds = field.GetComponent<MapBackgrounds>();

        var human_manager = GameObject.Find("HumanManager");
        _aiGenerator = human_manager.GetComponent<AIGenerator>();

        _gameManager = GetComponent<GameManager>();

        _isSelectEnd = false;
        _isAxisDown = false;

        _cameraStartPos = _camera.transform.position;
        _camera.transform.position += new Vector3(0, 0, -20);
    }

    private void Start()
    {
        SoundManager.Instance.PlayBGM("title");
        StageSetup();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
            GameObject.Find("SceneChanger")
                .GetComponent<SceneChanger>().SceneChange("Title");

        if (_isSelectEnd == true)
        {
            if (EasingInitiator.IsEaseEnd(gameObject))
            {
                GetComponent<GameManager>().CurrentGameState = GameState.GAMEMAIN;
                Destroy(this);
                SoundManager.Instance.StopBGM();
                SoundManager.Instance.PlayBGM("ingame");
            }
        }

        if (GetComponent<GameManager>().CurrentGameState != GameState.SELECT)
        {
            _selectCanvas.SetActive(false);
            return;
        }
        _selectCanvas.SetActive(true);

        MapSelect();
    }

    private void MapSelect()
    {
        if (_isSelectEnd)
            return;

        // ステージ選択が終わったら演出する
        if (_gameManager.IsPushActionButton() || Input.GetKeyDown(KeyCode.Return))
        {
            _isSelectEnd = true;
            SelectEndStaging();
            StartButtonChange();
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
            _selectStageNum++;
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            _selectStageNum--;

        if (_isAxisDown == false)
        {
            if (Input.GetAxis(_horizontalAxis) == 1.0f)
            {
                _isAxisDown = true;
                _selectStageNum++;
            }
            else if (Input.GetAxis(_horizontalAxis) == -1.0f)
            {
                _isAxisDown = true;
                _selectStageNum--;
            }
        }
        if (Input.GetAxis(_horizontalAxis) == 0.0f)
            _isAxisDown = false;

        // 0はタイトルステージなので、1 ~ max 
        _selectStageNum = Mathf.Clamp(_selectStageNum, 1, 3);

        // テキスト更新
        StageNumTextupdate();

        if (_currentSelectStageNum == _selectStageNum)
            return;
        _currentSelectStageNum = _selectStageNum;

        // 選択音
        SoundManager.Instance.PlaySE("sentakuon");

        StageSetup();
    }

    private void StageSetup()
    {
        // マップを読み直す
        ChangeMap();

        // 人間を表示する
        _aiGenerator.InstanceHumans(_selectStageNum);

        // プレイヤーを扉の位置に移動させる
        PlayerPositionOffset();

        // カメラの位置をフィールドの中心に合わせる
        var camera_pos = _nodeManager.GetNodesCenterPoint();
        _camera.transform.position = new Vector3(camera_pos.x, camera_pos.y, _camera.transform.position.z);
    }

    public void ChangeMap()
    {
        _mapLoader.LoadMap(_selectStageNum);
        _nodeManager.Start();
    }

    private void PlayerPositionOffset()
    {
        _player.transform.position
            = new Vector3(_aiGenerator.StartNode.transform.position.x,
                          _aiGenerator.StartNode.transform.position.y + 10,
                          _player.transform.position.z);
    }

    public void SelectEndStaging()
    {
        float staging_time = 3.0f;
        EasingInitiator.Wait(gameObject, staging_time);
        EasingInitiator.Add(_selectCanvas, new Vector3(0, 0, -600), staging_time, EaseType.CubicOut);

        var player_pos = _player.transform.position;
        EasingInitiator.Add(_camera, player_pos + new Vector3(0, 0, -20), staging_time, EaseType.CubicOut);

        var default_angle = new Vector3(0, 0, 360);
        EasingInitiator.Add(_camera, default_angle, staging_time, EaseType.CubicOut, EaseValue.ROTATION);
        EasingInitiator.Add(_frame, new Vector3(0, 0, -15), staging_time, EaseType.CubicOut, EaseValue.ROTATION);
    }

    private void StageNumTextupdate()
    {
        _stageNum.text = "Stage " + _selectStageNum;
    }

    private void StartButtonChange()
    {
        var push = Resources.Load<Sprite>("Texture/SelectUI/s-botan02");
        _startButton.sprite = push;
    }
}
