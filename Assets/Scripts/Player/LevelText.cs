using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelText : MonoBehaviour
{
    TextMeshProUGUI levelText;

    private void Awake()
    {
        levelText = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Start()
    {
        GameManager.Inst.MainPlayer.onLevelChange += LevelSet;
    }

    void LevelSet()
    {
        levelText.text = GameManager.Inst.MainPlayer.Level.ToString();
    }
}
