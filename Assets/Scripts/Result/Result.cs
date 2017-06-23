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

        HumanBoardInstance();
    }

    void GameOver()
    {
        _gameOver.SetActive(true);
        _imageText = _gameOver.transform.GetChild(0).GetComponent<Image>();
        SoundManager.Instance.PlayBGM("gameover");
    }

    void HumanBoardInstance()
    {
        if (ShareData.Instance.WomanCount >= 1)
            _humans.Add("Woman");
        if (ShareData.Instance.TallManCount >= 1)
            _humans.Add("TallMan");
        if (ShareData.Instance.FatCount >= 1)
            _humans.Add("Fat");

        var woman_sprite = Resources.Load<Sprite>("Texture/GameMainUI/HumanListUI/woman_bustup");
        var tallman_sprite = Resources.Load<Sprite>("Texture/GameMainUI/HumanListUI/noppo_bustup");
        var fat_sprite = Resources.Load<Sprite>("Texture/GameMainUI/HumanListUI/matyo_bustup");
        var board = Resources.Load<GameObject>("Prefabs/Result/HumanBoard");

        for (int i = 0; i < _humans.Count; i++)
        {
            if (_humans[i] == "Woman")
            {
                var item = Instantiate(board, transform);
                item.transform.GetChild(0).GetComponent<Image>().sprite = woman_sprite;
                _boards.Add(item);
            }
            if (_humans[i] == "TallMan")
            {
                var item = Instantiate(board, transform);
                item.transform.GetChild(0).GetComponent<Image>().sprite = tallman_sprite;
                _boards.Add(item);
            }
            if (_humans[i] == "Fat")
            {
                var item = Instantiate(board, transform);
                item.transform.GetChild(0).GetComponent<Image>().sprite = fat_sprite;
                _boards.Add(item);
            }
        }

        foreach (var item in _boards)
        {
            var t = item.transform;
            t.localPosition = Vector3.zero;
            t.localScale = Vector3.one;
        }

        if (_boards.Count == 2)
        {
            _boards[0].transform.localPosition = new Vector3(130, 0, 0);
            _boards[1].transform.localPosition = new Vector3(-130, 0, 0);
        }
        if (_boards.Count == 3)
        {
            _boards[0].transform.localPosition = new Vector3(-200, 0, 0);
            _boards[1].transform.localPosition = new Vector3(0, 0, 0);
            _boards[2].transform.localPosition = new Vector3(200, 0, 0);
        }
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
