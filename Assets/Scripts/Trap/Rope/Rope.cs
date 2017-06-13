using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour
{
	public FootPrint _footPrint = null;

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

		//同時に引っかかった時のためにfor文
		int overturnNum = 0;
		for (int i = 0; i < _footPrint.HumansOnNode.Count; i++)
		{
			if (_footPrint.HumansOnNode[i].tag == "Killer")
				continue;

			if (_footPrint.HumansOnNode[i].GetComponent<AIController>().GetMovement().MoveComplete())
			{
				_footPrint.HumansOnNode[i].GetComponent<AITrapEffect>().ToOverturn();
				overturnNum++;
                _footPrint.gameObject.GetComponent<TrapStatus>().IsSpawn = false;
            }
				
		}

		if (overturnNum == 0)
			return;

		//今のところ一度発動したら消えるようにしている
		Destroy(gameObject);
	}
}
