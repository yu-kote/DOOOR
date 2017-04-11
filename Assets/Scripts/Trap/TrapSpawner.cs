using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapSpawner : MonoBehaviour
{
	private TrapStatus _trapStatus = null;
	private TrapList _trapList = null;

	void Start()
	{
		_trapStatus = GetComponent<TrapStatus>();
		if (_trapStatus == null)
			Debug.Log("_trapStatus is null");
		_trapList = GameObject.Find("TrapList").GetComponent<TrapList>();
		if (_trapList == null)
			Debug.Log("_trapList is null");
	}

	public void SpawnTrap()
	{
		//すでに生成されていたらはじく
		if (_trapStatus.IsSpawn)
			return;
		//TrapTypeがNONEだった場合はじく
		if (_trapStatus.Type == TrapType.NONE)
			return;

		GameObject _trapObject = Instantiate(_trapList.GetTrapObject(_trapStatus.Type));
		_trapObject.transform.position = transform.position;
		switch (_trapStatus.Type)
		{
			case TrapType.PITFALLS:
				_trapObject.GetComponent<PitFall>().FootPrint
					= GetComponent<FootPrint>();
				break;
			case TrapType.NONE:
				break;
		}

		_trapStatus.IsSpawn = true;
	}
}
