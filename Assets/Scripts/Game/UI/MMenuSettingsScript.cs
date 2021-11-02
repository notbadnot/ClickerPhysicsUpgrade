using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class MMenuSettingsScript : MonoBehaviour
{
    public event Action PressBabyEvent;
    public event Action PressHumanEvent;
    public event Action PressAlienEvent;
    public event Action PressAcceptEvent;
    public event Action PressCancelEvent;
    public event Action<bool> CrowdingChangeEvent;

    private Toggle crowdingToogle;
    public void OnPressBaby() 
    {
        PressBabyEvent?.Invoke();
    }

    public void OnPressHuman()
    {
        PressHumanEvent?.Invoke();
    }
    public void OnPressAlien()
    {
        PressAlienEvent?.Invoke();
    }
    public void OnPressAccept()
    {
        PressAcceptEvent.Invoke();
    }
    public void OnPressCancel()
    {
        PressCancelEvent?.Invoke();
    }
    public void OnCrowdingChange()
    {
        CrowdingChangeEvent?.Invoke(crowdingToogle.isOn);
    }
    // Start is called before the first frame update
    void Start()
    {
        crowdingToogle = gameObject.transform.Find("Panel").Find("Toggle").GetComponent<Toggle>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
