using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : ItemBase
{
	void Start()
	{
		_itemID = ItemID.KEY;
		if (_isInstanceAttribute)
			CreateItem("Key");
	}
}
