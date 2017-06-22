using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UniRx;
using System;

public enum TrapDirection
{
    NONE,
    UP,
    DOWN,
    RIGHT,
    LEFT,
}

public class TrapSelectUI : MonoBehaviour
{
    const int trap_max = 4;
    [SerializeField]
    string _horizontalAxis = "CrossHorizontal";
    [SerializeField]
    string _verticalAxis = "CrossVertical";

    [SerializeField]
    Image[] _traps;

    TrapDirection _trapDirection;
    TrapDirection _currentDirection;

    Sprite[] _buttonSprites;

    [SerializeField]
    private float[] _recastTime;
    public class TrapUseStatus
    {
        public bool Enable = true;
        public float RecastTime = 0.0f;
        public float CurrentTime = 0.0f;
        public bool CanUse = true;
        public GameObject RecastBar;
        public float MinX, MaxX;
        public GameObject PushButtonEffect;
    }

    TrapUseStatus[] _trapUseStatus = new TrapUseStatus[trap_max];

    private TrapDirection _pushValue;
    public TrapDirection PushValue { get { return _pushValue; } }

    bool _isRecastStop;

    private void Start()
    {
        _trapDirection = TrapDirection.NONE;
        _buttonSprites = Resources.LoadAll<Sprite>("Texture/GameMainUI/TrapUI/itemcross");

        RecastBarSetup();
    }

    public void RecastBarSetup()
    {
        _isRecastStop = false;
        // リキャストとリキャストバーの初期化
        for (int i = 0; i < _trapUseStatus.Count(); i++)
        {
            _trapUseStatus[i] = new TrapUseStatus();
            _trapUseStatus[i].Enable = true;
            _trapUseStatus[i].RecastTime = _recastTime[i];
            _trapUseStatus[i].CurrentTime = 0.0f;
            _trapUseStatus[i].PushButtonEffect =
                _traps[i].gameObject.transform.FindChild("PushButtonEffect").gameObject;


            var bar = _trapUseStatus[i].RecastBar = _traps[i].gameObject.transform
                                          .FindChild("RecastBar").gameObject;
            var rect = bar.transform as RectTransform;
            var trap_transform = _traps[i].transform as RectTransform;

            // バーの一番左端
            _trapUseStatus[i].MinX =
                            Mathf.Abs(rect.offsetMax.x) + trap_transform.sizeDelta.x;
            // 右端
            _trapUseStatus[i].MaxX = rect.offsetMax.x;

            // 初期値はバーを出さないようにする
            var start_bar_right = Mathf.Abs(_trapUseStatus[i].MinX) + Mathf.Abs(_trapUseStatus[i].MaxX);
            rect.offsetMax = new Vector2(-start_bar_right + _trapUseStatus[i].MaxX, rect.offsetMax.y);

        }
    }

    void Update()
    {
        if (GameObject.Find("GameManager").GetComponent<GameManager>().CurrentGameState
            != GameState.GAMEMAIN)
        {
            _isRecastStop = true;
            return;
        }

        _isRecastStop = false;
        _trapDirection = GetTrapDirection();
        _pushValue = EnableCheck(_trapDirection);
        ButtonSpriteChange();
        RecastBarUpdate();
    }

    // リキャストを開始する
    public bool TrapRecast(TrapDirection dir)
    {
        var num = (int)dir - 1;
        if (_trapUseStatus[num].CanUse == false)
            return false;

        _trapUseStatus[num].CanUse = false;

        StartCoroutine(CallBack(_trapUseStatus[num].RecastTime,
                       () =>
                       {
                           _trapUseStatus[num].CanUse = true;
                           _trapUseStatus[num].CurrentTime = 0.0f;
                       }));
        return true;
    }

    // そのトラップが使用可能かどうか返す
    public bool CanUseTrap(TrapDirection dir)
    {
        return _trapUseStatus[(int)dir - 1].CanUse;
    }

    // リキャスト時間をもらう
    public float GetRecastTime(TrapDirection dir)
    {
        var num = (int)dir - 1;
        return _trapUseStatus[num].RecastTime;
    }

