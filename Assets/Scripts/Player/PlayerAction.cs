using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UniRx;

public class PlayerAction : MonoBehaviour
{
    // プレイヤーがマップに対してアクションを起こす時に押すボタン
    [SerializeField]
    private string _actionButton = "Action";
    [SerializeField]
    private string _soundButton = "Sound";

    [SerializeField]
    TrapSelectUI _trapSelectUi;

    [SerializeField]
    GameObject _buttonGuide;
    bool _isButtonGuideView;
    Vector3 _startEulerAngle;

    [SerializeField]
    GameObject _camera;

    //選択しているトラップのタイプ
    [SerializeField]
    private TrapType _selectTrapType = TrapType.PITFALLS;
    public TrapType SelectTrapType { get { return _selectTrapType; } set { _selectTrapType = value; } }

    //ドアの状態固定時間
    [SerializeField]
    private float _statusLockTime = 2.0f;

    [SerializeField]
    private float _stairsLockTime = 17.0f;

    // ドアを固定したときに出るやつ
    GameObject _doorLock;

    // 音を出す
    [SerializeField]
    private AISoundManager _aiSoundManager;
    [SerializeField]
    private GameObject _voice;
    private bool _isVoiceCharge = false;
    private float _voiceRange = 5.0f;
    private float _minVoiceRange = 5.0f;
    private float _maxVoiceRange = 30.0f;
    private float _chargeCount = 0.0f;
    private float _chargeMaxTime = 2.0f;
    private bool _isVoiceRecast = false;

    [SerializeField]
    private MapBackgrounds _mapBackgrounds;

    private TrapSpawnManager _trapSpawnManager = null;

    void Start()
    {
        _trapSpawnManager
            = GameObject.Find("TrapSpawnManager").GetComponent<TrapSpawnManager>();
        if (_trapSpawnManager == null)
            Debug.Log("_trapSpawnManager is null");

        // ボタン案内の初期角度を保持する
        _startEulerAngle = _buttonGuide.transform.eulerAngles;

        _voice.transform.localScale = Vector3.zero;
    }

    private void FixedUpdate()
    {
        _isButtonGuideView = _buttonGuide.activeInHierarchy;
        _buttonGuide.SetActive(false);
    }

    void Update()
    {
        if (GameObject.Find("GameManager").GetComponent<GameManager>().CurrentGameState
            != GameState.GAMEMAIN)
            return;

        VoiceSoundUpdate();

        var value = _trapSelectUi.PushValue;
        // 停電を発動させる
        if (value == TrapDirection.UP)
        {
            if (_trapSelectUi.TrapRecast(value) == false)
                return;
            _mapBackgrounds.LightAllControll(false);
            StartCoroutine(CallBack(_trapSelectUi.GetRecastTime(TrapDirection.UP),
               () => _mapBackgrounds.LightAllControll(true)));
            GetComponent<PlayerAnimation>().ChangeAnimation(PlayerAnimationStatus.UP_TRAP, 0.6f);
        }
    }

    // 指定秒数後に関数を呼ぶ
    private IEnumerator CallBack(float time, Action action)
    {
        yield return new WaitForSeconds(time);
        action();
    }

    //プレイヤーのトリガーの範囲内に入ったノードのトラップステータスの情報を見て
    //今選択しているトラップが設置できる場合生成する
    public void OnTriggerStay(Collider other)
    {
        if (GameObject.Find("GameManager").GetComponent<GameManager>().CurrentGameState
            != GameState.GAMEMAIN)
            return;

        var value = _trapSelectUi.PushValue;
        //ボタン押してなかったらはじく
        if (value != TrapDirection.NONE)
            if (other.tag == "Node")
                CreateTrap(other.gameObject, value);

        // 階段隠しの発動
        if (value == TrapDirection.LEFT)
            if (other.tag == "Attribute")
                if (other.name.Contains("Stairs"))
                {
                    if (_trapSelectUi.TrapRecast(value) == false)
                        return;
                    CraftTheInstallation(other.gameObject);
                    return;
                }

        // ドアをロックする
        if (IsDoorLock())
            if (other.tag == "Attribute")
                if (other.name == "Door" + "(Clone)")
                {
                    CraftTheInstallation(other.gameObject);
                    return;
                }

        // ドアをロックするためのボタン案内を出す
        if (other.tag == "Attribute")
            if (other.name == "Door" + "(Clone)")
            {
                _buttonGuide.SetActive(true);
                if (_isButtonGuideView != _buttonGuide.activeInHierarchy)
                    ButtonGuideStart(other.gameObject);

                ButtonGuideUpdate(other.gameObject);
            }
    }

