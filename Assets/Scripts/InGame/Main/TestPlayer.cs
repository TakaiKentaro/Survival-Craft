using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayer : MonoBehaviour
{
    public static TestPlayer Instance { get; private set; }

    private Inventory _inventory;
    
    private void Awake()
    {
        _inventory = new Inventory(UseItem, 7);
    }

    public void UseItem(Item inventoryItem)
    {
        Debug.Log("Use Item: " + inventoryItem);
    }
    
    public Inventory GetInventory() {
        return _inventory;
    }
}
