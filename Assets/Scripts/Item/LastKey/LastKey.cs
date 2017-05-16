using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LastKey : ItemBase
{
	void Start()
	{
		_type = ItemType.LASTKEY;
		if (_isInstanceAttribute)
			CreateItem("LastKey");
	}
}
