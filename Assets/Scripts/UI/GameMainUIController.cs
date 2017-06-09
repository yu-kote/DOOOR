using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMainUIController : MonoBehaviour
{
    [SerializeField]
    GameObject[] _uiList;

    Dictionary<string, Vector3> _uiStartPositions = new Dictionary<string, Vector3>();
    Dictionary<string, Vector3> _uiEndPositions = new Dictionary<string, Vector3>();


    float _easeTime;

    void Start()
    {
        _easeTime = 1.0f;
        UiSetup();
    }

    void Update()
    {

    }

    private void UiSetup()
    {
        foreach (var ui in _uiList)
        {
            _uiStartPositions.Add(ui.name, ui.transform.localPosition);
            if (ui.name == "RightTurnBoard")
                RightTurnBoardSetup(ui);
            if (ui.name == "LeftTurnBoard")
                LeftTurnBoardSetup(ui);
            if (ui.name == "TrapCrossOperation")
                TrapCrossOperationSetup(ui);
            if (ui.name == "HumanList")
                HumanListSetup(ui);
        }
        foreach (var ui in _uiList)
        {
            _uiEndPositions.Add(ui.name, ui.transform.localPosition);
        }
    }

    public void UiStart()
    {
        foreach (var ui in _uiList)
        {
            EaseUi(ui, _uiStartPositions[ui.name]);
        }
    }

    public void UiFadeAway()
    {
        foreach (var ui in _uiList)
        {
            EaseUi(ui, _uiEndPositions[ui.name]);
        }
    }

    // UIを隠す処理

    void RightTurnBoardSetup(GameObject ui)
    {
        var rect = ui.transform as RectTransform;
        rect.localPosition += new Vector3(100, 0, 0);
    }

    void LeftTurnBoardSetup(GameObject ui)
    {
        var rect = ui.transform as RectTransform;
        rect.localPosition += new Vector3(-100, 0, 0);
    }

    void TrapCrossOperationSetup(GameObject ui)
    {
        var rect = ui.transform as RectTransform;
        rect.localPosition += new Vector3(0, -300, 0);
    }

    void HumanListSetup(GameObject ui)
    {
        var rect = ui.transform as RectTransform;
        rect.localPosition += new Vector3(0, -350, 0);
    }

    // UIを所定の位置までイージングさせる

    void EaseUi(GameObject ui, Vector3 target_pos)
    {
        EasingInitiator.Add(ui, target_pos,
                    _easeTime, EaseType.CircOut);
    }

    private void OnDestroy()
    {
        _uiStartPositions.Clear();
        _uiEndPositions.Clear();
    }
}
