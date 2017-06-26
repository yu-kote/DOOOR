using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ResultStatus
{
    GAMECLEAR,
    GAMEOVER,
}

public class ShareData
{
    public readonly static ShareData Instance = new ShareData();
    public ResultStatus Status;

    public int WomanCount = 0;
    public int TallManCount = 0;
    public int FatCount = 0;

    public void Reset()
    {
        WomanCount = 0;
        TallManCount = 0;
        FatCount = 0;
    }

    public int SelectStage = 1;
    public int CanSelectStageMax = 0;
    public int StageMax = 8;

    public List<int> ClearStages = new List<int>();
}
