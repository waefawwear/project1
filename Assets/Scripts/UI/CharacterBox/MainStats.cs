using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class MainStats : MonoBehaviour
{
    Player player;

    TextMeshProUGUI value_Damage;
    TextMeshProUGUI value_Health;
    TextMeshProUGUI value_Mana;
    TextMeshProUGUI value_Armor;
    TextMeshProUGUI value_Critical;

    private void Awake()
    {
        player = FindObjectOfType<Player>();

        value_Damage = GameObject.Find("Damage").transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        value_Health = GameObject.Find("Health").transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        value_Mana = GameObject.Find("Mana").transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        value_Armor = GameObject.Find("Armor").transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        value_Critical = GameObject.Find("Critical").transform.GetChild(1).GetComponent<TextMeshProUGUI>();

        player.onDamageChange += DamageTextRefresh;
        player.onStaminaChange += StaminaTextRefresh;
        player.onManaStatChange += ManaTextRefresh;
        player.onDefenseChange += DefenseTextRefresh;
        player.onLuckChange += LuckTextRefresh;
    }

    private void Start()
    {
        AllRefresh();
    }

    void DamageTextRefresh()
    {
        value_Damage.text = player.Damage.ToString();
    }

    void StaminaTextRefresh()
    {
        value_Health.text = player.MaxHP.ToString();
    }

    void ManaTextRefresh()
    {
        value_Mana.text = player.MaxMP.ToString();
    }

    void DefenseTextRefresh()
    {
        value_Armor.text = player.DefencePower.ToString();
    }

    void LuckTextRefresh()
    {
        value_Critical.text = player.CriticalRate.ToString();
    }

    void AllRefresh()
    {
        DamageTextRefresh();
        StaminaTextRefresh();
        ManaTextRefresh();
        DefenseTextRefresh();
        LuckTextRefresh();
    }
}
