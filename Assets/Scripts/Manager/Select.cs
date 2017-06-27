using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

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
    private GameObject _trapCrossOperation;
    [SerializeField]
    private TrapSelectUI _trapSelectUi;
    [SerializeField]
    private GameObject _helpButtonGuide;

    [SerializeField]
    private Text _stageNum;
    [SerializeField]
    private GameObject _pushStartBar;
    [SerializeField]
    private Image _startButton;

    [SerializeField]
    private GameObject _rightArrow;
    [SerializeField]
    private GameObject _leftArrow;

    Vector3 _rightArrowStartPos;
    Vector3 _leftArrowStartPos;


    [SerializeField]
    private int _stageMin;
    [SerializeField]
    private int _stageMax;

    [SerializeField]
    private Image _fade;
    [SerializeField]
    private GameObject _clearMark;
    [SerializeField]
    private Text _stageGuideText;
    [SerializeField]
    private Image _help;

    [SerializeField]
    private string _backToTitleButton = "Sound";
    [SerializeField]
    string _horizontalAxis = "Horizontal";
    bool _isAxisDown;
    [SerializeField]
    private string _leftRotateButton = "L1";
    [SerializeField]
    private string _rightRotateButton = "R1";


    private NodeManager _nodeManager;
    private GameManager _gameManager;
    private Reloader _reloader;

    int _selectStageNum;
    int _currentSelectStageNum;

    bool _isSelectEnd;
    int _selectDirection;

    void Awake()
    {
        var field = GameObject.Find("Field");
        _nodeManager = field.GetComponent<NodeManager>();

        _gameManager = GetComponent<GameManager>();
        _reloader = GetComponent<Reloader>();

        _isSelectEnd = false;
        _isAxisDown = false;

        _camera.transform.position += new Vector3(0, 0, -20);

        _trapCrossOperation.SetActive(true);

        // ステージ数の初期化
        _selectStageNum = ShareData.Instance.SelectStage;
        _currentSelectStageNum = _selectStageNum;

        // ヘルプの初期化
        _help.color = new Color(1, 1, 1, 0);
        _gameManager.SetImageChildColor(_help, _help.color);
        _help.gameObject.SetActive(true);

        // 左右の矢印の初期化
        _rightArrowStartPos = _rightArrow.transform.localPosition;
        _leftArrowStartPos = _leftArrow.transform.localPosition;

        // クリアマークの初期化
        _clearMark.SetActive(false);
        // ステージ解放のガイドコメント初期化
        _stageGuideText.text = "";
    }

    private void Start()
    {
        SoundManager.Instance.PlayBGM("title");

        StartCoroutine(Setup());
    }

    private IEnumerator Setup()
    {
        yield return null;
        _reloader.StageSetup(_selectStageNum);
        CameraSetup();
        ClearCheck(_selectStageNum);
        StartCoroutine(StageSelectStart());
    }

    // すぐにゲームを開始できないように少しガードして、
    // ステージを切り替えたときにフェードする
    private IEnumerator StageSelectStart()
    {
        yield return new WaitForSeconds(1.0f);

        while (true)
        {
            yield return null;

            var color = _fade.color;
            color.a -= Time.deltaTime * 2;

            var alpha_min = 0.0f;
            var bitblack_alpha = 0.8f;
            if (color.a <= bitblack_alpha)
                if (_selectStageNum > ShareData.Instance.CanSelectStageMax)
                {
                    alpha_min = bitblack_alpha;
                }
            color.a = Mathf.Clamp(color.a, alpha_min, 1.0f);
            _fade.color = color;

            if (MapSelect() == false)
                continue;

            while (_fade.color.a < 1.0f)
            {
                color.a += Time.deltaTime * 2;
                _fade.color = color;
                yield return null;
            }

            // ステージを切り替える
            _reloader.StageSetup(_selectStageNum);
            CameraSetup();
            ClearCheck(_selectStageNum);
            CanStartChangeUI(_selectStageNum);
        }
    }

    void CameraSetup()
    {
        // カメラの位置をフィールドの中心に合わせる
        var mover = _camera.GetComponent<CameraMover>();
        var camera_pos = _nodeManager.GetNodesCenterPoint();
        _camera.transform.position = new Vector3(camera_pos.x, camera_pos.y, _camera.transform.position.z);
        _camera.transform.eulerAngles = mover.StartAngle;
    }

    void ClearCheck(int stage_num)
    {
        _clearMark.SetActive(false);

        // クリア済みのステージならクリアマークを出す
        if (ShareData.Instance.ClearStages.FirstOrDefault(num => num == stage_num) == 0)
            return;
        _clearMark.SetActive(true);
    }

    void CanStartChangeUI(int stage_num)
    {
        if (_selectStageNum <= ShareData.Instance.CanSelectStageMax)
        {
            _pushStartBar.SetActive(true);
            _stageGuideText.text = "";
        }
        else
        {
            var t = "ステージ";
            if (stage_num == 2) t = "Stage" + 1 + "をクリアすることで解放できます";
            if (stage_num == 3) t = "Stage" + 2 + "をクリアすることで解放できます";
            if (stage_num == 4) t = "Stage" + 3 + "をクリアすることで解放できます";
            if (stage_num == 5) t = "Stage" + 4 + "をクリアすることで解放できます";
            if (stage_num == 6) t = "Stage" + 5 + "をクリアすることで解放できます";
            if (stage_num == 7) t = "Stage" + 5 + "をクリアすることで解放できます";
            if (stage_num == 8) t = "Stage" + 5 + "をクリアすることで解放できます";
            if (stage_num == 9) t = "Stage" + 5 + "をクリアすることで解放できます";
            if (stage_num == 10) t = "Stage" + 5 + "をクリアすることで解放できます";
            if (stage_num == 11) t = "Stage" + 5 + "をクリアすることで解放できます";
            if (stage_num == 12) t = "Stage" + 5 + "をクリアすることで解放できます";
            if (stage_num == 13) t = "Stage" + 5 + "をクリアすることで解放できます";


            _stageGuideText.text = t;
            _pushStartBar.SetActive(false);
        }
    }

    void Update()
    {
        if (_gameManager.CurrentGameState != GameState.SELECT)
        {
            _selectCanvas.SetActive(false);
            return;
        }

#if DEBUG
        if (Input.GetKeyDown(KeyCode.T))
            GameObject.Find("SceneChanger").GetComponent<SceneChanger>()
                .SceneChange("Title", () => SoundManager.Instance.StopBGM());
#endif

        BackToTitle();
        ArrowEffect();

        // テキスト更新
        StageNumTextupdate();

        // カメラが寄る演出が終わったら操作説明を出す
        if (_isSelectEnd == true)
            if (EasingInitiator.IsEaseEnd(gameObject))
                GameStart();
        //StartCoroutine(OperationDraw());

        _selectCanvas.SetActive(true);
    }

    // 操作説明をゲーム開始直後に表示する
    private IEnumerator OperationDraw()
    {
        yield return null;

        while (true)
        {
            yield return null;
            var color = _help.color;
            color.a += Time.deltaTime * 2;
            color.a = Mathf.Clamp(color.a, 0, 1);
            _help.color = color;

            if (_gameManager.IsPushActionButton() == false &&
                Input.GetKeyDown(KeyCode.Return) == false)
                continue;

            while (_help.color.a > 0.0f)
            {
                color.a -= Time.deltaTime * 3;
                _help.color = color;
                yield return null;
            }
            break;
        }
        yield return null;

        GameStart();
    }

    // ゲームを開始する
    private void GameStart()
    {
        SoundManager.Instance.StopBGM();
        SoundManager.Instance.PlayBGM("ingame");
        _gameManager.CurrentGameState = GameState.GAMEMAIN;

        _helpButtonGuide.SetActive(false);
        if (_selectStageNum == 1)
        {
            _trapCrossOperation.SetActive(false);
            _trapSelectUi.SetEnableTrap(false, false, false, false);
            _helpButtonGuide.SetActive(true);
        }
        else if (_selectStageNum == 2)
            _trapSelectUi.SetEnableTrap(false, true, false, false);
        else if (_selectStageNum == 3)
            _trapSelectUi.SetEnableTrap(false, true, true, true);
        else if (_selectStageNum == 4)
            _trapSelectUi.SetEnableTrap(true, true, true, true);
        else
            _trapSelectUi.SetEnableTrap();

        _reloader.KillerSetup();

        ShareData.Instance.SelectStage = _selectStageNum;
    }

    private void BackToTitle()
    {
        if (Input.GetButtonDown(_backToTitleButton))
        {
            GameObject.Find("SceneChanger").GetComponent<SceneChanger>()
                .SceneChange("Title", () => SoundManager.Instance.StopBGM());
        }
    }

    private bool MapSelect()
    {
        if (_isSelectEnd)
            return false;

        // ステージ選択が終わったら演出する
        if (_selectStageNum <= ShareData.Instance.CanSelectStageMax)
            if (_gameManager.IsPushActionButton() || Input.GetKeyDown(KeyCode.Return))
            {
                _isSelectEnd = true;
                SelectEndStaging();
                StartButtonChange();
                SoundManager.Instance.ChoiceSE();
            }

        // スティック
        float horizotal = Input.GetAxis("Horizontal");
        if (horizotal > 0.5f)
        {
            if (_selectDirection == -1 || _selectDirection == 0)
            {
                _selectDirection = 1;
                _selectStageNum++;
            }
        }
        else if (horizotal < -0.5f)
        {
            if (_selectDirection == 1 || _selectDirection == 0)
            {
                _selectDirection = -1;
                _selectStageNum--;
            }
        }
        if (horizotal < 0.1f && horizotal > -0.1f)
            _selectDirection = 0;

        // キーボードの矢印
        if (Input.GetKeyDown(KeyCode.RightArrow))
            _selectStageNum++;
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            _selectStageNum--;

        // コントローラー十字キー
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

        // コントローラーLRボタン
        if (Input.GetButton(_rightRotateButton))
            _selectStageNum++;
        if (Input.GetButton(_leftRotateButton))
            _selectStageNum--;

        // 0はタイトルステージなので、1 ~ max 
        _selectStageNum = Mathf.Clamp(_selectStageNum, _stageMin,
                                      ShareData.Instance.StageMax);

        if (_currentSelectStageNum == _selectStageNum)
            return false;
        _currentSelectStageNum = _selectStageNum;

        // 選択音
        SoundManager.Instance.PlaySE("sentakuon");

        return true;
    }

    public void SelectEndStaging()
    {
        StartCoroutine(ClearMarkStaging());
        float staging_time = 3.0f;
        EasingInitiator.Wait(gameObject, staging_time);
        EasingInitiator.Add(_selectCanvas, new Vector3(0, 0, -600), staging_time, EaseType.CubicOut);

        var player_pos = _player.transform.position;
        EasingInitiator.Add(_camera, player_pos + new Vector3(0, 0, -20), staging_time, EaseType.CubicOut);

        var default_angle = new Vector3(0, 0, 360);
        EasingInitiator.Add(_camera, default_angle, staging_time, EaseType.CubicOut, EaseValue.ROTATION);
        EasingInitiator.Add(_frame, new Vector3(0, 0, -15), staging_time, EaseType.CubicOut, EaseValue.ROTATION);
    }

    private IEnumerator ClearMarkStaging()
    {
        var image = _clearMark.GetComponent<Image>();
        var color = image.color;
        while (color.a > 0)
        {
            color.a -= Time.deltaTime;
            image.color = color;
            yield return null;
        }
        _clearMark.SetActive(false);
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

    float _effectTime = 0.0f;
    private void ArrowEffect()
    {
        if (_selectStageNum <= _stageMin)
            _leftArrow.SetActive(false);
        else if (_selectStageNum > _stageMin)
            _leftArrow.SetActive(true);

        if (_selectStageNum >= ShareData.Instance.StageMax)
            _rightArrow.SetActive(false);
        else if (_selectStageNum < ShareData.Instance.StageMax)
            _rightArrow.SetActive(true);

        _effectTime += 10 * Time.deltaTime;
        float x = Mathf.Sin(1.0f + 0.4f * _effectTime) * 10.0f;

        _rightArrow.transform.localPosition =
            _rightArrowStartPos + new Vector3(x, 0, 0);

        _leftArrow.transform.localPosition =
            _leftArrowStartPos + new Vector3(-x, 0, 0);
    }
}
