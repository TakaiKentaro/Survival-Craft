using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : IItemHolder
{
    public static Inventory Instance;

    public event EventHandler OnItemListChanged;

    private List<Item> _itemList;
    private Action<Item> _useItemAction;
    public InventorySlot[] _inventorySlotArray;

    public Inventory(Action<Item> useItemAction, int inventorySlotCount)
    {
        this._useItemAction = useItemAction;
        _itemList = new List<Item>();

        _inventorySlotArray = new InventorySlot[inventorySlotCount];
        for (int i = 0; i < inventorySlotCount; i++)
        {
            _inventorySlotArray[i] = new InventorySlot(i);
        }

        // デバック用
        AddItem(new Item { itemType = Item.ItemType.Wood, amount = 12 });
        AddItem(new Item { itemType = Item.ItemType.Stick, amount = 1 });
        AddItem(new Item { itemType = Item.ItemType.Axe_Stone, amount = 1 });
        AddItem(new Item { itemType = Item.ItemType.Pickaxe_Stone, amount = 1 });
        AddItem(new Item { itemType = Item.ItemType.Stone, amount = 6 });
    }

    public void CallAddItem(Item.ItemType item, int amount)
    {
        AddItem(new Item { itemType = item, amount = amount });
    }

    public InventorySlot GetEmptyInventorySlot()
    {
        foreach (InventorySlot inventorySlot in _inventorySlotArray)
        {
            if (inventorySlot.IsEmpty())
            {
                return inventorySlot;
            }
        }

        Debug.LogError("空のInventorySlotが見つかりません");
        return null;
    }

    public InventorySlot GetInventorySlotWithItem(Item item)
    {
        foreach (InventorySlot inventorySlot in _inventorySlotArray)
        {
            if (inventorySlot.GetItem() == item)
            {
                return inventorySlot;
            }
        }

        Debug.LogError($"アイテム{item}がInventorySlotで見つかりません");
        return null;
    }

    public void AddItem(Item item)
    {
        _itemList.Add(item);
        item.SetItemHolder(this);
        GetEmptyInventorySlot().SetItem(item);
        OnItemListChanged?.Invoke(this, EventArgs.Empty);
    }

    public void AddItemMergeAmount(Item item)
    {
        // Adds an Item and increases amount if same ItemType already present
        if (item.IsStackable())
        {
            bool itemAlreadyInInventory = false;
            foreach (Item inventoryItem in _itemList)
            {
                if (inventoryItem.itemType == item.itemType)
                {
                    inventoryItem.amount += item.amount;
                    itemAlreadyInInventory = true;
                }
            }

            if (!itemAlreadyInInventory)
            {
                _itemList.Add(item);
                item.SetItemHolder(this);
                GetEmptyInventorySlot().SetItem(item);
            }
        }
        else
        {
            _itemList.Add(item);
            item.SetItemHolder(this);
            GetEmptyInventorySlot().SetItem(item);
        }

        OnItemListChanged?.Invoke(this, EventArgs.Empty);
    }

    public void RemoveItem(Item item)
    {
        GetInventorySlotWithItem(item).RemoveItem();
        _itemList.Remove(item);
        OnItemListChanged?.Invoke(this, EventArgs.Empty);
    }

    public void RemoveItemAmount(Item.ItemType itemType, int amount)
    {
        RemoveItemRemoveAmount(new Item { itemType = itemType, amount = amount });
    }

    public void RemoveItemRemoveAmount(Item item)
    {
        if (item.IsStackable())
        {
            Item itemInInventory = null;
            foreach (Item inventoryItem in _itemList)
            {
                if (inventoryItem.itemType == item.itemType)
                {
                    inventoryItem.amount -= item.amount;
                    itemInInventory = inventoryItem;
                }
            }

            if (itemInInventory != null && itemInInventory.amount <= 0)
            {
                GetInventorySlotWithItem(itemInInventory).RemoveItem();
                _itemList.Remove(itemInInventory);
            }
        }
        else
        {
            GetInventorySlotWithItem(item).RemoveItem();
            _itemList.Remove(item);
        }

        OnItemListChanged?.Invoke(this, EventArgs.Empty);
    }

    public void AddItem(Item item, InventorySlot inventorySlot)
    {
        // Add Item to a specific Inventory slot
        _itemList.Add(item);
        item.SetItemHolder(this);
        inventorySlot.SetItem(item);

        OnItemListChanged?.Invoke(this, EventArgs.Empty);
    }

    public void UseItem(Item item)
    {
        _useItemAction(item);
    }

    public List<Item> GetItemList()
    {
        return _itemList;
    }

    public InventorySlot[] GetInventorySlotArray()
    {
        return _inventorySlotArray;
    }

    public bool CanAddItem()
    {
        return GetEmptyInventorySlot() != null;
    }


    /// <summary>
    /// １つのインベントリスロットを表す
    /// </summary>
    public class InventorySlot
    {
        private int _index;
        private Item _item;

        public InventorySlot(int index)
        {
            this._index = index;
        }

        public Item GetItem()
        {
            return _item;
        }

        public void SetItem(Item item)
        {
            this._item = item;
        }

        public void RemoveItem()
        {
            _item = null;
        }

        public bool IsEmpty()
        {
            return _item == null;
        }
    }
}