using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using System.Linq;
using UniRx;
using System;
using UnityEngine.UI;

public class ReportBoard : MonoBehaviour
{
    private static ReportBoard _instance;
    public static ReportBoard Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.Find("UICanvas").GetComponent<ReportBoard>();
                if (_instance == null)
                    Assert.IsNull(_instance);

            }
            return _instance;
        }
    }

    [SerializeField]
    private GameObject _board;
    [SerializeField]
    private GameObject _importantBoard;

    private List<GameObject> _boards = new List<GameObject>();

    float _popTime = 0.7f;
    float _endTime = 4.0f;

    public void Pop(string text, bool is_important = false)
    {
        var instance_board = _board;
        if (is_important)
            instance_board = _importantBoard;

        var board = Instantiate(instance_board, transform);
        board.transform.localPosition = new Vector3(0, board.transform.position.y, 0);
        board.transform.localScale = Vector3.one;
        board.transform.localRotation = Quaternion.identity;

        var board_text = board.transform.FindChild("Text").GetComponent<Text>();
        board_text.text = text;

        _boards.Add(board);

        StartCoroutine(PopStack());
    }

    private IEnumerator PopStack()
    {
        while (_boards.FirstOrDefault() != null)
        {
            while (true)
            {
                if (EasingInitiator.IsEaseEnd(_boards.FirstOrDefault()))
                    break;
                yield return null;
            }
            if (_boards.FirstOrDefault() == null)
                yield break;
            EasingInitiator.Add(_boards.FirstOrDefault(), new Vector3(0, 450, 0), _popTime, EaseType.CubicIn);
            EasingInitiator.Wait(_boards.FirstOrDefault(), _endTime);
            EasingInitiator.Add(_boards.FirstOrDefault(), _boards.FirstOrDefault().transform.localPosition, _popTime, EaseType.CubicOut);
        }
    }

    void Update()
    {
        var remove_list = _boards.FirstOrDefault();

        if (remove_list != null)
            if (EasingInitiator.IsEaseEnd(_boards.FirstOrDefault()))
            {
                Destroy(remove_list);
                _boards.Remove(remove_list);
            }

    }
}
