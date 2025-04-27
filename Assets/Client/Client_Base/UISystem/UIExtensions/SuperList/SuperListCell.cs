
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class SuperListCell : MonoBehaviour, IPointerClickHandler
{
    protected Action<SuperListCell> cellClick = null;

    protected object data;
    protected bool selected;

    [HideInInspector] public CanvasGroup canvasGroup;
    [HideInInspector] public int index;

    public void OnPointerClick(PointerEventData eventData)
    {

    }

    public void SetClickHandle(Action<SuperListCell> cellClick)
    {
        this.cellClick = cellClick;
    }

    public void SetSelected(bool value)
    {
        selected = value;
    }
}
