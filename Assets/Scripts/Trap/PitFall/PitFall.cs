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
	private NodeManager _nodeManager = null;
	private Vector2 _nodeCell = Vector2.zero;
	public Vector2 NodeCell
	{
		get { return _nodeCell; }
		set { _nodeCell = value; }
	}

	private bool _isUsed = false;
	public bool IsUsed
	{
		get { return _isUsed; }
		set { _isUsed = value; }
	}

	void Start()
	{
		if (_footPrint == null)
			Debug.Log("_footPrint is null");

		_nodeManager = GameObject.Find("Field").GetComponent<NodeManager>();
		if(_nodeManager == null)
			Debug.Log("_nodeManager is null");
	}

	void Update()
	{
		//発動済みだったらはじく
		if (_isUsed)
			return;
		//ノードに人が一人もいなければはじく
		if (_footPrint.HumansOnNode.Count == 0)
			return;
		//一階だったらはじく
		if (_nodeCell.y == 0)
			return;

		//ここに落とし穴のアニメーション開始処理を記述する
		//未実装

		List<List<GameObject>> nodes = _nodeManager.Nodes;
		GameObject underNode = nodes[(int)_nodeCell.y - 1][(int)_nodeCell.x];
		//下に壁があれば落ちない
		if (underNode.GetComponent<Wall>() != null)
			return;

		Debug.Log("・・・・・・・・・・・・・・・");
		Debug.Log("落とし穴　作動");
		Debug.Log("・・・・・・・・・・・・・・・");

		_isUsed = true;
	}
}
