using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerStatus : MonoBehaviourPun
{
    public float health;
    public float maxHealth;
    public bool isDead;
    public int kills;
    public bool HPchange;
    [Header("Photon")]
    public int id;
    public Player photonPlayer;
    public bool photonViewIsMine = false;
    private Renderer childRenderer;
    private int curAttackerId;
    void Start()
    {
        health = maxHealth;
        childRenderer = GetComponentInChildren<Renderer>();
    }
    private void Update()
    {
       
    }
    [PunRPC]
    public void Initialize(Player player)
    {
        id = player.ActorNumber;
        photonPlayer = player;
        GameManager.instance.players[id - 1] = this;

        // is this not our local player?
        if (!photonView.IsMine)
        {
            GetComponentInChildren<Camera>().gameObject.SetActive(false);
        }
        else
        {
            GameUIManager.instance.Initialize(this);
        }

    }
    [PunRPC]
    public void OnHealChange(float changeValue)
    {
        if (isDead)
        {
            return;
        }
        health += changeValue;
        health = Mathf.Clamp(health,0,maxHealth);
        HPchange = changeValue > 0;
        if (!HPchange)
            GameUIManager.instance.ShowOverlay();
        GameUIManager.instance.clearLerpTimer();
        if (health <= 0)
        {
            Debug.Log("died");
            photonView.RPC("Die", RpcTarget.All);
            
        }
    }
    [PunRPC]
    void Die()
    {
        health = 0;
        isDead = true;
        GameManager.instance.alivePlayers--;
        // host will check win condition
        if (PhotonNetwork.IsMasterClient)
            GameManager.instance.CheckWinCondition();
        // is this our local player?
        if (photonView.IsMine)
        {
            if (curAttackerId != 0)
                GameManager.instance.GetPlayer(curAttackerId).photonView.RPC("AddKill", RpcTarget.All);
            // set the cam to spectator
            GetComponentInChildren<SetAsInspect>().SetAsSpectator();
            // disable the physics and hide the player
            //rig.isKinematic = true;
            transform.position = new Vector3(0, -50, 0);
        }
    }


}
