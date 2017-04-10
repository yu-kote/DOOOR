using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PitFall : MonoBehaviour
{
	private FootPrint _footPrint = null;
	public FootPrint FootPrint
	{
		get { return _footPrint; }
		set { _footPrint = value; }
	}

	private bool _isUsed = false;
	public bool IsUsed
	{
		get { return _isUsed; }
		set { _isUsed = value; }
	}

	void Start()
	{
		if (_footPrint = null)
			Debug.Log("_footPrint is null");
	}

	void Update()
	{
		//発動済みだったらはじく
		if (_isUsed)
			return;
		//ノードに人が一人もいなければはじく
		if (_footPrint.HumansOnNode.Count == 0)
			return;

		//ここに落とし穴のアニメーション開始処理を記述する
		//未実装

		_isUsed = true;
	}
}
