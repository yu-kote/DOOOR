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
    DEGUTI,
    DEADSPACE,
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
    public Vector2 itaPos = new Vector2(int.MaxValue, int.MaxValue);
    public Vector2 manhaPos = new Vector2(int.MaxValue, int.MaxValue);

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
        _mapDatas.Clear();

        string path = _mapDirectoryPath + "/Stage" + stageNum.ToString() + "/";
        TextAsset csvFile = Resources.Load(path + "Map") as TextAsset;
        StringReader reader = new StringReader(csvFile.text);
        while (reader.Peek() > -1)
        {
            string line = reader.ReadLine();
            _mapDatas.Add(line.Split(','));
        }

        csvFile = Resources.Load(path + "Item") as TextAsset;
        reader = new StringReader(csvFile.text);
        List<string[]> data = new List<string[]>();
        while (reader.Peek() > -1)
        {
            string line = reader.ReadLine();
            data.Add(line.Split(','));
        }

        int x = int.Parse(data[0][0]);
        int y = int.Parse(data[0][1]);
        lastKeyPos = new Vector2(x, y);

        if (bool.Parse(data[1][0]) == true)
        {
            x = int.Parse(data[1][1]);
            y = int.Parse(data[1][2]);
            itaPos = new Vector2(x, y);
        }

        if (bool.Parse(data[2][0]) == true)
        {
            x = int.Parse(data[2][1]);
            y = int.Parse(data[2][2]);
            manhaPos = new Vector2(x, y);
        }
    }
}