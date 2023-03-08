using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object_ToolManagement : MonoBehaviour
{
    [SerializeField] private Object_Tool _sword;
    [SerializeField] private Object_Tool _axe;
    [SerializeField] private Object_Tool _pickaxe;

    public void OnUse(string name)
    {
        switch(name)
        {
            case "Sword":
                {

                }
                break;

            case "Axe":
                {

                }
                break;

            case "Pickaxe":
                {

                }
                break;
        }
    }
}
