using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class IntelligenceStats : MonoBehaviour
{
    TextMeshProUGUI stat_Player_Text;


    TextMeshProUGUI stat_Max_Text;

    Button upBtn;
    Button downBtn;

    float stat_Player = 0;

    public float Stat_Player
    {
        get => stat_Player;
        set
        {
            if (stat_Player != value)
            {
                stat_Player = Mathf.Clamp(value, 0, stat_Max);
                stat_Player_Text.text = stat_Player.ToString();
            }
        }
    }

    float stat_Max = 100;

    public float Stat_Max
    {
        get => stat_Max;
        set
        {
            stat_Max = value;
            stat_Max_Text.text = stat_Max.ToString();
        }
    }

    private void Awake()
    {
        stat_Player_Text = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        stat_Max_Text = transform.GetChild(3).GetComponent<TextMeshProUGUI>();
        upBtn = transform.GetChild(4).GetComponent<Button>();
        downBtn = transform.GetChild(5).GetComponent<Button>();

        upBtn.onClick.AddListener(UpStats);
        downBtn.onClick.AddListener(DownStats);
    }

    void UpStats()
    {
        if(GameManager.Inst.StatPoints.StatPoint > 0)
        {
            Stat_Player++;
            GameManager.Inst.StatPoints.StatPoint--;
            GameManager.Inst.MainPlayer.onStatsChange?.Invoke();
        }
    }

    void DownStats()
    {
        if(Stat_Player >= 1)
        {
            Stat_Player--;
            GameManager.Inst.StatPoints.StatPoint++;
            GameManager.Inst.MainPlayer.onStatsChange?.Invoke();
        }
    }
}
