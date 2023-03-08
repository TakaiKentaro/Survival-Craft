using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class Object_UI : MonoBehaviour
{
    [Header("TEXT")]
    [SerializeField] private TextMeshProUGUI _text;

    [Header("Slider")] 
    [SerializeField] private Slider _hpSlider;
    
    [Header("Camera")]
    [SerializeField] float _hideDistance = 0;

    private CinemachineVirtualCamera _camera;
    private int _hp;
    private ObjectType _type;

    public void SetObject(ObjectType type, int hp, CinemachineVirtualCamera cam)
    {
        _camera = cam;
        _type = type;
        _text.text = _type.ToString();
        _hpSlider.maxValue = hp;
        _hpSlider.value = hp;
        _hp = hp;
    }

    public void OnDamage(int dmg)
    {
        _hpSlider.value = dmg;
    }

    private void Update()
    {
        if(!_camera) return;
        float camera = Vector3.Distance(_camera.transform.position, transform.position);

        if(camera <= _hideDistance)
        {
            Vector3 vector3 = _camera.transform.position - this.transform.position;
            Quaternion quaternion = Quaternion.LookRotation(vector3);
            this.transform.rotation = quaternion;
        }
    }
}
