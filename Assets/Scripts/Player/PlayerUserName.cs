using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUserName : MonoBehaviour
{
    [SerializeField, Tooltip("ユーザーの名前")] UserParam _mySelf;
    [SerializeField, Tooltip("ユーザーの名前テキスト")] Text _name;

    private void Start()
    {
        _name.text = _mySelf.Name;
    }
}
