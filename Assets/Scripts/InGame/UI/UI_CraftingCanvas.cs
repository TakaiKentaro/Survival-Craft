using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_CraftingCanvas : MonoBehaviour
{
    [SerializeField] GameObject _uiCraftingSystem;

    [SerializeField] bool _isCrafting;

    private void Start()
    {
        if(!_isCrafting)
        {
            Cursor.visible = false;
            _uiCraftingSystem.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if(Input.GetButtonDown("Interact"))
        {
            _isCrafting = !_isCrafting;
            CanvasHide();
        }
    }

    void CanvasHide()
    {
        if (_isCrafting)
        {
            Cursor.visible = true;
            _uiCraftingSystem.gameObject.SetActive(true);
        }
        else
        {
            Cursor.visible = false;
            _uiCraftingSystem.gameObject.SetActive(false);
        }
    }
}
