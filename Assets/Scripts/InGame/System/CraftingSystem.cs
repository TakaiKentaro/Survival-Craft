using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public class CraftingSystem : IItemHolder
{
    public const int GRID_SIZE = 5;

    public event EventHandler OnGridChanged;

    private CraftingRecipeData[] _craftingRecipeDatas = Resources.LoadAll<CraftingRecipeData>("");
    private Dictionary<Item.ItemType, Item.ItemType[,]> _recipeDictionary;
    private Item[,] _itemArray;
    private Item _outputItem;

    public CraftingSystem()
    {
        _itemArray = new Item[GRID_SIZE, GRID_SIZE];
        _recipeDictionary = new Dictionary<Item.ItemType, Item.ItemType[,]>();
        
        for (int i = 0; i < _craftingRecipeDatas.Length; i++)
        {
            CraftingRecipeData craftingRecipeData = _craftingRecipeDatas[i];

            Debug.Log(craftingRecipeData);

            Item.ItemType[,] itemTypes = craftingRecipeData._itemTypes;
            Item.ItemType recipe = craftingRecipeData._recipe;

            Item.ItemType[,] craftRecipe = new Item.ItemType[GRID_SIZE, GRID_SIZE];

            for (int x = 0; x < GRID_SIZE; x++)
            {
                for (int y = 0; y < GRID_SIZE; y++)
                {
                    craftRecipe[x, y] = itemTypes[x, y];
                }
            }

            _recipeDictionary[recipe] = craftRecipe;
        }
    }

    public bool IsEmpty(int x, int y)
    {
        return _itemArray[x, y] == null;
    }

    public Item GetItem(int x, int y)
    {
        return _itemArray[x, y];
    }

    public void SetItem(Item item, int x, int y)
    {
        if (item != null)
        {
            item.RemoveFromItemHolder();
            item.SetItemHolder(this);
        }

        _itemArray[x, y] = item;
        CreateOutput();
        OnGridChanged?.Invoke(this, EventArgs.Empty);
    }

    public void IncreaseItemAmount(int x, int y)
    {
        GetItem(x, y).amount++;
        OnGridChanged?.Invoke(this, EventArgs.Empty);
    }

    public void DecreaseItemAmount(int x, int y)
    {
        if (GetItem(x, y) != null)
        {
            GetItem(x, y).amount--;
            if (GetItem(x, y).amount == 0)
            {
                RemoveItem(x, y);
            }

            OnGridChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public void RemoveItem(int x, int y)
    {
        SetItem(null, x, y);
    }

    public bool TryAddItem(Item item, int x, int y)
    {
        if (IsEmpty(x, y))
        {
            SetItem(item, x, y);
            return true;
        }
        else
        {
            if (item.itemType == GetItem(x, y).itemType)
            {
                IncreaseItemAmount(x, y);
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public void RemoveItem(Item item)
    {
        if (item == _outputItem)
        {
            // 出力したレシピを消す
            ConsumeRecipeItems();
            CreateOutput();
            OnGridChanged?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            // グリッドからアイテムを消す
            for (int x = 0; x < GRID_SIZE; x++)
            {
                for (int y = 0; y < GRID_SIZE; y++)
                {
                    if (GetItem(x, y) == item)
                    {
                        // Removed this one
                        RemoveItem(x, y);
                    }
                }
            }
        }
    }

    public void AddItem(Item item)
    {
    }

    public bool CanAddItem()
    {
        return false;
    }


    private Item.ItemType GetRecipeOutput()
    {
        foreach (Item.ItemType recipeItemType in _recipeDictionary.Keys)
        {
            Item.ItemType[,] recipe = _recipeDictionary[recipeItemType];

            bool completeRecipe = true;
            for (int x = 0; x < GRID_SIZE; x++)
            {
                for (int y = 0; y < GRID_SIZE; y++)
                {
                    if (recipe[x, y] != Item.ItemType.None)
                    {
                        // レシピのアイテム
                        if (IsEmpty(x, y) || GetItem(x, y).itemType != recipe[x, y])
                        {
                            // アイテム欄が空もしくは、 異なるItemType
                            completeRecipe = false;
                        }
                    }
                }
            }

            if (completeRecipe)
            {
                return recipeItemType;
            }
        }

        return Item.ItemType.None;
    }

    private void CreateOutput()
    {
        Item.ItemType recipeOutput = GetRecipeOutput();
        if (recipeOutput == Item.ItemType.None)
        {
            _outputItem = null;
        }
        else
        {
            _outputItem = new Item { itemType = recipeOutput };
            _outputItem.SetItemHolder(this);
        }
    }

    public Item GetOutputItem()
    {
        return _outputItem;
    }

    public void ConsumeRecipeItems()
    {
        for (int x = 0; x < GRID_SIZE; x++)
        {
            for (int y = 0; y < GRID_SIZE; y++)
            {
                DecreaseItemAmount(x, y);
            }
        }
    }
}