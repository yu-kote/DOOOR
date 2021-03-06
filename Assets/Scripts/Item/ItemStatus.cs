﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemStatus : MonoBehaviour
{
	private uint _puttingItemStatus = 0;
	public uint PuttingItemStatus
	{
		get { return _puttingItemStatus; }
		set { _puttingItemStatus = value; }
	}

	public ItemType GetItem()
	{
		return (ItemType)_puttingItemStatus;
	}

	public void AddPutItem(uint id)
	{

		if ((_puttingItemStatus & (id - (id >> 1))) != 0)
		{
			Debug.Log("すでにおかれています");
			return;
		}

		_puttingItemStatus += (id);

		switch ((ItemType)id)
		{
			case ItemType.KEY:

				gameObject.AddComponent<Key>();
				break;

			case ItemType.LASTKEY:

				gameObject.AddComponent<LastKey>();
				break;
			case ItemType.FLASHLIGHT:

				gameObject.AddComponent<FlashLight>();
				break;
			case ItemType.GUN:

				gameObject.AddComponent<Gun>();
				break;

			case ItemType.TYENSO:

				gameObject.AddComponent<Tyenso>();
				break;
		}

	}

	public ItemType AcquiredItem(ItemType id)
	{
		if ((_puttingItemStatus & (uint)id) == 0)
		{
			Debug.Log("すでにおかれています");
			return ItemType.NONE;
		}

		_puttingItemStatus -= (uint)id;

		switch (id)
		{
			case ItemType.KEY:

				return gameObject.GetComponent<Key>().AcquiredItem();
			case ItemType.LASTKEY:

				return gameObject.GetComponent<LastKey>().AcquiredItem();
			case ItemType.FLASHLIGHT:

				return gameObject.GetComponent<FlashLight>().AcquiredItem();
			case ItemType.GUN:

				return gameObject.GetComponent<Gun>().AcquiredItem();

			case ItemType.TYENSO:

				return gameObject.GetComponent<Tyenso>().AcquiredItem();
		}

		return ItemType.NONE;
	}
}
