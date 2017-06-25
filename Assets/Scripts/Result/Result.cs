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
    private Text _imageTextComment;
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

        StartCoroutine(ChangeTitle(2.5f));
    }

    void GameClear()
    {
        _gameClear.SetActive(true);
        _imageText = _gameClear.transform.GetChild(0).GetComponent<Image>();

        _imageTextComment = _imageText.transform.GetChild(0).GetComponent<Text>();
        _imageTextComment.color = new Color(1, 0, 0, 0);
        _imageTextComment.text = CommetRandom(ResultStatus.GAMECLEAR);

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

        _imageTextComment = _imageText.transform.GetChild(0).GetComponent<Text>();
        _imageTextComment.color = new Color(1, 1, 1, 0);
        _imageTextComment.text = CommetRandom(ResultStatus.GAMEOVER);

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
        _humans.Add("Fat");
        _humans.Add("Fat");
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

            var comment_color = _imageTextComment.color;
            comment_color.a += Time.deltaTime;
            _imageTextComment.color = comment_color;

            yield return null;
        }
    }

    private string CommetRandom(ResultStatus status)
    {
        var max_num = 8;
        var rand_num = UnityEngine.Random.Range(0, max_num);
        if (status == ResultStatus.GAMECLEAR)
        {
            if (rand_num == 0) return "これでまた平穏が訪れましたね";
            if (rand_num == 1) return "また犠牲者が増えてしまいました";
            if (rand_num == 2) return "あぁ、なんてむごい";
            if (rand_num == 3) return "屋敷内のお掃除完了です";
            if (rand_num == 4) return "隠れるためには必要な犠牲なのです";
            if (rand_num == 5) return "あぁ、もっと殺したい";
            if (rand_num == 6) return "この感触は慣れることはないですね";
            if (rand_num == 7) return "無事に時が過ぎ去ります";
        }
        if (status == ResultStatus.GAMEOVER)
        {
            if (rand_num == 0) return "彼らは今後楽しく生きるのでしょうか";
            if (rand_num == 1) return "此処の場所を言いふらさないといいのですが";
            if (rand_num == 2) return "われらも外、出たいですね";
            if (rand_num == 3) return "今日はピクニック日よりみたいです";
            if (rand_num == 4) return "おや、いつの間にか朝でしたね";
            if (rand_num == 5) return "新しい鍵を作らなくては";
            if (rand_num == 6) return "当分は怯えながら暮らすことになりそうです";
            if (rand_num == 7) return "逃げてしまいましたか";
        }
        return "";
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
