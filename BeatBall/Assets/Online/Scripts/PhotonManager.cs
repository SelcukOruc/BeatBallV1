using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonManager : MonoBehaviourPunCallbacks
{

    public static PhotonManager ins { get; private set; }
    private void Awake()
    {

        if (ins != null && ins != this)
        {
            Destroy(this);
        }
        else
        {
            ins = this;
        }

    }



   
    public bool HasGameBegun = false;

    [SerializeField] private GameObject redPlayerPrefab;
    [SerializeField] private GameObject ball;

    public List<GameObject> Players = new List<GameObject>();  
    
    void Start()
    {
     
        
            GameObject player = PhotonNetwork.Instantiate(redPlayerPrefab.name, GameManager.ins.StartPos, Quaternion.identity);
            player.transform.GetChild(0).gameObject.SetActive(true);
            player.transform.GetChild(1).gameObject.SetActive(true);
            player.transform.GetChild(2).gameObject.SetActive(true);
        
       
        
        
        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.InstantiateRoomObject(ball.name, Vector3.one * 2, Quaternion.identity);

      
        
    }
    
    
    [PunRPC]
    public void FindPlayers()
    {
        Players.Clear();
        
        foreach (var player in PhotonNetwork.PlayerList)
        {
            GameObject _playerFound = GameObject.Find(player.ActorNumber.ToString());
            Players.Add(_playerFound);
            
        }
        
       
    }
    public void MoveToPostions()
    {
        foreach (var player in Players)
        {
            PlayerMovment _playerMovment = player.GetComponent<PlayerMovment>();
            _playerMovment.TeleportToPos();

        }
    }
    [PunRPC]
    public void StartGame()
    {
        base.photonView.RPC("FindPlayers", RpcTarget.All);
        MoveToPostions();
        HasGameBegun = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.X) && PhotonNetwork.IsMasterClient && !HasGameBegun )
        {
            base.photonView.RPC("StartGame", RpcTarget.All);
        }


    }

   
    
  
    
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        base.photonView.RPC("FindPlayers", RpcTarget.All);
    }

}
