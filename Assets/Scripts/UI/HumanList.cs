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

public class HumanBoard
{
    public GameObject Board;
    public GameObject DeadMark;
    public Image Face;
    public Sprite[] Sprites;
    public BoardFaceType FaceType = BoardFaceType.NORMAL;
    private BoardFaceType _currentFaceType = BoardFaceType.HURRY;

    public void FaceTypeUpdate()
    {
        //if (FaceType == _currentFaceType)
        //    return;
        //_currentFaceType = FaceType;

        Face.sprite = System.Array.Find<Sprite>(
            Sprites, (sprite) => sprite.name.Equals(
                Sprites[(int)FaceType].name));
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

    private void Start()
    {
        _womanSprites = Resources.LoadAll<Sprite>("Texture/GameMainUI/HumanListUI/woman_bustup");
        _tallmanSprites = Resources.LoadAll<Sprite>("Texture/GameMainUI/HumanListUI/noppo_bustup");
        _fatSprites = Resources.LoadAll<Sprite>("Texture/GameMainUI/HumanListUI/matyo_bustup");
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

        _humanBoardList.Add(number, new HumanBoard());
        var human_board = _humanBoardList[number];
        human_board.Board = Instantiate(_humanBoard, transform);

        var dead_mark = human_board.Board.transform.FindChild("DeadMark").gameObject;
        dead_mark.SetActive(false);
        human_board.DeadMark = dead_mark;

        var face = human_board.Board.transform.FindChild("Human").gameObject;
        human_board.Face = face.GetComponent<Image>();
        
        if (target.GetComponent<MyNumber>().Name == "Woman")
            human_board.Sprites = _womanSprites;
        else if (target.GetComponent<MyNumber>().Name == "TallMan")
            human_board.Sprites = _tallmanSprites;
        else if (target.GetComponent<MyNumber>().Name == "Fat")
            human_board.Sprites = _fatSprites;

        human_board.FaceTypeUpdate();

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
        foreach (var board in _humanBoardList)
        {
            var target = _aiGenerator.SurvivalCheckNumber(board.Key);
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

    public void OnDestroy()
    {
        foreach (var board in _humanBoardList)
            Destroy(board.Value.Board);

        _humanBoardList.Clear();
    }
}
