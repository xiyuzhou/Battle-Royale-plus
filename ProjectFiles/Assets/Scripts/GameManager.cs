using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;

public class GameManager : MonoBehaviourPun
{
    [Header("Players")]
    public string playerPrefabLocation;
    public PlayerStatus[] players;
    public Transform[] spawnPoints;
    public int alivePlayers;
    private int playersInGame;
    // instance
    public static GameManager instance;
    public float postGameTime;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        players = new PlayerStatus[PhotonNetwork.PlayerList.Length];
        alivePlayers = players.Length;
        photonView.RPC("ImInGame", RpcTarget.AllBuffered);
    }

    [PunRPC]
    void ImInGame()
    {
        playersInGame++;
        if (PhotonNetwork.IsMasterClient && playersInGame == PhotonNetwork.PlayerList.Length)
            photonView.RPC("SpawnPlayer", RpcTarget.All);
    }
    [PunRPC]
    void SpawnPlayer()
    {
        GameObject playerObj = PhotonNetwork.Instantiate(playerPrefabLocation, spawnPoints[Random.Range(0, spawnPoints.Length)].position, Quaternion.identity);
        // initialize the player for all other players
        playerObj.GetComponent<PlayerStatus>().photonView.RPC("Initialize", RpcTarget.All, PhotonNetwork.LocalPlayer);

    }
    public PlayerStatus GetPlayer(int playerId)
    {
        foreach (PlayerStatus player in players)
        {
            if (player != null && player.id == playerId)
                return player;
        }
        return null;

    }
    public PlayerStatus GetPlayer(GameObject playerObj)
    {
        foreach (PlayerStatus player in players)
        {
            if (player != null && player.gameObject == playerObj)
                return player;
        }
        return null;

    }
    public void CheckWinCondition()
    {
        if (alivePlayers == 1)
            photonView.RPC("WinGame", RpcTarget.All, players.First(x => !x.isDead).id);
    }
    [PunRPC]
    void WinGame(int winningPlayer)
    {
        // set the UI win text
        GameUIManager.instance.SetWinText(GetPlayer(winningPlayer).photonPlayer.NickName);
        Invoke("GoBackToMenu", postGameTime);
    }
    void GoBackToMenu()
    {
        NetworkManager.instance.ChangeScene("Menu");
    }
}
