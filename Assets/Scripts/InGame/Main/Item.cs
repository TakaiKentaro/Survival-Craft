using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Item
{
    public enum ItemType
    {
        None,
        Wood,
        Stick,
        Stone,
        Sword_Stone,
        Axe_Stone,
        Pickaxe_Stone,
        Boat,
    }

    public ItemType itemType;
    public int amount = 1;
    private IItemHolder itemHolder;


    public void SetItemHolder(IItemHolder itemHolder)
    {
        this.itemHolder = itemHolder;
    }

    public IItemHolder GetItemHolder()
    {
        return itemHolder;
    }

    public void RemoveFromItemHolder()
    {
        if (itemHolder != null)
        {
            // Remove from current Item Holder
            itemHolder.RemoveItem(this);
        }
    }

    public void MoveToAnotherItemHolder(IItemHolder newItemHolder)
    {
        RemoveFromItemHolder();
        newItemHolder.AddItem(this);
    }


    public Sprite GetSprite()
    {
        return GetSprite(itemType);
    }

    public static Sprite GetSprite(ItemType itemType)
    {
        switch (itemType)
        {
            default:
            //素材
            case ItemType.Wood: return ItemAssets.Instance.s_Wood;
            case ItemType.Stick: return ItemAssets.Instance.s_Stick;
            case ItemType.Stone: return ItemAssets.Instance.s_Stone;
            
            //武器
            case ItemType.Sword_Stone: return ItemAssets.Instance.s_Sword_Stone;
            case ItemType.Axe_Stone: return ItemAssets.Instance.s_Axe_Stone;
            case ItemType.Pickaxe_Stone: return ItemAssets.Instance.s_Pickaxe_Stone;
            case ItemType.Boat: return ItemAssets.Instance.s_Boat;
        }
    }

    public Color GetColor()
    {
        return GetColor(itemType);
    }

    public static Color GetColor(ItemType itemType)
    {
        switch (itemType)
        {
            default:
            case ItemType.Wood: return new Color(1, 1, 1);
            case ItemType.Stick: return new Color(1, 1, 1);
            case ItemType.Stone: return new Color(1, 1, 1);
            case ItemType.Sword_Stone: return new Color(1, 1, 1);
        }
    }

    public bool IsStackable()
    {
        return IsStackble(itemType);
    }

    public static bool IsStackble(ItemType itemType)
    {
        switch (itemType)
        {
            default:
            // 重ね可能   
            case ItemType.Wood:
            case ItemType.Stick:
            case ItemType.Stone:
                return true;

            // 重ね不可能
            case ItemType.Sword_Stone:
            case ItemType.Axe_Stone:
            case ItemType.Pickaxe_Stone:
            case ItemType.Boat:
                return false;
        }
    }

    public int GetCost()
    {
        return GetCost(itemType);
    }

    public static int GetCost(ItemType itemType)
    {
        switch (itemType)
        {
            default:
                return 0;
        }
    }
    
    public override string ToString()
    {
        return itemType.ToString();
    }
}