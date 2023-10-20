using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealPack : Interactable
{
    public float HealAmount = 30;
    protected override void Interact()
    {
        Debug.Log("Interacted with " + gameObject.name);
        Player.photonView.RPC("OnHealChange", Player.photonPlayer, HealAmount);
    }
}
