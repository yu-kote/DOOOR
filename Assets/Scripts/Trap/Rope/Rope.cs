using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour
{
	private FootPrint _footPrint = null;

	void Start()
	{
		if (_footPrint == null)
			Debug.Log("_footPrint is null");
	}

	void Update()
	{
		//ノードに人が一人もいなければはじく
		if (_footPrint.HumansOnNode.Count == 0)
			return;

		//殺人鬼だったらはじくカモ
		//未実装

		//同時に引っかかった時のためにfor文
		for (int i = 0; i < _footPrint.HumansOnNode.Count; i++)
			_footPrint.HumansOnNode[i].GetComponent<AITrapEffect>().ToOverturn();

		//今のところ一度発動したら消えるようにしている
		Destroy(gameObject);
	}
}
