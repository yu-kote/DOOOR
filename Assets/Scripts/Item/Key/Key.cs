using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : ItemBase
{
	void Start()
	{
		_type = ItemType.KEY;
		if (_isInstanceAttribute)
			CreateItem("Key");
	}
}
