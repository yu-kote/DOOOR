﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapSpawnManager : MonoBehaviour
{
    private TrapList _trapList = null;

    [SerializeField]
    public GameObject _camera;

    void Start()
    {
        _trapList = GameObject.Find("TrapList").GetComponent<TrapList>();
        if (_trapList == null)
            Debug.Log("_trapList is null");
    }

    public void SpawnTrap(TrapType type, Transform nodeTrans, Vector3 angle)
    {
        //TrapTypeがNONEだった場合はじく
        if (type == TrapType.NONE)
            return;

        GameObject _trapObject = Instantiate(_trapList.GetTrapObject(type));
        _trapObject.transform.position = nodeTrans.position + new Vector3(0, 0.5f, 0);
        _trapObject.transform.eulerAngles = angle;

        switch (type)
        {
            case TrapType.PITFALLS:
                _trapObject.GetComponent<PitFall>()._footPrint
                    = nodeTrans.GetComponent<FootPrint>();
                _trapObject.GetComponent<PitFall>().NodeCell
                    = new Vector2(nodeTrans.GetComponent<Node>().CellX,
                    nodeTrans.GetComponent<Node>().CellY);

                SoundManager.Instance.PlaySE("wana2", _trapObject);
                break;
            case TrapType.ROPE:
                _trapObject.GetComponent<Rope>()._footPrint
                    = nodeTrans.GetComponent<FootPrint>();

                SoundManager.Instance.PlaySE("wana", _trapObject);
                break;

            case TrapType.SOUND:
                break;

            case TrapType.NONE:
                break;
        }
    }
}
