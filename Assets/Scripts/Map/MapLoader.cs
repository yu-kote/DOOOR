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
    [SerializeField]
    private string _mapDirectoryPath = "PlannerData/";

    public List<string[]> _mapDatas = new List<string[]>();
    public List<string[]> _trapDatas = new List<string[]>();
    public List<string[]> _itemDatas = new List<string[]>();

    void Awake()
    {
        LoadMap();
    }

    void LoadMap()
    {
        TextAsset csvFile = Resources.Load(_mapDirectoryPath + "Map") as TextAsset;
        StringReader reader = new StringReader(csvFile.text);
        while (reader.Peek() > -1)
        {
            string line = reader.ReadLine();
            _mapDatas.Add(line.Split(','));
        }

        csvFile = Resources.Load(_mapDirectoryPath + "TrapStatus") as TextAsset;
        reader = new StringReader(csvFile.text);
        while (reader.Peek() > -1)
        {
            string line = reader.ReadLine();
            _trapDatas.Add(line.Split(','));
        }

        csvFile = Resources.Load(_mapDirectoryPath + "ItemStatus") as TextAsset;
        reader = new StringReader(csvFile.text);
        while (reader.Peek() > -1)
        {
            string line = reader.ReadLine();
            _itemDatas.Add(line.Split(','));
        }
    }
}