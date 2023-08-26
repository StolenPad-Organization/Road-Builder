using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionUI : MonoBehaviour
{
    [SerializeField] private Animator anim;

    void Start()
    {
        
    }

    private void OnEnable()
    {
        EventManager.StartEndTransition += StartEndTransition;
    }

    private void OnDisable()
    {
        EventManager.StartEndTransition -= StartEndTransition;
    }

    void Update()
    {
        
    }

    private void StartEndTransition()
    {
        anim.SetTrigger("End");
    }

    //public void OnTransitionEnded()
    //{
    //    EventManager.InstantWin?.Invoke();
    //}
}
