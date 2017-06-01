using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;
using System.Collections.Generic;

/// <summary>
/// 使い方
/// SoundManager.Instance.PlaySE("se_getItem.wav");
/// </summary>
public class SoundManager : MonoBehaviour
{
    private static SoundManager _instance;
    public static SoundManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = (SoundManager)FindObjectOfType(typeof(SoundManager));
                if (_instance == null)
                    Assert.IsNull(_instance);

            }
            return _instance;
        }
    }

    public SoundVolume volume = new SoundVolume();

    // AudioSource

    private AudioSource _bgmSource;

    // 16個のSEが同時に鳴らせる限界とします
    private AudioSource[] _seSource = new AudioSource[16];

    //private AudioSource[] _voiceSouce = new AudioSource[8];

    // AudioClip

    private Dictionary<string, int> _bgmList = new Dictionary<string, int>();
    private AudioClip[] _bgm;

    private Dictionary<string, int> _seList = new Dictionary<string, int>();
    private AudioClip[] _se;

    //private Dictionary<string, int> _voiceList = new Dictionary<string, int>();
    //private AudioClip[] _voice;


    private void Awake()
    {
        GameObject[] obj = GameObject.FindGameObjectsWithTag("SoundManager");
        if (obj.Length > 1)
            Destroy(gameObject);
        else
            DontDestroyOnLoad(gameObject);

        _bgmSource = gameObject.AddComponent<AudioSource>();
        _bgmSource.loop = true;

        for (int i = 0; i < _seSource.Length; i++)
            _seSource[i] = gameObject.AddComponent<AudioSource>();

        //for (int i = 0; i < _seSource.Length; i++)
        //_voiceSouce[i] = gameObject.AddComponent<AudioSource>();

        _bgm = Resources.LoadAll<AudioClip>("Audio/BGM");
        _se = Resources.LoadAll<AudioClip>("Audio/SE");

        for (int i = 0; i < _bgm.Length; i++)
            _bgmList[_bgm[i].name] = i;

        for (int i = 0; i < _se.Length; i++)
        {
            _seList[_se[i].name] = i;
        }
    }

    private void Update()
    {
        // ミュート
        _bgmSource.mute = volume.Mute;

        foreach (var s in _seSource)
            s.mute = volume.Mute;
        //foreach (var s in _voiceSouce)
        //s.mute = volume.Mute;

        // ボリューム
        _bgmSource.volume = volume.Bgm;

        foreach (var s in _seSource)
            s.volume = volume.Se;
        //foreach (var s in _voiceSouce)
        //s.volume = volume.Voice;
    }

    // BGM 再生
    public void PlayBGM(string name)
    {
        PlayBGM(_bgmList[name]);
    }

    // GameObjectを指定して BGM 再生
    public void PlayBGM(string name, GameObject target)
    {
        var s = target.AddComponent<AudioSource>();

        // 3D上でしか聞こえなくする
        s.spatialBlend = 1.0f;

        s.clip = _bgm[_bgmList[name]];
        s.Play();
        s.loop = true;

        // 決め打ちです
        s.maxDistance = 40.0f;                      // 音量変化の距離の最大値
        s.rolloffMode = AudioRolloffMode.Linear;    // 音量変化のタイプ
    }

    // BGM 再生 
    public void PlayBGM(int index)
    {
        if (0 > index || _bgm.Length <= index) return;
        if (_bgmSource.clip == _bgm[index]) return;

        _bgmSource.Stop();
        _bgmSource.clip = _bgm[index];
        _bgmSource.Play();
    }

    // BGM 停止
    public void StopBGM()
    {
        _bgmSource.Stop();
        _bgmSource.clip = null;
    }


    // SE 再生
    public void PlaySE(string name)
    {
        PlaySE(_seList[name]);
    }

    // GameObjectを指定して SE 再生
    public void PlaySE(string name, GameObject target)
    {
        var s = target.AddComponent<AudioSource>();

        // 3D上でしか聞こえなくする
        s.spread = 360.0f;

        s.clip = _se[_seList[name]];
        s.Play();

        // 3D上でしか聞こえなくする
        s.spatialBlend = 1.0f;

        // 決め打ちです
        s.maxDistance = 40.0f;                      // 音量変化の距離の最大値
        s.rolloffMode = AudioRolloffMode.Linear;    // 音量変化のタイプ
    }

    // SE 再生
    public void PlaySE(int index)
    {
        if (0 > index || _se.Length <= index) return;
        foreach (var s in _seSource)
        {
            if (s.isPlaying == false)
            {
                s.clip = _se[index];
                s.Play();
                return;
            }
        }
    }

    // SE 停止
    public void StopSE()
    {
        foreach (var s in _seSource)
        {
            s.Stop();
            s.clip = null;
        }
    }

    // 音声 再生
    //public void PlayVoice(int index)
    //{
    //    if (0 > index || _voice.Length <= index) return;
    //    foreach (var s in _voiceSouce)
    //    {
    //        if (s.isPlaying == false)
    //        {
    //            s.clip = _voice[index];
    //            s.Play();
    //            return;
    //        }
    //    }
    //}

    // 音声 停止
    //public void StopVoice()
    //{
    //    foreach (var s in _voiceSouce)
    //    {
    //        s.Stop();
    //        s.clip = null;
    //    }
    //}

}


public class SoundVolume
{
    public float Bgm = 1.0f;
    public float Voice = 1.0f;
    public float Se = 1.0f;
    public bool Mute = false;

    public void Init()
    {
        Bgm = 1.0f;
        Voice = 1.0f;
        Se = 1.0f;
        Mute = false;
    }
}
