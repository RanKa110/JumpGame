using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    static CharacterManager _instance;

    public static CharacterManager Instance
    //    싱글톤 선언
    {
        get
        {
            if( _instance == null)
            {
                _instance = new GameObject("CharacterManager")
                    .AddComponent<CharacterManager>();
            }
            return _instance;
        }
    }
    public Player _player;


    public Player Player
    //    플레이어 프로퍼티
    {
        get { return _player; }
        set { _player = value; }
    }

    private void Awake()
    {
        if(_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            if(_instance == this)
            {
                Destroy(gameObject);
            }
        }
    }



}

