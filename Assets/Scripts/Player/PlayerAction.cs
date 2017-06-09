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
    private string _upButton = "Up";
    [SerializeField]
    private string _downButton = "Down";
    [SerializeField]
    private string _soundButton = "Sound";

    [SerializeField]
    string _horizontalAxis = "CrossHorizontal";
    [SerializeField]
    string _verticalAxis = "CrossVertical";
    [SerializeField]
    TrapSelectUI _trapSelectUi;

    //選択しているトラップのタイプ
    [SerializeField]
    private TrapType _selectTrapType = TrapType.PITFALLS;
    public TrapType SelectTrapType
    {
        get { return _selectTrapType; }
        set { _selectTrapType = value; }
    }
    //ドアの状態固定時間
    [SerializeField]
    private float _statusLockTime = 2.0f;

    [SerializeField]
    private float[] _recastTime;

    public class TrapUseStatus
    {
        public float RecastTime = 0.0f;
        public bool CanUse = true;
    }
    TrapUseStatus[] _trapUseStatus = new TrapUseStatus[4];

    // 音を出す
    [SerializeField]
    private AISoundManager _aiSoundManager;

    private TrapSpawnManager _trapSpawnManager = null;

    void Start()
    {
        _trapSpawnManager
            = GameObject.Find("TrapSpawnManager").GetComponent<TrapSpawnManager>();
        if (_trapSpawnManager == null)
            Debug.Log("_trapSpawnManager is null");

        for (int i = 0; i < _trapUseStatus.Count(); i++)
            _trapUseStatus[i] = new TrapUseStatus();

        // リキャスト時間を初期化する
        for (int i = 0; i < _recastTime.Count(); i++)
            _trapUseStatus[i].RecastTime = _recastTime[i];
    }

    void Update()
    {
        if (GameObject.Find("GameManager").GetComponent<GameManager>().CurrentGameState
            != GameState.GAMEMAIN)
            return;

        // 音だけどこでも鳴らせるので特殊処理
        var value = _trapSelectUi.PushValue();

        if (value == TrapDirection.LEFT)
        {
            var num = (int)value - 1;
            if (_trapUseStatus[num].CanUse == false)
                return;

            _trapUseStatus[num].CanUse = false;
            Callback(_trapUseStatus[num].RecastTime,
                     () => _trapUseStatus[num].CanUse = true);
            _aiSoundManager.MakeSound(gameObject, gameObject.transform.position, 20, 1);
            SoundManager.Instance.PlaySE("otodasu", gameObject);
        }
    }

    // 指定秒数後に関数を呼ぶ
    private void Callback(float time, Action action)
    {
        Observable.Timer(TimeSpan.FromSeconds(time)).Subscribe(_ =>
        {
            action();
        }).AddTo(gameObject);
    }

    //プレイヤーのトリガーの範囲内に入ったノードのトラップステータスの情報を見て
    //今選択しているトラップが設置できる場合生成する
    public void OnTriggerStay(Collider other)
    {
        if (GameObject.Find("GameManager").GetComponent<GameManager>().CurrentGameState
            != GameState.GAMEMAIN)
            return;

        var value = _trapSelectUi.PushValue();
        //ボタン押してなかったらはじく
        if (value != TrapDirection.NONE)
            if (other.tag == "Node")
                CreateTrap(other.gameObject, value);

        if (IsDoorLock())
            if (other.tag == "Attribute")
                CraftTheInstallation(other.gameObject);
    }

    public bool IsDoorLock()
    {
        return Input.GetButton(_actionButton);
    }

    private void CreateTrap(GameObject node, TrapDirection cross_direction)
    {
        // 音だったら鳴らしてはじく
        if (cross_direction == TrapDirection.LEFT)
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

        var num = (int)cross_direction - 1;
        if (_trapUseStatus[num].CanUse == false)
            return;

        _trapUseStatus[num].CanUse = false;
        Callback(_trapUseStatus[num].RecastTime,
                 () => _trapUseStatus[num].CanUse = true);

        //トラップ生成
        _trapSpawnManager.SpawnTrap(_selectTrapType, node.transform);
        //今の所一つのノードに対して一つのトラップしか仕掛けれない状態にしている
        trapStatus.IsSpawn = true;
    }

    private void CraftTheInstallation(GameObject attribute)
    {
        switch (attribute.name)
        {
            case "Door" + "(Clone)":

                attribute.transform.parent.GetComponent<Door>().LockDoorStatus(_statusLockTime);
                break;
        }
    }

}
