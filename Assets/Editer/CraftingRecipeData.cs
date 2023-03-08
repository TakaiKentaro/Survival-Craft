using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu( menuName = "ScriptableObject", fileName = "CraftRecipeData" )]
[System.Serializable]
public class CraftingRecipeData : ScriptableObject
{
    public Item.ItemType[,] _itemTypes = new Item.ItemType[5, 5];
    public Item.ItemType _recipe = Item.ItemType.None;

    public void Init()
    {
        _itemTypes = new Item.ItemType[5, 5];
    }
}

[CustomEditor(typeof(CraftingRecipeData))]
public class CraftingRecipeDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        CraftingRecipeData data = (CraftingRecipeData)target;
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                data._itemTypes[i, j] = (Item.ItemType)EditorGUILayout.EnumPopup($"Item Type{i},{j}", data._itemTypes[i, j]);
            }
        }
    }
}