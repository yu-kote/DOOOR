using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LastKey : ItemBase
{
	void Start()
	{
		_itemID = ItemID.LASTKEY;
		if (_isInstanceAttribute)
			CreateItem("LastKey");
	}
}
