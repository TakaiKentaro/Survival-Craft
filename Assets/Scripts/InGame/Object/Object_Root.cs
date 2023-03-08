using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public enum ObjectType
{
    None,

    Wood,

    Stone,
}


public class Object_Root : MonoBehaviour
{
    [SerializeField] private ObjectType _objectType;
    [SerializeField] private int _hp;
    [SerializeField] CinemachineVirtualCamera _camera;

    [SerializeField] private Object_Body _body;
    [SerializeField] private Object_UI _ui;

    private void Start()
    {
        _camera = FindObjectOfType<CinemachineVirtualCamera>();

        _body.SetObject(_objectType);
        _ui.SetObject(_objectType, _hp, _camera);
    }

    public void OnCollisionDamage(string tag, int dmg)
    {
        DamageCalculation(tag, dmg);
    }

    private void DamageCalculation(string tag, int dmg)
    {
        switch (tag)
        {
            case "Axe":
                {
                    if (_objectType == ObjectType.Wood)
                    {
                        _hp -= dmg * 2;
                        _ui.OnDamage(_hp);
                    }
                    else
                    {
                        _hp -= dmg;
                        _ui.OnDamage(_hp);
                    }
                }
                break;
            case "Pickaxe":
                {
                    if (_objectType == ObjectType.Stone)
                    {
                        _hp -= dmg * 2;
                        _ui.OnDamage(_hp);
                    }
                    else
                    {
                        _hp -= dmg;
                        _ui.OnDamage(_hp);
                    }
                }
                break;
            default:
                {
                    _hp -= dmg;
                    _ui.OnDamage(_hp);
                }
                break;
        }

        if (_hp <= 0)
        {
            var drawAmount = UnityEngine.Random.Range(3, 5);

            //switch (_objectType)
            //{
            //    case ObjectType.Wood:
            //        {
            //            Inventory.Instance.CallAddItem(Item.ItemType.Wood, drawAmount);
            //        }
            //        break;
            //    case ObjectType.Stone:
            //        {
            //            Inventory.Instance.CallAddItem(Item.ItemType.Stone, drawAmount);
            //        }
            //        break;
            //}

            gameObject.SetActive(false);
        }
    }
}
