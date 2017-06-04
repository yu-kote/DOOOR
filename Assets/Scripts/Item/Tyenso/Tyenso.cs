using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tyenso : ItemBase {

	void Start()
	{
		_type = ItemType.TYENSO;
		if (_isInstanceAttribute)
			CreateItem("Tyenso");
	}
}
