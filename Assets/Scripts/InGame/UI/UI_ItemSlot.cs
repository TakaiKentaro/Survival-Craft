using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_ItemSlot : MonoBehaviour , IDropHandler
{
    private Action _onDropAction;

    public void SetOnDropAction(Action onDropAction)
    {
        this._onDropAction = onDropAction;
    }

    public void OnDrop(PointerEventData eventData)
    {
        UI_ItemDrag.Instance.Hide();
        _onDropAction();
    }
}
