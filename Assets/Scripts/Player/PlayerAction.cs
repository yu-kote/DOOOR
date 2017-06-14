﻿using System.Collections;
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

    // ドアを固定したときに出るやつ
    GameObject _doorLock;

    // 音を出す
    [SerializeField]
    private AISoundManager _aiSoundManager;
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

        // 音だけどこでも鳴らせるので特殊処理
        var value = _trapSelectUi.PushValue;
        if (value == TrapDirection.LEFT)
        {
            if (_trapSelectUi.TrapRecast(value) == false)
                return;
            _aiSoundManager.MakeSound(gameObject, gameObject.transform.position, 20, 1);
            SoundManager.Instance.PlaySE("otodasu", gameObject);
        }

        if (value == TrapDirection.RIGHT)
        {
            if (_trapSelectUi.TrapRecast(value) == false)
                return;
            _mapBackgrounds.LightAllControll(false);
            StartCoroutine(CallBack(_trapSelectUi.GetRecastTime(TrapDirection.RIGHT),
               () => _mapBackgrounds.LightAllControll(true)));
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

        if (IsDoorLock())
            if (other.tag == "Attribute")
            {
                CraftTheInstallation(other.gameObject);
                return;
            }

        if (other.tag == "Attribute")
        {
            if (other.name == "Door" + "(Clone)" || other.name.Contains("Stairs"))
            {
                _buttonGuide.SetActive(true);
                if (_isButtonGuideView != _buttonGuide.activeInHierarchy)
                    ButtonGuideStart(other.gameObject);

                ButtonGuideUpdate(other.gameObject);
            }
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
        // 音か停電だったらはじく
        if (cross_direction == TrapDirection.LEFT || cross_direction == TrapDirection.RIGHT)
            return;
        // リキャストが終わってなかったらはじく
        if (_trapSelectUi.CanUseTrap(cross_direction) == false)
            return;

        if (cross_direction == TrapDirection.UP)
            _selectTrapType = TrapType.PITFALLS;
        if (cross_direction == TrapDirection.DOWN)
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

            stairs1.LockStairsStatus(_statusLockTime);
            var complate = stairs2.LockStairsStatus(_statusLockTime);
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
        //var distance = _camera.transform.position - door_node.transform.position;
        //var facade_dir = distance.normalized * 10;
        //var offset_value = facade_dir.x;
        //if (Mathf.Abs(facade_dir.x) < Mathf.Abs(facade_dir.z))
        //    offset_value = facade_dir.z;
        //if (offset_value > 0)
        //    offset_value *= -1;

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
    }

    private Vector3 CameraDistance()
    {
        return _camera.transform.position - transform.position;
    }

    private Vector3 FieldUiAngle()
    {
        return _startEulerAngle + transform.eulerAngles;
    }

    private void OnDestroy()
    {
    }
}
