using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSystem : MonoBehaviour
{
    [SerializeField] private TestPlayer _player;
    
    [SerializeField] private UI_Inventory _uiInventory;

    [SerializeField] private UI_CraftingSystem _uiCraftingSystem;

    private void Start()
    {
        _uiInventory.SetPlayer(_player);
        _uiInventory.SetInventory(_player.GetInventory());

        CraftingSystem craftingSystem = new CraftingSystem();
        
        _uiCraftingSystem.SetCraftingSystem(craftingSystem);
    }
}
