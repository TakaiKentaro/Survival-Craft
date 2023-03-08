using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening.Plugins.Core.PathCore;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEngine;

[CustomEditor(typeof(CraftingSystem))]
public class MyWindow : EditorWindow
{
    Item.ItemType[,] itemTypes = new Item.ItemType[5, 5];
    Item.ItemType recepi = Item.ItemType.None;
    private string assetName = "";

    [MenuItem("Window/CraftRecipeGenerator")]
    static void Open()
    {
        var window = GetWindow<MyWindow>();
        window.titleContent = new GUIContent("CraftRecipeGenerator");
    }

    void OnGUI()
    {
        GUILayout.Label( "クラフトレシピ" );
        
        EditorGUILayout.BeginHorizontal(GUI.skin.box);
        {
            itemTypes[0, 4] = (Item.ItemType)EditorGUILayout.EnumPopup(itemTypes[0, 4]);
            itemTypes[1, 4] = (Item.ItemType)EditorGUILayout.EnumPopup(itemTypes[1, 4]);
            itemTypes[2, 4] = (Item.ItemType)EditorGUILayout.EnumPopup(itemTypes[2, 4]);
            itemTypes[3, 4] = (Item.ItemType)EditorGUILayout.EnumPopup(itemTypes[3, 4]);
            itemTypes[4, 4] = (Item.ItemType)EditorGUILayout.EnumPopup(itemTypes[4, 4]);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal(GUI.skin.box);
        {
            itemTypes[0, 3] = (Item.ItemType)EditorGUILayout.EnumPopup(itemTypes[0, 3]);
            itemTypes[1, 3] = (Item.ItemType)EditorGUILayout.EnumPopup(itemTypes[1, 3]);
            itemTypes[2, 3] = (Item.ItemType)EditorGUILayout.EnumPopup(itemTypes[2, 3]);
            itemTypes[3, 3] = (Item.ItemType)EditorGUILayout.EnumPopup(itemTypes[3, 3]);
            itemTypes[4, 3] = (Item.ItemType)EditorGUILayout.EnumPopup(itemTypes[4, 3]);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal(GUI.skin.box);
        {
            itemTypes[0, 2] = (Item.ItemType)EditorGUILayout.EnumPopup(itemTypes[0, 2]);
            itemTypes[1, 2] = (Item.ItemType)EditorGUILayout.EnumPopup(itemTypes[1, 2]);
            itemTypes[2, 2] = (Item.ItemType)EditorGUILayout.EnumPopup(itemTypes[2, 2]);
            itemTypes[3, 2] = (Item.ItemType)EditorGUILayout.EnumPopup(itemTypes[3, 2]);
            itemTypes[4, 2] = (Item.ItemType)EditorGUILayout.EnumPopup(itemTypes[4, 2]);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal(GUI.skin.box);
        {
            itemTypes[0, 1] = (Item.ItemType)EditorGUILayout.EnumPopup(itemTypes[0, 1]);
            itemTypes[1, 1] = (Item.ItemType)EditorGUILayout.EnumPopup(itemTypes[1, 1]);
            itemTypes[2, 1] = (Item.ItemType)EditorGUILayout.EnumPopup(itemTypes[2, 1]);
            itemTypes[3, 1] = (Item.ItemType)EditorGUILayout.EnumPopup(itemTypes[3, 1]);
            itemTypes[4, 1] = (Item.ItemType)EditorGUILayout.EnumPopup(itemTypes[4, 1]);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal(GUI.skin.box);
        {
            itemTypes[0, 0] = (Item.ItemType)EditorGUILayout.EnumPopup(itemTypes[0, 0]);
            itemTypes[1, 0] = (Item.ItemType)EditorGUILayout.EnumPopup(itemTypes[1, 0]);
            itemTypes[2, 0] = (Item.ItemType)EditorGUILayout.EnumPopup(itemTypes[2, 0]);
            itemTypes[3, 0] = (Item.ItemType)EditorGUILayout.EnumPopup(itemTypes[3, 0]);
            itemTypes[4, 0] = (Item.ItemType)EditorGUILayout.EnumPopup(itemTypes[4, 0]);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal(GUI.skin.box);
        recepi = (Item.ItemType)EditorGUILayout.EnumPopup("作成アイテム",recepi);
        if (GUILayout.Button("リセット"))
        {
            Reset();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal(GUI.skin.box);
        GUILayout.Label( "データのアセット名" );
        assetName = (string)GUILayout.TextField(assetName);
        EditorGUILayout.EndHorizontal();
        
        if(GUILayout.Button("レシピ作成",GUILayout.Height(100)))
        {
            Debug.Log("レシピ作成");
            CreatRecepi();

            itemTypes = null;
            itemTypes = new Item.ItemType[5, 5];
        }

        void CreatRecepi()
        {
            CraftingRecipeData _assets = ScriptableObject.CreateInstance<CraftingRecipeData>();
            _assets.Init();
            _assets._itemTypes = itemTypes;
            _assets._recipe = recepi;
            AssetDatabase.CreateAsset(_assets,$"Assets/Resources/{assetName}.asset");
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = _assets;
            AssetDatabase.SaveAssets();
            EditorUtility.SetDirty(_assets);
            //AssetDatabase.Refresh();
            //Reset();
        }

        void Reset()
        {
            for (int i = 0; i < itemTypes.GetLength(0); i++)
            {
                for (int j = 0; j < itemTypes.GetLength(1); j++)
                {
                    itemTypes[i, j] = Item.ItemType.None;
                }
            }
            recepi = Item.ItemType.None;
            assetName = "";
        }
    }
}