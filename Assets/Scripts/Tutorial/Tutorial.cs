using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Tutorial : MonoBehaviour
{

    [SerializeField]
    private string _skipButton = "Start";
    [SerializeField]
    private string _fastForwardButton = "Action";

    private GameObject _tutorialCanvas;

    [SerializeField]
    private GameObject _textCanvas;

    private int _storyNum;
    public int StoryNum { get { return _storyNum; } set { _storyNum = value; } }

    private int _storyMax;

    [SerializeField]
    private float _nextStoryMoveTime = 2.0f;
    [SerializeField]
    private float _nextTextMoveTime = 1.0f;

    [SerializeField]
    private int[] _sceneChangeNums;

    [SerializeField]
    private Font _font;
    [SerializeField]
    private Font _startFont;

    SceneChanger _sceneChanger;

    void Start()
    {
        _sceneChanger = GameObject.Find("SceneChanger").GetComponent<SceneChanger>();
        SoundManager.Instance.PlayBGM("title");

        _tutorialCanvas = gameObject;
        _storyMax = _tutorialCanvas.transform.childCount - 1;

        StorySetup();
        StartCoroutine(Story());
    }

    private void StorySetup()
    {
        // 紙芝居の初期化
        for (int i = 0; i < _tutorialCanvas.transform.childCount; i++)
        {
            var show = _tutorialCanvas.transform.GetChild(i).gameObject;
            show.SetActive(false);
        }

        // 文字の初期化
        for (int i = 0; i < _textCanvas.transform.childCount; i++)
        {
            var texts = _textCanvas.transform.GetChild(i).gameObject;
            texts.SetActive(false);
            for (int k = 0; k < texts.transform.childCount; k++)
            {
                var c_text = texts.transform.GetChild(k).gameObject;
                c_text.SetActive(false);
                var text = c_text.transform.GetChild(0).GetComponent<Text>();
                text.font = _font;
                if (i == _textCanvas.transform.childCount - 1)
                    if (k == texts.transform.childCount - 1)
                        text.font = _startFont;
            }
        }
    }

    private IEnumerator Story()
    {
        int story_scene_num = 0;
        int story_count = 0;
        while (_storyNum <= _storyMax)
        {
            // 紙芝居の管理
            var show = _tutorialCanvas.transform.GetChild(_storyNum).gameObject;
            show.SetActive(true);

            var image = show.GetComponent<Image>();
            if (_storyNum > 0)
            {
                image.color = new Color(1, 1, 1, 0);
                StartCoroutine(DisplayGradually(image));
            }
            _storyNum++;

            var next_story_time = 0.0f;
            while (next_story_time < _nextStoryMoveTime)
            {
                if (Input.GetButtonDown(_fastForwardButton))
                    break;
                next_story_time += Time.deltaTime;
                yield return null;
            }

            // ここは文字が発生しないので、とばす
            if (_storyNum == 18 || _storyNum == 19 || _storyNum == 21 || _storyNum == 22)
            {
                //_textCanvas.transform.GetChild(story_scene_num).gameObject.SetActive(false);
                var t_list = _textCanvas.transform.GetChild(story_scene_num).gameObject;
                if (t_list.transform.childCount <= story_count)
                {
                    story_count = 0;
                    story_scene_num++;

                    // カラーのイージングをさせる（消すとき）
                    for (int i = 0; i < t_list.transform.childCount; i++)
                    {
                        var t = t_list.transform.GetChild(i).gameObject;

                        StartCoroutine(DisplayGradually(
                            t.GetComponent<Image>(),
                            2, 0, -1, () => t.transform.parent.gameObject.SetActive(false)));

                        var image_text = t.transform.GetChild(0).gameObject;

                        StartCoroutine(DisplayGradually(
                            image_text.GetComponent<Text>(), 2, 0, -1));
                    }
                    continue;
                }
                continue;
            }

            // 一番最初はテキストを表示しない
            if (_storyNum <= 1)
                continue;

            // 紙芝居につく文字の管理
            var text_list = _textCanvas.transform.GetChild(story_scene_num).gameObject;
            for (int i = 0; i < _textCanvas.transform.childCount; i++)
            {
                text_list = _textCanvas.transform.GetChild(i).gameObject;
                text_list.SetActive(false);
                if (story_scene_num == i)
                    text_list.SetActive(true);
            }

            // シーンが切り替わる時に文字を消すため、今表示している文字を全て消す
            text_list = _textCanvas.transform.GetChild(story_scene_num).gameObject;
            if (text_list.transform.childCount <= story_count)
            {
                story_count = 0;
                story_scene_num++;

                // カラーのイージングをさせる（消すとき）
                for (int i = 0; i < text_list.transform.childCount; i++)
                {
                    var t = text_list.transform.GetChild(i).gameObject;

                    StartCoroutine(DisplayGradually(
                        t.GetComponent<Image>(),
                        2, 0, -1, () => t.transform.parent.gameObject.SetActive(false)));

                    var image_text = t.transform.GetChild(0).gameObject;

                    StartCoroutine(DisplayGradually(
                        image_text.GetComponent<Text>(), 2, 0, -1));
                }
                continue;
            }

            var text = text_list.transform.GetChild(story_count).gameObject;
            text.SetActive(true);

            // カラーのイージングをさせる（表示するとき）
            var text_image = text.GetComponent<Image>();
            text_image.color = new Color(1, 1, 1, 0);
            StartCoroutine(DisplayGradually(text_image, 2, 0.75f));

            var tc = text_image.transform.GetChild(0).gameObject.GetComponent<Text>();
            tc.color = new Color(tc.color.r, tc.color.g, tc.color.b, 0);
            StartCoroutine(DisplayGradually(tc, 1, 0.75f));

            // 場面が変わる判定
            story_count++;

            //next_story_time = 0.0f;
            //while (next_story_time < _nextTextMoveTime)
            //{
            //    if (Input.GetButtonDown(_fastForwardButton))
            //        break;
            //    next_story_time += Time.deltaTime;
            //    yield return null;
            //}
            yield return new WaitForSeconds(_nextTextMoveTime);
        }

        while (true)
        {
            if (Input.GetButtonDown(_fastForwardButton))
            {
                ChangeGamemain();
            }
            yield return null;
        }
    }

    private IEnumerator DisplayGradually(Graphic image, float speed = 1,
                                         float change_alpha = 1.0f, int direction = 1,
                                         Action action = null)
    {
        float alpha = image.color.a;
        Color color = image.color;
        Func<bool> check = null;
        if (direction > 0)
            check = () => image.color.a <= change_alpha;
        else if (direction < 0)
            check = () => image.color.a >= change_alpha;

        while (check())
        {
            alpha += Time.deltaTime * speed * direction;
            alpha = Mathf.Clamp(alpha, 0, 1);
            image.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }
        if (action != null)
            action();
    }

    void Update()
    {
        if (Input.GetButtonDown(_skipButton) || Input.GetKeyDown(KeyCode.Return))
        {
            ChangeGamemain();
        }
    }

    void ChangeGamemain()
    {
        _sceneChanger.SceneChange("GameMain");
        SoundManager.Instance.StopBGM();
    }
}
