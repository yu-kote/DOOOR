using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapList : MonoBehaviour
{
	[SerializeField]
	private List<GameObject> _trapList = new List<GameObject>();

	void Awake()
	{
		if (_trapList.Count == 0)
			Debug.Log("_trapListがセットされてません");
	}

	public GameObject GetTrapObject(TrapType type)
	{
		if (type == TrapType.NONE)
			Debug.Log("type is NONE");

		return _trapList[(int)type];
	}
}
