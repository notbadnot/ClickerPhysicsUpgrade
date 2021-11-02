using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AlienSquadScript : MonoBehaviour
{
    [SerializeField] public int aliensInSquad = 1;
    [SerializeField] public GameObject[] alienPrefab;
    private int aliveInSquad;

    public event Action<GameObject> SquadIsReady;
    // Start is called before the first frame update
    void Start()
    {
        for (int alienMember = 0; alienMember < aliensInSquad; alienMember++)
        {
            var newMember = Instantiate(alienPrefab[0], transform);
            newMember.transform.localPosition = new Vector3(Mathf.Cos(Mathf.PI *2 / aliensInSquad *alienMember) * 3, Mathf.Sin(Mathf.PI *2 / aliensInSquad * alienMember)* 3, 0);
            HingeJoint2D hinge = gameObject.AddComponent<HingeJoint2D>();
            newMember.GetComponent<AlienScript>().ImGetShotedEvent += AlienSquadScript_ImGetShotedEvent;
            newMember.GetComponent<AlienScript>().ImDestroidEvent += AlienSquadScript_ImDestroidEvent;
            newMember.GetComponent<Rigidbody2D>().drag = 0;
            newMember.GetComponent<Rigidbody2D>().mass = 1000;
            hinge.connectedBody = newMember.GetComponent<Rigidbody2D>();
            hinge.useLimits = false;
            var _motor = hinge.motor;
            _motor.motorSpeed = 100;
            hinge.motor = _motor;
            hinge.useMotor = true;
            //var _limits = hinge.limits;
            //_limits.max = 360;
            //_limits.min = 0;
            // hinge.limits = _limits;
        }
        aliensInSquad = aliensInSquad;
        SquadIsReady?.Invoke(gameObject);
    }

    private void AlienSquadScript_ImDestroidEvent(GameObject obj)
    {
        obj.GetComponent<AlienScript>().ImGetShotedEvent -= AlienSquadScript_ImGetShotedEvent;
        obj.GetComponent<AlienScript>().ImDestroidEvent -= AlienSquadScript_ImDestroidEvent;
        aliensInSquad--;
        if (aliensInSquad < 1)
        {
            Destroy(gameObject);
        }
    }

    private void AlienSquadScript_ImGetShotedEvent(GameObject obj)
    {
        HingeJoint2D[] hingeJoints = gameObject.GetComponents<HingeJoint2D>();
        foreach (HingeJoint2D hingeJoint in hingeJoints)
        {
            if (hingeJoint.connectedBody == obj.GetComponent<Rigidbody2D>())
            {
                Destroy(hingeJoint);
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
