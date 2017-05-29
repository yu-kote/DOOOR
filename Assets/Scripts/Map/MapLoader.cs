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
	CANBREAKEWALL,
	DEGUTI
}

public class MapLoader : MonoBehaviour
{
	private string _mapDirectoryPath = "PlannerData/MapData";

	public int _stageNum = 1;
	public int StageNum
	{
		get { return _stageNum; }
		set { _stageNum = value; }
	}

	public List<string[]> _mapDatas = new List<string[]>();
	public Vector2 lastKeyPos = Vector2.zero;
	public string[] items;

	void Awake()
	{
		LoadMap(_stageNum);
	}

	public int GetStageNum()
	{
		string path = Application.dataPath + "/Resources/" + _mapDirectoryPath;
		string[] directoryNames = System.IO.Directory.GetDirectories(path, "*", System.IO.SearchOption.AllDirectories);
		int stageNum = directoryNames.Length;

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

		csvFile = Resources.Load(path + "LastKeyPlace") as TextAsset;
		reader = new StringReader(csvFile.text);
		while (reader.Peek() > -1)
		{
			string line = reader.ReadLine();
			string[] data = line.Split(',');
			int x = int.Parse(data[0]);
			int y = int.Parse(data[1]);
			lastKeyPos = new Vector2(x, y);
		}

		csvFile = Resources.Load(path + "ItemList") as TextAsset;
		reader = new StringReader(csvFile.text);
		while (reader.Peek() > -1)
		{
			string line = reader.ReadLine();
			items = line.Split(',');
		}
	}
}