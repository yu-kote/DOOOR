using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UniRx;
using System;

public class GameManager : MonoBehaviour
{
    private AIGenerator _aiGenerator;

    [SerializeField]
    private GameObject _gameClear;
    [SerializeField]
    private GameObject _gameOver;


    void Start()
    {
        //var field = GameObject.Find("Field");
        var human_manager = GameObject.Find("HumanManager");
        _aiGenerator = human_manager.GetComponent<AIGenerator>();
    }

    void Update()
    {
        GameClear();
        GameOver();
    }

    void GameClear()
    {
        var humans = _aiGenerator.Humans;
        if (humans.Count == 0) return;
        if (humans.FirstOrDefault(human => human.tag == "Victim") != null)
            return;
        _gameClear.SetActive(true);
    }

    void GameOver()
    {
        var humans = _aiGenerator.Humans;
        var goal_human = humans
            .Where(human => human.tag == "Victim")
            .FirstOrDefault(human =>
        {
            var item = human.GetComponent<AIItemController>().HaveItemCheck(ItemType.LASTKEY);
            if (item == false)
                return false;
            var current_node = human.GetComponent<AIController>().CurrentNode;
            if (current_node == null)
                return false;
            if (current_node.GetComponent<Deguti>())
            {
                human.GetComponent<AIItemController>().UseItem(ItemType.LASTKEY);

                var ai_controller = human.GetComponent<AIController>();
                var animation = human.GetComponent<VictimAnimation>();
                animation.AnimStatus = VictimAnimationStatus.OPEN_DOOR;
                ai_controller.StopMovement(0.5f, () => animation.AnimStatus = VictimAnimationStatus.IDOL);
                return true;
            }
            return false;
        });

        if (goal_human == null)
            return;

        _gameOver.SetActive(true);
    }

}
