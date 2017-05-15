using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AISoundManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _aiSound;

    private List<GameObject> _aiSounds = new List<GameObject>();
    public List<GameObject> AISounds { get { return _aiSounds; } set { _aiSounds = value; } }

    void Start()
    {

    }

    /// <summary>
    /// 指定した秒数の間音を発生させる
    /// </summary>
    public AISound MakeSound(Vector3 pos, float range, int effect_time)
    {
        var sound = Instantiate(_aiSound, transform);
        var ai_sound = sound.GetComponent<AISound>();
        ai_sound.MakeSound(pos, range, effect_time);
        _aiSounds.Add(sound);
        return ai_sound;
    }

    /// <summary>
    /// 指定したGameObjectが消えるまで音を発生させ続ける
    /// </summary>
    public AISound MakeSound(GameObject obj, float range)
    {
        var sound = Instantiate(_aiSound, transform);
        var ai_sound = sound.GetComponent<AISound>();
        ai_sound.MakeSound(obj, range);
        _aiSounds.Add(sound);
        return ai_sound;
    }

    void Update()
    {

    }
}
