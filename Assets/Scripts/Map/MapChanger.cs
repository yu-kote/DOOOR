using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapChanger : MonoBehaviour
{
	private int _stageNum = 1;
	public int StageNum
	{
		get { return _stageNum; }
		set { _stageNum = value; }
	}
	private int _stageMaxNum = 1;

	private MapLoader _mapLoader = null;

	void Start()
	{
		_mapLoader = GameObject.Find("Field").GetComponent<MapLoader>();
		if (_mapLoader == null)
			Debug.Log("_mapLoader is null");

		_stageMaxNum = _mapLoader.GetStageNum();
	}

	public void IncrementStageNum()
	{
		_stageNum = Mathf.Min(_stageMaxNum, _stageNum + 1);
	}

	public void DecrementStageNum()
	{
		_stageNum = Mathf.Max(1	, _stageNum - 1);
	}

	public void ChangeMap()
	{
		_mapLoader.StageNum = _stageNum;

		SceneManager.LoadScene("GameMain");
	}
}
