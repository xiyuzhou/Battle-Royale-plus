using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ForceField : MonoBehaviour
{
    public float shrinkWaitTime;
    public float shrinkAmount;
    public float shrinkDuration;
    public float minShrinkAmount;
    public float playerDamage;
    private float lastShrinkEndTime;
    private bool shrinking;
    private float targetDiameter;
    private float lastPlayerCheckTime;

    void Start()
    {
        lastShrinkEndTime = Time.time;
        targetDiameter = transform.localScale.x;
    }
    void Update()
    {
        if (shrinking)
        {
            transform.localScale = Vector3.MoveTowards(
                        transform.localScale,
                        Vector3.one * targetDiameter,
                        (shrinkAmount / shrinkDuration) * Time.deltaTime);
            if (transform.localScale.x == targetDiameter)
                shrinking = false;
        }
        else
        {
            // can we shrink again?
            if (Time.time - lastShrinkEndTime >= shrinkWaitTime && transform.localScale.x > minShrinkAmount)
                Shrink();

        }
        CheckPlayers();
    }
    void Shrink()
    {
        shrinking = true;
        // make sure we don't shrink below the min amount
        if (transform.localScale.x - shrinkAmount > minShrinkAmount)
            targetDiameter -= shrinkAmount;
        else
            targetDiameter = minShrinkAmount;
        lastShrinkEndTime = Time.time + shrinkDuration;
    }
    void CheckPlayers()
    {
        if (Time.time - lastPlayerCheckTime > 1.0f)
        {
            lastPlayerCheckTime = Time.time;
            // loop through all players
            foreach (PlayerStatus player in GameManager.instance.players)
            {
                if (player == null)
                    return;
                if (player.isDead || !player)
                    continue;
                if (Vector3.Distance(transform.position, player.transform.position) >= transform.localScale.x)
                {
                    player.photonView.RPC("OnHealChange", player.photonPlayer, playerDamage);
                }
            }
        }
    }
}

