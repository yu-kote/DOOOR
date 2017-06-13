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
                CraftTheInstallation(other.gameObject);
    }

    public bool IsDoorLock()
    {
        return Input.GetButton(_actionButton);
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
        _trapSpawnManager.SpawnTrap(_selectTrapType, node.transform);
        //今の所一つのノードに対して一つのトラップしか仕掛けれない状態にしている
        trapStatus.IsSpawn = true;

        // リキャストを開始する
        _trapSelectUi.TrapRecast(cross_direction);
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
