using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPack : Interactable
{
    public int Ammo = 30;
    protected override void Interact()
    {
        gunSystem.GainAmmo(Ammo);
    }
}