    /// <summary>
    /// 使用可能トラップを選ぶ
    /// </summary>
    public void SetEnableTrap(bool up = true, bool down = true,
                              bool right = true, bool left = true)
    {
        _trapUseStatus[0].Enable = up;
        _trapUseStatus[1].Enable = down;
        _trapUseStatus[2].Enable = right;
        _trapUseStatus[3].Enable = left;
        TrapImageSetup();
    }

    private void TrapImageSetup()
    {
        for (int i = 0; i < _trapUseStatus.Count(); i++)
            _traps[i].gameObject.SetActive(_trapUseStatus[i].Enable);
    }

    // 使用不可能か調べる
    private TrapDirection EnableCheck(TrapDirection dir)
    {
        int num = (int)dir - 1;
        if (num < 0 || num > _trapUseStatus.Count() - 1)
            return TrapDirection.NONE;
        if (_trapUseStatus[num].Enable)
            return dir;
        return TrapDirection.NONE;
    }

    // 指定秒数後に関数を呼ぶ
    private void Callback(float time, Action action)
    {
        Observable.Timer(TimeSpan.FromSeconds(time)).Subscribe(_ =>
        {
            action();
        }).AddTo(gameObject);
    }

    // 指定秒数後に関数を呼ぶ
    private IEnumerator CallBack(float time, Action action)
    {
        var count = 0.0f;
        while (count < time)
        {
            yield return null;
            if (_isRecastStop)
                continue;
            count += Time.deltaTime;
        }
        action();
    }

    // 十字キーの方向をTrapDirectionに変換する
    private TrapDirection GetTrapDirection()
    {
        if (Input.GetAxis(_verticalAxis) == 1.0f)
            return TrapDirection.UP;
        if (Input.GetAxis(_verticalAxis) == -1.0f)
            return TrapDirection.DOWN;
        if (Input.GetAxis(_horizontalAxis) == 1.0f)
            return TrapDirection.RIGHT;
        if (Input.GetAxis(_horizontalAxis) == -1.0f)
            return TrapDirection.LEFT;
        return TrapDirection.NONE;
    }

    // 十字キーの見た目の切り替え
    void ButtonSpriteChange()
    {
        if (_currentDirection == _trapDirection)
            return;

        gameObject.GetComponent<Image>().sprite = System.Array.Find<Sprite>(
                    _buttonSprites, (sprite) => sprite.name.Equals(
                        "itemcross_" + (int)_trapDirection));

        PushButtonEffect(_trapDirection);

        int num = (int)_trapDirection - 1;

        if (num == -1)
        {
            num = (int)_currentDirection - 1;
            _traps[num].color = new Color(255, 255, 255);
        }
        else if (num >= 0)
            _traps[num].color = new Color(100, 100, 100);

        _currentDirection = _trapDirection;
    }

    // ボタンを押したときのエフェクト？を出す
    void PushButtonEffect(TrapDirection dir)
    {
        if (dir == TrapDirection.NONE) return;
        int num = (int)dir - 1;
        _trapUseStatus[num].PushButtonEffect.SetActive(true);
        StartCoroutine(CallBack(0.1f,
            () => _trapUseStatus[num].PushButtonEffect.SetActive(false)));
    }

    // 全てのリキャストを増減させる
    void RecastBarUpdate()
    {
        for (int i = 0; i < _trapUseStatus.Count(); i++)
        {
            if (_trapUseStatus[i].Enable == false)
                continue;
            if (_trapUseStatus[i].CanUse)
                continue;

            _trapUseStatus[i].CurrentTime += Time.deltaTime;

            _trapUseStatus[i].CurrentTime = Mathf.Clamp(_trapUseStatus[i].CurrentTime, 0.0f, _trapUseStatus[i].RecastTime);

            var lerp = _trapUseStatus[i].CurrentTime / _trapUseStatus[i].RecastTime;
            var change_size = Mathf.Abs(_trapUseStatus[i].MinX) + Mathf.Abs(_trapUseStatus[i].MaxX);

            var current_right = change_size * lerp;
            var rect = _trapUseStatus[i].RecastBar.transform as RectTransform;

            rect.offsetMax = new Vector2(-current_right + _trapUseStatus[i].MaxX, rect.offsetMax.y);
        }
    }
}
