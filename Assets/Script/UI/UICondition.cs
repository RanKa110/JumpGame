using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICondition : MonoBehaviour
{
    public Condition hp;
    public Condition hunger;
    public Condition stamina;


    void Start()
    {
        CharacterManager.Instance.Player.condition.uICondition = this;
    }

}
