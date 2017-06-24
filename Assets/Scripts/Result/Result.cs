using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;


public class Result : MonoBehaviour
{
    [SerializeField]
    private string _actionButton = "Action";
    [SerializeField]
    private GameObject _gameClear;
    [SerializeField]
    private GameObject _gameOver;

    [SerializeField]
    private MenuBarManager _menuBarManager;

    private Image _imageText;
    private List<string> _humans = new List<string>();
    private List<GameObject> _boards = new List<GameObject>();

    private void Awake()
    {
        _menuBarManager.gameObject.SetActive(true);
    }

    void Start()
    {
        _gameClear.SetActive(false);
        _gameOver.SetActive(false);

        if (ShareData.Instance.Status == ResultStatus.GAMECLEAR)
            GameClear();
        if (ShareData.Instance.Status == ResultStatus.GAMEOVER)
            GameOver();

        _imageText.color = new Color(1, 1, 1, 0);

        _menuBarManager.SetBarAction(0, () =>
        {
            GameObject.Find("SceneChanger").GetComponent<SceneChanger>()
                            .SceneChange("GameMain", () => SoundManager.Instance.StopBGM());
            SoundManager.Instance.volume.Bgm = 1;
        });
        _menuBarManager.SetBarAction(1, () =>
        {
            GameObject.Find("SceneChanger").GetComponent<SceneChanger>()
                            .SceneChange("Title", () => SoundManager.Instance.StopBGM());
            SoundManager.Instance.volume.Bgm = 1;
        });

        StartCoroutine(ChangeTitle(2.0f));
    }

    void GameClear()
    {
        _gameClear.SetActive(true);
        _imageText = _gameClear.transform.GetChild(0).GetComponent<Image>();

        var image = _gameClear.GetComponent<Image>();
        image.sprite = Resources.Load<Sprite>("Texture/Result/clear03");
        SoundManager.Instance.PlayBGM("gameclear");

        SoundManager.Instance.volume.Bgm = 3;

        HumanBoard();
    }

    void GameOver()
    {
        _gameOver.SetActive(true);
        _imageText = _gameOver.transform.GetChild(0).GetComponent<Image>();
        SoundManager.Instance.PlayBGM("gameover");
    }

    void HumanBoard()
    {
        if (ShareData.Instance.WomanCount >= 1)
            _humans.Add("Woman");
        if (ShareData.Instance.TallManCount >= 1)
            _humans.Add("TallMan");
        if (ShareData.Instance.FatCount >= 1)
            _humans.Add("Fat");
        StartCoroutine(HumanBoardInstance());
    }

    private IEnumerator HumanBoardInstance()
    {
        var woman_sprite = Resources.Load<Sprite>("Texture/GameMainUI/HumanListUI/woman_bustup");
        var tallman_sprite = Resources.Load<Sprite>("Texture/GameMainUI/HumanListUI/noppo_bustup");
        var fat_sprite = Resources.Load<Sprite>("Texture/GameMainUI/HumanListUI/matyo_bustup");
        var board = Resources.Load<GameObject>("Prefabs/Result/HumanBoard");

        var board_positions = new List<Vector3>();

        if (_humans.Count == 1)
        {
            board_positions.Add(new Vector3(0, 0, 0));
        }
        if (_humans.Count == 2)
        {
            board_positions.Add(new Vector3(130, 0, 0));
            board_positions.Add(new Vector3(-130, 0, 0));
        }
        if (_humans.Count == 3)
        {
            board_positions.Add(new Vector3(-200, 0, 0));
            board_positions.Add(new Vector3(0, 0, 0));
            board_positions.Add(new Vector3(200, 0, 0));
        }

        for (int i = 0; i < _humans.Count; i++)
        {
            var item = board;
            if (_humans[i] == "Woman")
            {
                item = Instantiate(board, transform);
                item.transform.GetChild(0).GetComponent<Image>().sprite = woman_sprite;
                _boards.Add(item);
            }
            if (_humans[i] == "TallMan")
            {
                item = Instantiate(board, transform);
                item.transform.GetChild(0).GetComponent<Image>().sprite = tallman_sprite;
                _boards.Add(item);
            }
            if (_humans[i] == "Fat")
            {
                item = Instantiate(board, transform);
                item.transform.GetChild(0).GetComponent<Image>().sprite = fat_sprite;
                _boards.Add(item);
            }

            item.transform.localPosition = board_positions[i];
            item.transform.localScale = Vector3.one;
            item.transform.FindChild("DeadMark").GetComponent<DeadMark>().Number = i;
        }

        yield return null;
    }

    private IEnumerator ChangeTitle(float time)
    {
        yield return new WaitForSeconds(time);
        while (true)
        {
            var color = _imageText.color;
            color.a += Time.deltaTime;
            _imageText.color = color;

            yield return null;
        }
    }


    private void Update()
    {
        if (ShareData.Instance.Status == ResultStatus.GAMECLEAR)
            SoundManager.Instance.volume.Bgm = 3;
        if (ShareData.Instance.Status == ResultStatus.GAMEOVER)
            SoundManager.Instance.volume.Bgm = 1;

    }

    private void OnDestroy()
    {
        foreach (var item in _boards)
            Destroy(item);
        _boards.Clear();
        _humans.Clear();
    }
}
