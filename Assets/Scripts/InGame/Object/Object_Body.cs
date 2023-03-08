using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object_Body : MonoBehaviour
{
    private MeshRenderer _color;
    private ObjectType _type;

    private void Start()
    {
        _color = GetComponent<MeshRenderer>();
    }

    public void SetObject(ObjectType type)
    {
        _type = type;

        switch (_type)
        {
            case ObjectType.Wood:
            {
            }
                break;

            case ObjectType.Stone:
            {
            }
                break;

            case ObjectType.None:
                break;
        }
    }
}