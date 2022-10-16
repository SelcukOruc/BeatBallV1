using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject playerListPanel;
    [SerializeField] private GameObject EscapeMenu;
    
    [SerializeField] private GameObject localplayer;
    [SerializeField] private CinemachineFreeLook cam;

    private void Start()
    {
        Invoke(nameof(FindLocalPlayer), 0.5f);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Tab))
            playerListPanel.SetActive(true);
        else
            playerListPanel.SetActive(false);

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (EscapeMenu.activeInHierarchy)
            {
                EscapeMenu.SetActive(false);
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                EscapeMenu.SetActive(true);
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.Confined;
            }
               
        }
            
    
    }

    public TMP_InputField SensivityInputFieldX;
    public TMP_InputField SensivityInputFieldY;
    public void ApplySettings()
    {

        cam.m_XAxis.m_MaxSpeed = int.Parse(SensivityInputFieldX.text);
        cam.m_YAxis.m_MaxSpeed = int.Parse(SensivityInputFieldY.text);

        SensivityInputFieldY.text = cam.m_YAxis.m_MaxSpeed.ToString();
        SensivityInputFieldX.text = cam.m_XAxis.m_MaxSpeed.ToString();


        Debug.Log("Input recieved succesfully.");
    }
    public void ExitGame()
    {
        Application.Quit();
    }

    public void FindLocalPlayer()
    {
        localplayer = GameObject.Find(PhotonNetwork.LocalPlayer.ActorNumber.ToString());
        cam = localplayer.transform.GetChild(1).GetComponent<CinemachineFreeLook>();
        Debug.Log("Found" + localplayer.name);
        
        SensivityInputFieldY.text = cam.m_YAxis.m_MaxSpeed.ToString();
        SensivityInputFieldX.text = cam.m_XAxis.m_MaxSpeed.ToString();
    }

    public void CursorVisibility(bool _isVisible)
    {
        if (_isVisible)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }   
    }

    public void Disconnect()
    {
        if (PhotonNetwork.InRoom)
            PhotonNetwork.LeaveRoom();

        SceneManager.LoadScene(1);

    }

}
