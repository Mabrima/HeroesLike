using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatViewer : MonoBehaviour
{

    [SerializeField] Text healthText;
    [SerializeField] Text attackText;
    [SerializeField] Text defenceText;
    [SerializeField] Text damageText;
    [SerializeField] Text speedText;
    [SerializeField] Text initiativeText;

    public void UpdateStats(int currentHealth, int maxHealth, int attack, int defence, int minDamage, int maxDamage, int speed, int initiative)
    {
        healthText.text = currentHealth + "/" + maxHealth;
        attackText.text = "" + attack;
        defenceText.text = "" + defence;
        damageText.text = "" + minDamage + " - " + maxDamage;
        speedText.text = "" + speed;
        initiativeText.text = "" + initiative;
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void StopShow()
    {
        gameObject.SetActive(false);
    }

}
