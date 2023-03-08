using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputName : MonoBehaviour
{
    InputField _inputField;

    private void Start()
    {
        _inputField = _inputField.GetComponent<InputField>();
    }

    public void InputUserName()
    {
        Debug.Log(_inputField.text);
    }
}
