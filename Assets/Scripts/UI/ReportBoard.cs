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

    private List<GameObject> _boards = new List<GameObject>();

    public void Pop(string text)
    {
        var board = Instantiate(_board, transform);
        board.transform.localPosition = new Vector3(0, board.transform.position.y, 0);
        board.transform.localScale = Vector3.one;
        board.transform.localRotation = Quaternion.identity;

        var board_text = board.transform.FindChild("Text").GetComponent<Text>();
        board_text.text = text;

        _boards.Add(board);

        EasingInitiator.Add(board, new Vector3(0, 400, 0), 1, EaseType.CubicIn);
        EasingInitiator.Wait(board, 3);
        EasingInitiator.Add(board, board.transform.localPosition, 1, EaseType.CubicOut);
    }

    void Update()
    {
        var remove_list = _boards.Where(b => EasingInitiator.IsEaseEnd(b)).ToList();

        if (remove_list != null)
            foreach (var item in remove_list)
            {
                Destroy(item);
                _boards.Remove(item);
            }
    }
}
