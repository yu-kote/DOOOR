using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyNumber : MonoBehaviour
{
    private int _number = int.MaxValue;
    public int Number
    {
        get
        {
            return _number;
        }
        set
        {
            if (_number == int.MaxValue)
                _number = value;
        }
    }
}
