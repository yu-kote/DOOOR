using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

public class MenuBarManager : MonoBehaviour
{
    [SerializeField]
    private string _actionButton = "Action";
    [SerializeField]
    string _verticallAxis = "Vertical";
    [SerializeField]
    string _crossVerticalAxis = "CrossVertical";

    [SerializeField]
    private GameObject[] _bar;

    private Action[] _barActions = null;

    private bool _isBarAction;
    private int _selectDirection;
    private bool _isAxisDown;
    private int _selectNum;
    private int _currentselectnum;

    private bool _isFirstSelectEnd;

    private void Awake()
    {
        _barActions = new Action[_bar.Count()];
        _selectNum = 0;
        _currentselectnum = -1;
        _isBarAction = false;
        _isFirstSelectEnd = false;
    }

    void Start()
    {

    }

    /// <summary>
    /// 選んだ時に実行される関数を登録する
    /// </summary>
    public void SetBarAction(int num, Action action)
    {
        if (_barActions.Count() < 0)
            return;
        if (num < 0 || num > _barActions.Count() - 1)
            return;
        _barActions[num] = action;
    }

    void Update()
    {
        BarActionUpdate();
        SelectBarStaging();
        SelectNumUpdate();
    }

    private void BarActionUpdate()
    {
        if (Input.GetButtonDown(_actionButton) == false)
            return;
        _barActions[_selectNum]();
        _isBarAction = true;
        SoundManager.Instance.PlaySE("kettei");
        EasingInitiator.Add(_bar[_selectNum].transform.FindChild("Button").gameObject,
                            Vector3.one * 1.3f, 0.2f, EaseType.BackOut, EaseValue.SCALE);
        EasingInitiator.Add(_bar[_selectNum].transform.FindChild("Button").gameObject,
                            Vector3.one, 0.2f, EaseType.BackOut, EaseValue.SCALE);
    }

    // メニューの何かしらが実行されたらtrue
    public bool IsBarUpdate()
    {
        if (_isBarAction)
        {
            _isBarAction = false;
            return true;
        }
        return false;
    }

    private void SelectBarStaging()
    {
        if (_currentselectnum == _selectNum)
            return;
        _currentselectnum = _selectNum;

        if (_isFirstSelectEnd)
            SoundManager.Instance.PlaySE("sentakuon");
        if (_isFirstSelectEnd == false)
            _isFirstSelectEnd = true;



        var before_scale = new Vector3(1f, 1f, 1f);
        var after_scale = new Vector3(1.3f, 1.3f, 1f);

        for (int i = 0; i < _bar.Count(); i++)
        {
            EasingInitiator.DestoryEase(_bar[i]);
        }
        for (int i = 0; i < _bar.Count(); i++)
        {
            if (_selectNum == i)
            {
                _bar[i].transform.FindChild("Button").gameObject.SetActive(true);
                EasingInitiator.Add(_bar[i], after_scale, 0.2f,
                                    EaseType.QuartOut, EaseValue.SCALE);
            }
            else
            {
                _bar[i].transform.FindChild("Button").gameObject.SetActive(false);
                EasingInitiator.Add(_bar[i], before_scale, 0.2f,
                                    EaseType.QuartOut, EaseValue.SCALE);
            }
        }
    }

    private void SelectNumUpdate()
    {
        //下に移動を移す場合は++する

        // スティック
        float vertical = Input.GetAxis(_verticallAxis);
        if (vertical > 0.5f)
        {
            if (_selectDirection == -1 || _selectDirection == 0)
            {
                _selectDirection = 1;
                _selectNum++;
            }
        }
        else if (vertical < -0.5f)
        {
            if (_selectDirection == 1 || _selectDirection == 0)
            {
                _selectDirection = -1;
                _selectNum--;
            }
        }
        if (vertical < 0.1f && vertical > -0.1f)
            _selectDirection = 0;

        // キーボード
        if (Input.GetKeyDown(KeyCode.UpArrow))
            _selectNum--;
        if (Input.GetKeyDown(KeyCode.DownArrow))
            _selectNum++;

        // 十字キー
        if (_isAxisDown == false)
        {
            if (Input.GetAxis(_crossVerticalAxis) == 1.0f)
            {
                _isAxisDown = true;
                _selectNum--;
            }
            else if (Input.GetAxis(_crossVerticalAxis) == -1.0f)
            {
                _isAxisDown = true;
                _selectNum++;
            }
        }
        if (Input.GetAxis(_crossVerticalAxis) == 0.0f)
            _isAxisDown = false;

        _selectNum = (int)Mathf.Repeat(_selectNum, _bar.Count());
    }
}
