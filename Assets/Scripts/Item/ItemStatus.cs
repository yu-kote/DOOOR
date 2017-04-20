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


	public void AddPutItem(uint id)
	{
		for (; id > 0; id = id >> 1)
		{
			if ((_puttingItemStatus & (id - (id >> 1))) != 0)
			{
				Debug.Log("すでにおかれています");
				return;
			}

			_puttingItemStatus += (id - (id >> 1));

			switch ((ItemID)id)
			{
				case ItemID.KEY:

					gameObject.AddComponent<Key>();
					break;

				case ItemID.LASTKEY:

					gameObject.AddComponent<LastKey>();
					break;
				case ItemID.FLASHLIGHT:

					//gameObject.AddComponent<FlashLight>();
					break;
				case ItemID.GUN:

					//gameObject.AddComponent<Gun>();
					break;
			}
		}
		
	}

	public ItemID AcquiredItem(ItemID id)
	{
		if ((_puttingItemStatus & (uint)id) == 0)
		{
			Debug.Log("すでにおかれています");
			return ItemID.NONE;
		}

		_puttingItemStatus -= (uint)id;

		switch (id)
		{
			case ItemID.KEY:

				return gameObject.GetComponent<Key>().AcquiredItem();
			case ItemID.LASTKEY:

				return gameObject.GetComponent<LastKey>().AcquiredItem();
			//case ItemID.FLASHLIGHT:

			//	return gameObject.GetComponent<FlashLight>().AcquiredItem();
			//case ItemID.GUN:

			//	return gameObject.GetComponent<Gun>().AcquiredItem();
		}

		return ItemID.NONE;
	}
}
