using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class KillingFrame : MonoBehaviour
{
    // Start is called before the first frame update
    Collider2D myCollider2D;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    //Создается с коллайдером
    private void OnValidate()
    {
        myCollider2D = GetComponent<Collider2D>();
    }


    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponentInChildren<AlienScript>() != null)
        {
            Destroy(collision.gameObject);
        }
    }


}