    public bool IsDoorLock()
    {
        return Input.GetButton(_actionButton);
    }

    public bool IsDoorLockStart()
    {
        return Input.GetButtonDown(_actionButton);
    }

    private void CreateTrap(GameObject node, TrapDirection cross_direction)
    {
        // 階段隠しか停電だったらはじく
        if (cross_direction == TrapDirection.LEFT || cross_direction == TrapDirection.UP)
            return;
        // リキャストが終わってなかったらはじく
        if (_trapSelectUi.CanUseTrap(cross_direction) == false)
            return;

        if (cross_direction == TrapDirection.DOWN)
            _selectTrapType = TrapType.PITFALLS;
        if (cross_direction == TrapDirection.RIGHT)
            _selectTrapType = TrapType.ROPE;

        TrapStatus trapStatus = node.GetComponent<TrapStatus>();

        //生成済みだった場合はじく
        if (trapStatus.IsSpawn)
            return;

        //何も設置できない場合はじく
        if (trapStatus.CanSetTrapStatus == 0)
            return;

        //設置不可能だった場合はじく
        if (!trapStatus.IsCanSetTrap(_selectTrapType))
            return;

        //トラップ生成
        _trapSpawnManager.SpawnTrap(_selectTrapType, node.transform, FieldUiAngle());
        //今の所一つのノードに対して一つのトラップしか仕掛けれない状態にしている
        trapStatus.IsSpawn = true;

        // リキャストを開始する
        _trapSelectUi.TrapRecast(cross_direction);

        if (cross_direction == TrapDirection.DOWN)
            GetComponent<PlayerAnimation>().ChangeAnimation(PlayerAnimationStatus.DOWN_TRAP, 0.6f);
        if (cross_direction == TrapDirection.RIGHT)
            GetComponent<PlayerAnimation>().ChangeAnimation(PlayerAnimationStatus.RIGHT_TRAP, 0.6f);

    }

    private void CraftTheInstallation(GameObject attribute)
    {
        if (attribute.name.Contains("Door" + "(Clone)"))
        {
            var door = attribute.transform.parent.GetComponent<Door>();
            var complete = door.LockDoorStatus(_statusLockTime);
            if (complete == false)
                return;
            door.DoorLock(LockStart(attribute.transform.parent.gameObject));
            DoorLockUpdate(door.transform.parent.gameObject);
        }

        if (attribute.name.Contains("Stairs"))
        {
            var root_node = attribute.transform.parent.gameObject.GetComponent<Node>();
            var link_node = root_node.GetComponent<Stairs>().LinkNode;


            var stairs1 = root_node.GetComponent<Stairs>();
            var stairs2 = link_node.GetComponent<Stairs>();

            stairs1.LockStairsStatus(_stairsLockTime);
            var complate = stairs2.LockStairsStatus(_stairsLockTime);
            if (complate == false)
                return;

            stairs1.StairsLock(LockStart(stairs1.Attribute.transform.GetChild(0).gameObject));
            StairsLockUpdate(root_node.transform.gameObject,
                             link_node.transform.gameObject);
        }
    }

    // ボタンの案内を出すときに一回だけ呼ぶ
    private void ButtonGuideStart(GameObject target)
    {
        var facade_pos = CameraDistance().normalized * 2;
        _buttonGuide.transform.position = target.transform.position - new Vector3(-facade_pos.x, -3, -facade_pos.z);
    }

