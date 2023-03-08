using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUserName : MonoBehaviour
{
    [SerializeField, Tooltip("���[�U�[�̖��O")] UserParam _mySelf;
    [SerializeField, Tooltip("���[�U�[�̖��O�e�L�X�g")] Text _name;

    private void Start()
    {
        _name.text = _mySelf.Name;
    }
}
