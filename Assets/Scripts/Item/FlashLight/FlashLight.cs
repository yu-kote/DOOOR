using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashLight : ItemBase
{
	void Start()
	{
		_type = ItemType.FLASHLIGHT;
		if (_isInstanceAttribute)
			CreateItem("FlashLight");
	}
}
