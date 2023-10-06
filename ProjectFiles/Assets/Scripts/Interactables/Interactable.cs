using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public bool useEvents;
    public string promptMessage;
    public PlayerStatus Player;
    public GunSystem gunSystem;
    public ParticleSystem DestroyEffect;
    public virtual string OnLook()
    {
        return promptMessage;
    }
    public void BaseInteract()
    {
        if (useEvents)
            GetComponent<InteractionEvent>().OnInteract.Invoke();
        Interact();
        Destroy(gameObject);
    }
    protected virtual void Interact()
    {

    }
}
