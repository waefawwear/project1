using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MovableUI : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    [SerializeField]
    private Transform targetTr; // �̵��� UI

    private Vector2 startingPoint;
    private Vector2 moveBegin;
    private Vector2 moveOffset;

    private void Awake()
    {
        // �̵� ��� UI�� �������� ���� ���, �ڵ����� �θ�� �ʱ�ȭ
        if (targetTr == null)
            targetTr = transform.parent;
    }

    // �巡�� ���� ��ġ ����
    public void OnPointerDown(PointerEventData eventData)
    {
        startingPoint = targetTr.position;
        moveBegin = eventData.position;
    }

    // �巡�� : ���콺 Ŀ�� ��ġ�� �̵�
    public void OnDrag(PointerEventData eventData)
    {
        moveOffset = eventData.position - moveBegin;
        targetTr.position = startingPoint + moveOffset;
    }
}
