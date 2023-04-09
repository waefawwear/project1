using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class CharacterBoxUI : MonoBehaviour
{
    // 기본 데이터 -----------------------------------------------------------------------------------------------------------------
    CanvasGroup canvasGroup;

    public CanvasGroup Canvas => canvasGroup;
    // ----------------------------------------------------------------------------------------------------------------------------

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        Close();
    }

    public void CharacterBoxOnOffSwitch()
    {
        if (canvasGroup.blocksRaycasts)     // 열려있으면 닫고, 닫혀있으면 연다
        {
            Close();
        }
        else
        {
            Open();
        }
    }

    void Open()     // 열때
    {
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        GameManager.Inst.OnUiMode = true;
    }

    void Close()     // 닫을 때
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        GameManager.Inst.OnUiMode = false;
    }
}
