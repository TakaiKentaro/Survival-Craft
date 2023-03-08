using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class UI_Item : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    private Canvas _canvas;
    private RectTransform _rectTransform;
    private CanvasGroup _canvasGroup;
    private Image _image;
    private Item _item;
    private TextMeshProUGUI _amountText;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();
        _canvas = GetComponent<Canvas>();
        _image = transform.Find("Image").GetComponent<Image>();
        _amountText = transform.Find("AmountText").GetComponent<TextMeshProUGUI>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _canvasGroup.alpha = 0.5f;
        _canvasGroup.blocksRaycasts = false;
            UI_ItemDrag.Instance.Show(_item);
    }

    public void OnDrag(PointerEventData eventData)
    {
        //_rectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _canvasGroup.alpha = 1f;
        _canvasGroup.blocksRaycasts = true;
        UI_ItemDrag.Instance.Hide();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (_item != null)
            {
                if (_item.IsStackable())
                {
                    if (_item.amount > 1)
                    {
                        if (_item.GetItemHolder().CanAddItem())
                        {
                            int splitAmount = Mathf.FloorToInt(_item.amount / 2f);
                            _item.amount -= splitAmount;
                            Item duplicateItem = new Item{ itemType = _item.itemType, amount = splitAmount };
                            _item.GetItemHolder().AddItem(duplicateItem);
                        }
                    }
                }
            }
        }
    }

    public void SetSprite(Sprite sprite)
    {
        _image.sprite = sprite;
    }

    public void SetAmountText(int amount)
    {
        if (amount <= 1)
        {
            _amountText.text = "";
        }
        else
        {
            _amountText.text = amount.ToString();
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void SetItem(Item item)
    {
        this._item = item;
        SetSprite(Item.GetSprite(item.itemType));
        SetAmountText(item.amount);
    }
}