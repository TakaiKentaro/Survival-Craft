using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterEquipment : MonoBehaviour
{
    public event EventHandler OnEquipmentChanged;

    public enum EquipSlot
    {
        None,
        Helmet,
        Armor,
        Weapon,
    }
    
}
