using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public enum MapID
{
	FLOOR,
	LEFTSTAIRS,
	RIGHTSTAIRS,
	WALL,
	LEFTDOOR,
	RIGHTDOOR,
	START,
	KYUKEISPACE,
	DUMMYWALL,
}

public class MapLoader : MonoBehaviour
{
	private string _mapDirectoryPath = "PlannerData/MapData";

	private int _stageNum = 1;
	public int StageNum
	{
		get { return _stageNum; }
		set { _stageNum = value; }
	}

	public List<string[]> _mapDatas = new List<string[]>();
	public List<string[]> _trapDatas = new List<string[]>();
	public List<string[]> _itemDatas = new List<string[]>();

	void Awake()
	{
		LoadMap(_stageNum);
	}

	public int GetStageNum()
	{
		string path = Application.dataPath + "/Resources/" + _mapDirectoryPath;
		Debug.Log(path);
		string[] directoryNames = System.IO.Directory.GetDirectories(path, "*", System.IO.SearchOption.AllDirectories);
		int stageNum = directoryNames.Length;

		Debug.Log(stageNum);

		return stageNum;
	}

	public void LoadMap(int stageNum)
	{
		string path = _mapDirectoryPath + "/Stage" + stageNum.ToString() + "/";
		TextAsset csvFile = Resources.Load(path + "Map") as TextAsset;
		StringReader reader = new StringReader(csvFile.text);
		while (reader.Peek() > -1)
		{
			string line = reader.ReadLine();
			_mapDatas.Add(line.Split(','));
		}

		csvFile = Resources.Load(path + "TrapStatus") as TextAsset;
		reader = new StringReader(csvFile.text);
		while (reader.Peek() > -1)
		{
			string line = reader.ReadLine();
			_trapDatas.Add(line.Split(','));
		}

		csvFile = Resources.Load(path + "ItemStatus") as TextAsset;
		reader = new StringReader(csvFile.text);
		while (reader.Peek() > -1)
		{
			string line = reader.ReadLine();
			_itemDatas.Add(line.Split(','));
		}
	}
}