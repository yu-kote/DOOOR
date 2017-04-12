﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapSpawnManager : MonoBehaviour
{
	private TrapList _trapList = null;

	void Start()
	{
		_trapList = GameObject.Find("TrapList").GetComponent<TrapList>();
		if (_trapList == null)
			Debug.Log("_trapList is null");
	}

	public void SpawnTrap(TrapType type, Transform nodeTrans)
	{
		//TrapTypeがNONEだった場合はじく
		if (type == TrapType.NONE)
			return;

		GameObject _trapObject = Instantiate(_trapList.GetTrapObject(type));
		_trapObject.transform.position = nodeTrans.position;
		switch (type)
		{
			case TrapType.PITFALLS:
				_trapObject.GetComponent<PitFall>()._footPrint
					= nodeTrans.GetComponent<FootPrint>();
				_trapObject.GetComponent<PitFall>().NodeCell
					= new Vector2(nodeTrans.GetComponent<Node>().CellX,
					nodeTrans.GetComponent<Node>().CellY);
				break;
			case TrapType.ROPE:
				_trapObject.GetComponent<Rope>()._footPrint
					= nodeTrans.GetComponent<FootPrint>();
				break;

			case TrapType.NONE:
				break;
		}
	}
}
