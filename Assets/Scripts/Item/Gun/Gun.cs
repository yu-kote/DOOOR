using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : ItemBase {

	void Start()
	{
		_type = ItemType.GUN;
		if (_isInstanceAttribute)
			CreateItem("Gun");
	}
}
