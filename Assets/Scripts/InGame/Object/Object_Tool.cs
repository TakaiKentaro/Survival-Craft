using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object_Tool : MonoBehaviour
{
    [SerializeField] int _dmg;

    private string _tagName;
    private BoxCollider _boxCollider;

    private void Start()
    {
        _tagName = gameObject.tag;
        _boxCollider = GetComponent<BoxCollider>();

        if(gameObject.tag != "Hand")
        {
            _boxCollider.enabled = false;
            gameObject.SetActive(false);
        }
        
    }

    public void OnUse()
    {
        if(gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
        
    }


    public void OnCollider()
    {
        if(!_boxCollider.enabled)
        {
            _boxCollider.enabled = true;
        }
        else
        {
            _boxCollider.enabled = false;
        }
        
    }
    


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Object_Root>())
        {
            Object_Root obj = other.gameObject.GetComponent<Object_Root>();

            obj.OnCollisionDamage(_tagName, _dmg);
        }
    }
}