    // ターゲットとプレイヤーの距離を測っていい感じのところを目指すようにする
    private void ButtonGuideUpdate(GameObject target)
    {
        var distance = target.transform.position - transform.position;
        var facade_dir = CameraDistance().normalized * 2;
        var offset_pos = distance.normalized + new Vector3(-facade_dir.x, -3, -facade_dir.z);

        _buttonGuide.GetComponent<Approach>().Target = target;
        _buttonGuide.GetComponent<Approach>().OffsetPos = offset_pos;

        _buttonGuide.transform.eulerAngles = FieldUiAngle();
    }

    // ドアのロックの印の準備をする
    private GameObject LockStart(GameObject target)
    {
        _doorLock = Instantiate(Resources.Load<GameObject>("Prefabs/UI/DoorLock"),
                                target.transform);
        _doorLock.transform.localPosition = Vector3.zero;
        return _doorLock;
    }

    // ドアをロックしている時に呼ぶ関数
    private void DoorLockUpdate(GameObject door_node)
    {
        // 色々試したけど時間がないのでマジナン
        var offset_pos = new Vector3(0, 10, -6);
        _doorLock.transform.localPosition = offset_pos;
        _doorLock.transform.eulerAngles = FieldUiAngle();
    }

    // 階段をロックしている時に呼ぶ関数
    private void StairsLockUpdate(GameObject node1, GameObject node2)
    {
        var offset_pos = new Vector3(0, 1, -0.3f);

        _doorLock.transform.localPosition = offset_pos;
        _doorLock.transform.eulerAngles = FieldUiAngle();
        _doorLock.transform.localScale = Vector3.one;

        GetComponent<PlayerAnimation>().ChangeAnimation(PlayerAnimationStatus.LEFT_TRAP, 0.6f);

    }

    private Vector3 CameraDistance()
    {
        return _camera.transform.position - transform.position;
    }

    private Vector3 FieldUiAngle()
    {
        return _startEulerAngle + transform.eulerAngles;
    }

    public bool IsSoundChargeStart()
    {
        if (_isVoiceRecast == false)
            if (_isVoiceCharge == false)
                if (Input.GetButton(_soundButton))
                {
                    _isVoiceCharge = true;
                    return true;
                }
        return false;
    }

    private bool IsSoundChargeEnd()
    {
        if (_isVoiceCharge)
            if (Input.GetButtonUp(_soundButton))
            {
                _isVoiceCharge = false;
                return true;
            }
        return false;
    }

    private void VoiceSoundUpdate()
    {
        // 音だけどこでも鳴らせるので特殊処理
        if (IsSoundChargeStart())
        {
            _chargeCount = 0.0f;
            _voiceRange = 5.0f;
        }

        if (_isVoiceCharge)
        {
            _chargeCount = Mathf.Clamp(_chargeCount += Time.deltaTime, 0.0f, _chargeMaxTime);

            float value = EaseCubicOut(_chargeCount / _chargeMaxTime, _minVoiceRange, _maxVoiceRange);
            _voiceRange = value;
            _voice.transform.localScale = new Vector3(value / 10.0f, 0, value / 10.0f);
        }

        if (IsSoundChargeEnd())
        {
            GetComponent<PlayerAnimation>().ChangeAnimation(PlayerAnimationStatus.USETRAP2, 0.6f);
            SoundManager.Instance.PlaySE("otodasu", gameObject);
            _aiSoundManager.MakeSound(gameObject, gameObject.transform.position,
                                      _voiceRange * 0.9f, 1, transform.eulerAngles);

            _isVoiceRecast = true;
            StartCoroutine(CallBack(1.0f, () => _isVoiceRecast = false));

            _voice.transform.localScale = Vector3.zero;
        }
    }

    static float EaseCubicOut(float t, float b, float e)
    {
        t -= 1.0f;
        return (e - b) * (t * t * t + 1) + b;
    }

    private void OnDestroy()
    {
    }
}
