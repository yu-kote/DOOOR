using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAction : MonoBehaviour
{
	// プレイヤーがマップに対してアクションを起こす時に押すボタン
	[SerializeField]
	private string _actionButton = "Action";
	[SerializeField]
	private string _upButton = "Up";
	[SerializeField]
	private string _downButton = "Down";
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
		if (Input.GetButtonDown(_upButton))
		{
			_selectTrapType = (TrapType)(Mathf.Max(1, ((uint)_selectTrapType >> 1)));
			Debug.Log("up");
		}
		if (Input.GetButtonDown(_downButton))
		{
			_selectTrapType = (TrapType)(Mathf.Min((uint)TrapType.MAX >> 1, ((uint)_selectTrapType << 1)));
			Debug.Log("down");
		}
	}

	//プレイヤーのトリガーの範囲内に入ったノードのトラップステータスの情報を見て
	//今選択しているトラップが設置できる場合生成する
	public void OnTriggerStay(Collider other)
	{
		//ボタン押してなかったらはじく
		if (!Input.GetButtonDown(_actionButton))
			return;

		if (other.tag == "Node")
			CreateTrap(other.gameObject);
		else if (other.tag == "Attribute")
			CraftTheInstallation(other.gameObject);
	}

	private void CreateTrap(GameObject node)
	{
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
