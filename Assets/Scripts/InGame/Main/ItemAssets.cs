using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemAssets : MonoBehaviour
{
    public static ItemAssets Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public Transform _pfItemWorld;

    public Sprite s_Wood;
    public Sprite s_Stick;
    public Sprite s_Stone;
    public Sprite s_Sword_Stone;
    public Sprite s_Axe_Stone;
    public Sprite s_Pickaxe_Stone;
    public Sprite s_Boat;
}
