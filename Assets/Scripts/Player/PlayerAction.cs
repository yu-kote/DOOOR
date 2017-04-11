using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAction : MonoBehaviour
{
	// プレイヤーがマップに対してアクションを起こす時に押すボタン
	[SerializeField]
	private string _actionButton = "Action";
	//選択しているトラップのタイプ
	[SerializeField]
	private TrapType _selectTrapType = TrapType.PITFALLS;

	private TrapSpawnManager _trapSpawnManager = null;

	void Start()
	{
		_trapSpawnManager
			= GameObject.Find("TrapSpawnManager").GetComponent<TrapSpawnManager>();
		if (_trapSpawnManager == null)
			Debug.Log("_trapSpawnManager is null");
	}

	//プレイヤーのトリガーの範囲内に入ったノードのトラップステータスの情報を見て
	//今選択しているトラップが設置できる場合生成する
	public void OnTriggerStay(Collider other)
	{
		//ボタン押してなかったらはじく
		if (!Input.GetButtonDown(_actionButton))
			return;

		TrapStatus trapStatus = other.GetComponent<TrapStatus>();

		if (trapStatus == null)
			return;

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
		_trapSpawnManager.SpawnTrap(_selectTrapType, other.transform);
		//今の所一つのノードに対して一つのトラップしか仕掛けれない状態にしている
		trapStatus.IsSpawn = true;
		//ノードに対して何が設置されたか保存しときたい
		//未実装
	}
	
}
