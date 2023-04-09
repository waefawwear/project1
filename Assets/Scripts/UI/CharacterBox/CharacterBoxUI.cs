using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class CharacterBoxUI : MonoBehaviour
{
    // �⺻ ������ -----------------------------------------------------------------------------------------------------------------
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
        if (canvasGroup.blocksRaycasts)     // ���������� �ݰ�, ���������� ����
        {
            Close();
        }
        else
        {
            Open();
        }
    }

    void Open()     // ����
    {
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        GameManager.Inst.OnUiMode = true;
    }

    void Close()     // ���� ��
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        GameManager.Inst.OnUiMode = false;
    }
}
