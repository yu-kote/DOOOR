using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Select : MonoBehaviour
{
    [SerializeField]
    private GameObject _selectCanvas;

    private MapLoader _mapLoader;
    private NodeManager _nodeManager;
    private MapBackgrounds _mapBackgrounds;
    private AIGenerator _aiGenerator;

    int _selectStageNum;
    int _currentSelectStageNum;

    void Start()
    {
        _selectStageNum = 0;
        _currentSelectStageNum = _selectStageNum;

        var field = GameObject.Find("Field");
        _nodeManager = field.GetComponent<NodeManager>();
        _mapLoader = field.GetComponent<MapLoader>();
        _mapBackgrounds = field.GetComponent<MapBackgrounds>();

        var human_manager = GameObject.Find("HumanManager");
        _aiGenerator = human_manager.GetComponent<AIGenerator>();
    }

    void Update()
    {
        if (GetComponent<GameManager>().CurrentGameState != GameState.SELECT)
        {
            _selectCanvas.SetActive(false);
            return;
        }
        _selectCanvas.SetActive(true);

        if (Input.GetKeyDown(KeyCode.RightArrow))
            _selectStageNum++;
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            _selectStageNum--;

        // 0はタイトルステージなので、1 ~ max 
        _selectStageNum = Mathf.Clamp(_selectStageNum, 1, _mapLoader.GetStageNum() - 1);

        if (_currentSelectStageNum == _selectStageNum)
            return;
        _currentSelectStageNum = _selectStageNum;

        ChangeMap();

        _aiGenerator.InstanceHumans(_selectStageNum);
    }

    public void SelectEnd()
    {
    }

    public void ChangeMap()
    {
        _mapLoader.LoadMap(_selectStageNum);
        _nodeManager.Start();
        _mapBackgrounds.CreateMapBackgrond();
    }
}
