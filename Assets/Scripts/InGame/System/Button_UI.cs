using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Button_UI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler,
    IPointerDownHandler, IPointerUpHandler
{
    public Action ClickFunc = null;
    public Action MouseRightClickFunc = null;
    public Action MouseMiddleClickFunc = null;
    public Action MouseDownOnceFunc = null;
    public Action MouseUpFunc = null;
    public Action MouseOverOnceTooltipFunc = null;
    public Action MouseOutOnceTooltipFunc = null;
    public Action MouseOverOnceFunc = null;
    public Action MouseOutOnceFunc = null;
    public Action MouseOverFunc = null;
    public Action MouseOverPerSecFunc = null;
    public Action MouseUpdate = null;
    public Action<PointerEventData> OnPointerClickFunc;

    public enum HoverBehaviour
    {
        Custom,
        Change_Color,
        Change_Image,
        Change_SetActive,
    }

    public HoverBehaviour _hoverBehaviourType = HoverBehaviour.Custom;
    private Action _hoverBehaviourFuncEnter, _hoverBehaviourFuncExit;
    public Color _hoverBehaviourColorEnter, _hoverBehaviourColorExit;
    public Image _hoverBehaviourimage;
    public Sprite _hoverBehaviourSprite_Enter, _hoverBehaviourSprite_Exit;
    public bool _hoverBehaviourMove = false;
    public Vector2 _hoverBehaviourMoveAmount = Vector2.zero;
    private Vector2 _posEnter, _posExit;
    public bool _triggerMouseOutFuncOnClick = false;
    private bool _mouseOver;
    private float _mouseOverPerSecFuncTImer;

    private Action _internalOnPointerEnterFunc, _internalOnPointerExitFunc, _internalOnPointerClickFunc;

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        if (_internalOnPointerEnterFunc != null) _internalOnPointerEnterFunc();
        if (_hoverBehaviourMove) transform.localPosition = _posEnter;
        if (_hoverBehaviourFuncEnter != null) _hoverBehaviourFuncEnter();
        if (MouseOverOnceFunc != null) MouseOverOnceFunc();
        if (MouseOverOnceTooltipFunc != null) MouseOverOnceTooltipFunc();
        _mouseOver = true;
        _mouseOverPerSecFuncTImer = 0f;
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        if (_internalOnPointerExitFunc != null) _internalOnPointerExitFunc();
        if (_hoverBehaviourMove) transform.localPosition = _posExit;
        if (_hoverBehaviourFuncExit != null) _hoverBehaviourFuncExit();
        if (MouseOutOnceFunc != null) MouseOutOnceFunc();
        if (MouseOutOnceTooltipFunc != null) MouseOutOnceTooltipFunc();
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if (_internalOnPointerClickFunc != null) _internalOnPointerClickFunc();
        if (OnPointerClickFunc != null) OnPointerClickFunc(eventData);
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (_triggerMouseOutFuncOnClick)
            {
                OnPointerExit(eventData);
            }

            if (ClickFunc != null) ClickFunc();
        }

        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (MouseRightClickFunc != null) MouseRightClickFunc();
        }

        if (eventData.button == PointerEventData.InputButton.Middle)
        {
            if (MouseMiddleClickFunc != null) MouseMiddleClickFunc();
        }
    }

    public void Manual_OnPointerExit()
    {
        OnPointerExit(null);
    }

    public bool IsMouseOver()
    {
        return _mouseOver;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (MouseDownOnceFunc != null) MouseDownOnceFunc();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (MouseUpFunc != null) MouseUpFunc();
    }

    private void Awake()
    {
        _posExit = transform.localPosition;
        _posEnter = (Vector2)transform.localPosition + _hoverBehaviourMoveAmount;
        SetHoverBehaviourType(_hoverBehaviourType);
    }

    private void Update()
    {
        if (_mouseOver)
        {
            if (MouseOverFunc != null)
            {
                MouseOverFunc();
            }

            _mouseOverPerSecFuncTImer -= Time.unscaledDeltaTime;
            if (_mouseOverPerSecFuncTImer <= 0)
            {
                _mouseOverPerSecFuncTImer += 1f;
                if (MouseOverOnceFunc != null)
                {
                    MouseOverPerSecFunc();
                }
            }
        }

        if (MouseUpdate != null)
        {
            MouseUpdate();
        }
    }

    public void SetHoverBehaviourType(HoverBehaviour hoverBehaviourType)
    {
        this._hoverBehaviourType = hoverBehaviourType;
        switch (hoverBehaviourType)
        {
            case HoverBehaviour.Change_Color:
            {
                _hoverBehaviourFuncEnter = delegate { _hoverBehaviourimage.color = _hoverBehaviourColorEnter; };
                _hoverBehaviourFuncExit = delegate { _hoverBehaviourimage.color = _hoverBehaviourColorExit; };
            }
                break;
            case HoverBehaviour.Change_Image:
            {
                _hoverBehaviourFuncEnter = delegate { _hoverBehaviourimage.sprite = _hoverBehaviourSprite_Enter; };
                _hoverBehaviourFuncExit = delegate { _hoverBehaviourimage.sprite = _hoverBehaviourSprite_Exit; };
            }
                break;
            case HoverBehaviour.Change_SetActive:
            {
                _hoverBehaviourFuncEnter = delegate { _hoverBehaviourimage.gameObject.SetActive(true); };
                _hoverBehaviourFuncExit = delegate { _hoverBehaviourimage.gameObject.SetActive(true); };
            }
                break;
        }
    }

    public class InterceptActionHandler
    {
        private Action _removeInterceptFunc;

        public InterceptActionHandler(Action removeInterceptFunc)
        {
            this._removeInterceptFunc = removeInterceptFunc;
        }

        public void RemoveIntercept()
        {
            _removeInterceptFunc();
        }
    }

    public InterceptActionHandler InterceptActionClick(Func<bool> testPassthrougFunc)
    {
        return InterceptAction("ClickFunc", testPassthrougFunc);
    }

    public InterceptActionHandler InterceptAction(string fieldName, Func<bool> testPassthrougFunc)
    {
        return InterceptAction(GetType().GetField(fieldName), testPassthrougFunc);
    }

    public InterceptActionHandler InterceptAction(System.Reflection.FieldInfo fieldInfo, Func<bool> testPassthroughFunc)
    {
        Action backFunc = fieldInfo.GetValue(this) as Action;
        InterceptActionHandler interceptActionHandler =
            new InterceptActionHandler(() => fieldInfo.SetValue(this, backFunc));
        fieldInfo.SetValue(this,(Action)delegate()
        {
            if (testPassthroughFunc())
            {
                interceptActionHandler.RemoveIntercept();
                backFunc();
            }
        });
        return interceptActionHandler;
    }
}