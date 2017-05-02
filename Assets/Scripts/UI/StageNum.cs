using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageNum : MonoBehaviour
{
	private MapChanger _mapChanger = null;
	private TextSetter _textSetter = null;

	void Start()
	{
		GameObject changeMap = GameObject.Find("ChangeMap");
		_mapChanger = changeMap.GetComponent<MapChanger>();
		_textSetter = GetComponent<TextSetter>();
	}

	void Update()
	{
		_textSetter.SetText(_mapChanger.StageNum.ToString());
	}
}
