using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameSettingsSO", menuName = "ScriptableObjects/GameSettingsSO", order = 1)]
public class GameSettingsSO : ScriptableObject
{
    [SerializeField] int _health;
    [SerializeField] int _oilCans;
    [SerializeField] float _sanity;
    [SerializeField] int _currentDialogue;
    [SerializeField] int _houseIndex;

    public int health {
        get {
            return _health;
        }
        set {
            _health = value;
        }
    }

    public int oilCans {
        get {
            return _oilCans;
        }
        set {
            _oilCans = value;
        }
    }

    public float sanity {
        get {
            return _sanity;
        }
        set {
            _sanity = value;
        }
    }

    public int currentDialogue {
        get {
            return _currentDialogue;
        }
        set {
            _currentDialogue = value;
        }
    }

    public int houseIndex {
        get {
            return _houseIndex;
        }
        set {
            _houseIndex = value;
        }
    }
}
