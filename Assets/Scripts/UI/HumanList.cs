using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

public enum BoardFaceType
{
    NORMAL,     // 通常
    HURRY,      // 焦り
    FEAR,       // 恐怖
}

public enum BoardIcon
{
    NONE,
    SURPRISE,
    ITEM_GET,
    LAST_KEY,
}

public class HumanBoard
{
    public GameObject Board;
    public GameObject DeadMark;
    public Image Face;
    public Sprite[] FaceSprites;
    public BoardFaceType FaceType = BoardFaceType.NORMAL;
    private BoardFaceType _currentFaceType = BoardFaceType.HURRY;

    public Image[] Items;
    public Image Icon;
    public float IconDrawTime;

    public void FaceTypeUpdate()
    {
        if (FaceType == _currentFaceType)
            return;
        _currentFaceType = FaceType;

        Face.sprite = System.Array.Find<Sprite>(
            FaceSprites, (sprite) => sprite.name.Equals(
                FaceSprites[(int)FaceType].name));
    }

    public bool SetIcon(Sprite sprite)
    {
        Icon.sprite = sprite;
        Icon.color = Color.white;
        IconDrawTimeStart();
        return true;
    }

    public bool EraseIcon()
    {
        Icon.sprite = null;
        Icon.color = new Color(1, 1, 1, 0);
        return true;
    }

    public void IconUpdate()
    {
        if (Icon.sprite == null)
            return;

        IconDrawTime -= Time.deltaTime;

        if (IconDrawTime <= 0)
        {
            IconDrawTimeStart();
            EraseIcon();
        }
    }

    private void IconDrawTimeStart()
    {
        IconDrawTime = 3.0f;
    }

    public bool SetItem(Sprite sprite)
    {
        var remnant_item = Items.FirstOrDefault(item => item.sprite == null);
        if (remnant_item == null)
            return false;

        remnant_item.sprite = sprite;
        remnant_item.color = Color.white;
        return true;
    }

    public bool UseItem(string sprite_name)
    {
        var buried_item = Items.Where(item => item.sprite != null).ToList();
        if (buried_item == null)
            return false;

        var use_item = buried_item.FirstOrDefault(item => item.sprite.name == sprite_name);
        if (use_item == null)
            return false;

        use_item.sprite = null;
        use_item.color = new Color(1, 1, 1, 0);

        return true;
    }
}

public class HumanList : MonoBehaviour
{
    [SerializeField]
    private GameObject _humanBoard;

    private AIGenerator _aiGenerator;

    // MyNumber.Number
    private Dictionary<int, HumanBoard> _humanBoardList = new Dictionary<int, HumanBoard>();

    private Sprite[] _womanSprites;
    private Sprite[] _tallmanSprites;
    private Sprite[] _fatSprites;
    Dictionary<string, Sprite> _boardSprites = new Dictionary<string, Sprite>();

    private void Start()
    {
        _womanSprites = Resources.LoadAll<Sprite>("Texture/GameMainUI/HumanListUI/woman_bustup");
        _tallmanSprites = Resources.LoadAll<Sprite>("Texture/GameMainUI/HumanListUI/noppo_bustup");
        _fatSprites = Resources.LoadAll<Sprite>("Texture/GameMainUI/HumanListUI/matyo_bustup");

        var sprites = Resources.LoadAll<Sprite>("Texture/GameMainUI/HumanListUI/");
        for (int i = 0; i < sprites.Count(); i++)
        {
            _boardSprites[sprites[i].name] = sprites[i];
        }

        _aiGenerator = GameObject.Find("HumanManager").GetComponent<AIGenerator>();
    }

    public void HumanBoardInstantiate(GameObject target)
    {
        if (target == null)
            return;

        if (target.tag == "Killer")
            return;

        var number = target.GetComponent<MyNumber>().Number;

        // 現在インスタンスされているボードの数
        var count = _humanBoardList.Count;

        var rect = _humanBoard.transform as RectTransform;

        // 左にずらす値
        var offset_x = -rect.sizeDelta.x * count;

        // 新しいボードを追加する
        _humanBoardList.Add(number, new HumanBoard());
        var human_board = _humanBoardList[number];
        human_board.Board = Instantiate(_humanBoard, transform);

        // 死んだ時用のバッテンマークを初期化
        var dead_mark = human_board.Board.transform.FindChild("DeadMark").gameObject;
        dead_mark.SetActive(false);
        human_board.DeadMark = dead_mark;

        // 表情を変える変数を初期化
        var face = human_board.Board.transform.FindChild("Human").gameObject;
        human_board.Face = face.GetComponent<Image>();

        if (target.GetComponent<MyNumber>().Name == "Woman")
            human_board.FaceSprites = _womanSprites;
        else if (target.GetComponent<MyNumber>().Name == "TallMan")
            human_board.FaceSprites = _tallmanSprites;
        else if (target.GetComponent<MyNumber>().Name == "Fat")
            human_board.FaceSprites = _fatSprites;

        // 表情をノーマルにするため一度だけ呼ぶ
        human_board.FaceTypeUpdate();

        // 右上のアイコンを初期化
        var icon = human_board.Board.transform.FindChild("Icon").gameObject;
        human_board.Icon = icon.GetComponent<Image>();

        // アイテムを表示するところを初期化
        human_board.Items = new Image[3];
        for (int i = 0; i < 3; i++)
        {
            var item = human_board.Board.transform.FindChild("Item" + i).gameObject;
            human_board.Items[i] = item.GetComponent<Image>();
            human_board.Items[i].color = new Color(1, 1, 1, 0.0f);
        }

        // ボードの位置を初期化する
        StartCoroutine(TransformSetup(offset_x, number));
    }

