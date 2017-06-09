using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    [SerializeField]
    string _horizontalAxis = "CrossHorizontal";
    [SerializeField]
    string _verticalAxis = "CrossVertical";

    [SerializeField]
    Image[] _traps;

    TrapDirection _trapDirection;
    TrapDirection _currentDirection;


    Sprite[] _buttonSprites;

    void Start()
    {
        _trapDirection = TrapDirection.NONE;

        _buttonSprites = Resources.LoadAll<Sprite>("Texture/GameMainUI/TrapUI/itemcross");
    }

    void Update()
    {
        _trapDirection = PushValue();
        ButtonSpriteChange();
    }

    public TrapDirection PushValue()
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

    void ButtonSpriteChange()
    {
        if (_currentDirection == _trapDirection)
            return;

        gameObject.GetComponent<Image>().sprite = System.Array.Find<Sprite>(
                    _buttonSprites, (sprite) => sprite.name.Equals(
                        "itemcross_" + (int)_trapDirection));
        

        int num = (int)_trapDirection - 1;
        if (num == -1)
        {
            num = (int)_currentDirection - 1;
            _traps[num].color = new Color(255, 255, 255);
        }
        else if (num >= 0)
        {
            _traps[num].color = new Color(0, 255, 255);
        }
        _currentDirection = _trapDirection;
    }
}
