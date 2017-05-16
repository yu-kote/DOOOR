using System.Collections;
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
		for (; id > 0; id = id >> 1)
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

					//gameObject.AddComponent<FlashLight>();
					break;
				case ItemType.GUN:

					//gameObject.AddComponent<Gun>();
					break;
			}
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
			//case ItemID.FLASHLIGHT:

			//	return gameObject.GetComponent<FlashLight>().AcquiredItem();
			//case ItemID.GUN:

			//	return gameObject.GetComponent<Gun>().AcquiredItem();
		}

		return ItemType.NONE;
	}
}