    private IEnumerator TransformSetup(float offset, int target_NUM)
    {
        yield return null;
        var pos = new Vector3(_humanBoard.transform.position.x + offset, 0, 0);
        _humanBoardList[target_NUM].Board.transform.localPosition = pos;

        _humanBoardList[target_NUM].Board.transform.localScale = Vector3.one;
        _humanBoardList[target_NUM].Board.transform.localRotation = Quaternion.identity;
    }

    void Update()
    {
        FaceUpdate();
        IconUpdate();
    }

    void FaceUpdate()
    {
        foreach (var board in _humanBoardList)
        {
            var target = _aiGenerator.GetHumanFromMyNumber(board.Key);
            if (target == null)
            {
                board.Value.DeadMark.SetActive(true);
                continue;
            }
            else
                board.Value.DeadMark.SetActive(false);

            var status = target.GetComponent<VictimAnimation>().AnimStatus;

            BoardFaceType face_type = board.Value.FaceType;

            if (status == VictimAnimationStatus.IDOL ||
                status == VictimAnimationStatus.WALK)
                face_type = BoardFaceType.NORMAL;
            if (status == VictimAnimationStatus.RUN ||
                status == VictimAnimationStatus.STAGGER)
                face_type = BoardFaceType.HURRY;
            if (status == VictimAnimationStatus.CRISIS ||
                status == VictimAnimationStatus.DEAD)
                face_type = BoardFaceType.FEAR;

            board.Value.FaceType = face_type;
            board.Value.FaceTypeUpdate();
        }
    }

    private void IconUpdate()
    {
        foreach (var board in _humanBoardList)
        {
            var target = _aiGenerator.GetHumanFromMyNumber(board.Key);
            if (target == null)
                continue;

            if (target.GetComponent<VictimAnimation>().AnimStatus
                == VictimAnimationStatus.STAGGER)
            {
                var s = GetSprite(GetIconNameFromBoardIcon(BoardIcon.SURPRISE));
                board.Value.SetIcon(s);
            }

            if (target.GetComponent<AIItemController>().HaveItemCheck(ItemType.LASTKEY))
            {
                var s = GetSprite(GetIconNameFromBoardIcon(BoardIcon.LAST_KEY));
                board.Value.SetIcon(s);
            }
            board.Value.IconUpdate();
        }
    }

    public void HumanItemSetup()
    {
        foreach (var board in _humanBoardList)
        {
            var target = _aiGenerator.GetHumanFromMyNumber(board.Key);
            if (target == null)
                continue;

            target.GetComponent<AIItemController>().SetHumanList(this);
        }
    }

    public string GetItemNameFromItemType(ItemType type)
    {
        switch (type)
        {
            case ItemType.NONE:
                break;
            case ItemType.KEY:
                break;
            case ItemType.LASTKEY:
                return "key";
            case ItemType.FLASHLIGHT:
                return "flushlight";
            case ItemType.GUN:
                return "hundgun";
            case ItemType.TYENSO:
                return "chainsaw";
        }
        return null;
    }

    public Sprite GetSprite(string name)
    {
        return _boardSprites[name];
    }

    public Sprite GetItemSprite(ItemType type)
    {
        return _boardSprites[GetItemNameFromItemType(type)];
    }

    public string GetIconNameFromBoardIcon(BoardIcon type)
    {
        switch (type)
        {
            case BoardIcon.NONE:
                break;
            case BoardIcon.SURPRISE:
                return "surprise_icon";
            case BoardIcon.ITEM_GET:
                return "itemget_icon";
            case BoardIcon.LAST_KEY:
                return "key_icon";
        }
        return null;
    }

    public HumanBoard GetHumanBoard(int number)
    {
        return _humanBoardList[number];
    }

    public void OnDestroy()
    {
        foreach (var board in _humanBoardList)
            Destroy(board.Value.Board);

        _humanBoardList.Clear();
    }
}
