using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamagelbe
{
    void TakePhysicalDamage(int damage);
}


public class PlayerCondition : MonoBehaviour, IDamagelbe
{
    public UICondition uICondition;

    Condition hp { get { return uICondition.hp; } }
    Condition hunger { get { return uICondition.hunger; } }
    public Condition stamina { get { return uICondition.stamina; } }

    public float noHungerHPDecay;


    public event Action onTakeDamage;


    void Update()
    {
        //hunger.Subtrac(hunger.passiveValue * Time.deltaTime);
        
        stamina.Add(stamina.passiveValue * Time.deltaTime);
        
        /*
        if(hunger.curValue <= 0f)
        {
            hp.Subtrac(noHungerHPDecay * Time.deltaTime);
        }
        */
        if (hp.curValue <= 0f)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        hp.Add(amount);
    }

    public void Eat(float amount) 
    {
        hunger.Add(amount); 
    }

    public void Die()
    {
        Debug.Log("ав╬З╢ы!");
    }

    public void TakePhysicalDamage(int damage)
    {
        hp.Subtrac(damage);
        onTakeDamage?.Invoke();
    }

    public bool UseStamina(float amount)
    {
        if(stamina.curValue - amount < 0f)
        {
            return false;
        }

        stamina.Subtrac(amount);
        return true;
    }
}
