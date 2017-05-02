using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanSetTrapOnNode : MonoBehaviour
{
	[SerializeField]
	private bool isDisplay = true;
	private Transform _parentTrans;
	private Transform _mainCameraTrans = null;
	private TrapStatus _trapStatus = null;

	void Start()
	{
		_parentTrans = transform.parent;
		_trapStatus = _parentTrans.parent.GetComponent<TrapStatus>();
		if (_trapStatus == null)
			Debug.Log("_trapStatus is null");

		_mainCameraTrans = GameObject.Find("MainCamera").transform;
		if (_mainCameraTrans == null)
			Debug.Log("_mainCameraTrans is null");

		uint trapStatus = _trapStatus.CanSetTrapStatus;

		if (trapStatus == 0)
		{
			Destroy(_parentTrans.gameObject);
			return;
		}

		TextMesh textMesh = GetComponent<TextMesh>();
		for (uint i = 1; i < (uint)TrapType.MAX; i += i)
		{
			if ((trapStatus & i) == 0)
				continue;

			switch ((TrapType)i)
			{
				case TrapType.PITFALLS:
					textMesh.text += "PITFALLS";
					break;
				case TrapType.DUMMY:
					textMesh.text += "DUMMY";
					break;
				case TrapType.SOUND:
					textMesh.text += "SOUND";
					break;
				case TrapType.ROPE:
					textMesh.text += "ROPE";
					break;
				case TrapType.CARPET:
					textMesh.text += "CARPET";
					break;
			}

			if (i != ((uint)TrapType.MAX >> 1))
				textMesh.text += "\n";
		}

		_parentTrans.gameObject.SetActive(isDisplay);
	}

	void Update()
	{
		if (!_trapStatus.IsSpawn)
			return;

		Destroy(_parentTrans.gameObject);
	}
}
