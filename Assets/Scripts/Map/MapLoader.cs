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
	START
}

public class MapLoader : MonoBehaviour
{
	[SerializeField]
	private string _mapFilePath = "PlannerData/Map";

	public List<string[]> _mapDatas = new List<string[]>();

	void Awake()
	{
		LoadMap();
	}

	void LoadMap()
	{
		TextAsset csvFile = Resources.Load(_mapFilePath) as TextAsset;
		StringReader reader = new StringReader(csvFile.text);

		while (reader.Peek() > -1)
		{
			string line = reader.ReadLine();
			_mapDatas.Add(line.Split(','));
		}

		Debug.Log("sizeY : " + _mapDatas.Count);
		Debug.Log("sizeX : " + _mapDatas[0].Length);
	}
}