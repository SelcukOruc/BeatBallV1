using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class PlayerMovment : MonoBehaviourPunCallbacks
{
    [SerializeField] private Camera mainCam;

    [Header("Animation And Physics")]
    
    // Variables related to 'Ragdoll'.
    [SerializeField] private Transform playerParent;
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody hips;
    [SerializeField] ConfigurableJoint hipscj;

    // Variables related to 'Jump'.
    [SerializeField] private JumpScript leftFoot;
    [SerializeField] private JumpScript rightFoot;

    // Variables related to move
    Vector3 direction;
    bool isFillingStamina = true;

    // Variables related to 'Hit'.
    [Header("HIT")]
    [SerializeField] private Transform hitPoint;
    [SerializeField] private LayerMask whomToHit;
    [SerializeField] private float hitRadius;
    public GameObject HitVFX;
    bool isPressedDown = false;
    float ballMaxHeight = 18; //6

   

    [Header("Server Related")]
    // Server related Variables.
    PhotonView view;
    PhotonInGameController gameController;
    PlayerStat playerStat;

    [Header("Team Selection")]
    // Variables related to team selection.
    [SerializeField] private List<GameObject> limbs = new List<GameObject>();

    [SerializeField] private Material redTeamMat;
    [SerializeField] private Material yellowTeamMat;

    [SerializeField] private LayerMask redTeamLayer;

    GameObject fieldTeleportPointYellow;
    GameObject fieldTeleportPointRed;

   
    private void Start()
    {
        view = GetComponent<PhotonView>();
        playerStat = GetComponent<PlayerStat>();
        gameController = FindObjectOfType<PhotonInGameController>();

 

        fieldTeleportPointYellow = GameObject.Find("FieldTeleportPointYellow");
        fieldTeleportPointRed = GameObject.Find("FieldTeleportPointRed");

        StartCoroutine(Co_FillHitForce());
        StartCoroutine(Co_FillStamina());
       

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

      
    }

    void FixedUpdate()
    {
        
        if (view.IsMine)
            Move();

    }

    private void Update()
    {

       
       

        if (view.IsMine)
        {

            Jump();
            Hit();
           
            if (PhotonNetwork.IsMasterClient && Input.GetKeyDown(KeyCode.X) && !gameController.HasGameBegun)
                gameController.StartTheGame();

         
        }
       
    }






    #region Game Begun
    // RPC must stay because we sync not only transforms but also materials.
    // These Methods Only used at the begining of The game.
    // Therefore, players inital preferences that will last till the game ends can be written inside of these methods

    [PunRPC]
    public void Initial_TeleportToFieldYellow()
    {
        playerParent.SetPositionAndRotation(fieldTeleportPointYellow.transform.position, fieldTeleportPointYellow.transform.rotation);
        foreach (GameObject limb in limbs)
        {
            if (limb.TryGetComponent<MeshRenderer>(out MeshRenderer mesh))
                mesh.material = yellowTeamMat;

        }

    }

    [PunRPC]
    public void Initial_TeleportToFieldRed()
    {
        playerParent.SetPositionAndRotation(fieldTeleportPointRed.transform.position, fieldTeleportPointRed.transform.rotation);
        foreach (GameObject limb in limbs)
        {
            if(limb.TryGetComponent<MeshRenderer>(out MeshRenderer mesh))
                mesh.material = redTeamMat;

            limb.layer = redTeamLayer;
        }

    }
    #endregion


    #region Teams Scored


    public void OnYellowScored_Player()
    {
       
        if (playerStat.IsPlayerInYellowTeam)
        {
            playerParent.SetPositionAndRotation(fieldTeleportPointYellow.transform.position, fieldTeleportPointYellow.transform.rotation);
        }
        else
        {
            playerParent.SetPositionAndRotation(fieldTeleportPointRed.transform.position, fieldTeleportPointRed.transform.rotation);
        }

    }






    public void OnRedScoredP_Player()
    {
        
        if (!playerStat.IsPlayerInYellowTeam)
        {
            playerParent.SetPositionAndRotation(fieldTeleportPointRed.transform.position, fieldTeleportPointRed.transform.rotation);
        }
        else
        {
            playerParent.SetPositionAndRotation(fieldTeleportPointYellow.transform.position, fieldTeleportPointYellow.transform.rotation);
        }

    }










    #endregion


    #region Movment
    public void Move()
    {


        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        direction = new Vector3(horizontal, 0, vertical).normalized;




        // On Move
        if (direction.magnitude >= 0.1f)
        {

            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + mainCam.transform.eulerAngles.y;


            hipscj.targetRotation = Quaternion.Euler(0, -targetAngle, 0);

            animator.SetBool("isrun", true);
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            hips.AddForce(move(moveDir));
            
            hips.velocity = new Vector3(0, hips.velocity.y, 0);

        }
        // On Idle
        else
        {

            animator.SetBool("isrun", false);

            hips.velocity = new Vector3(0, hips.velocity.y, 0);
            isFillingStamina = true;
        }


    }

    Vector3 move(Vector3 _movedir)
    {
        if (Input.GetKey(KeyCode.LeftShift) && playerStat.Stamina > 0) // run
        {
            isFillingStamina = false;
            
            _movedir = _movedir.normalized * playerStat.MoveSpeed * 2.4f * Time.deltaTime;
            playerStat.Stamina -= 2;

        }
        else // walk
        {
            _movedir = _movedir.normalized * playerStat.MoveSpeed * Time.deltaTime;
           
            isFillingStamina = true;
        }

        return _movedir;
    }

   
    IEnumerator Co_FillStamina()
    {
        while (true)
        {
            if(playerStat.Stamina < (playerStat.staminaStartValue + 1) && isFillingStamina)
            {

                playerStat.Stamina += 4;
                yield return new WaitForSeconds(0.15f);
               
            
            }
          
            yield return null;
        }

    }



    public void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (leftFoot.IsGrounded || rightFoot.IsGrounded)
            {

                hips.AddForce(0, playerStat.jumpForce * Time.fixedDeltaTime, 0);

            }


        }


    }

    #endregion

    #region Hit

    // HIT
 
    public void Hit()
    {

        if (Input.GetMouseButtonUp(0))
        {
            // make anim work.
            animator.SetTrigger("hit");
            // Apply force to the target object.
            
            // Reset Hitforce
            isPressedDown = false;
            playerStat.HitForce = 0;

        }

        if (Input.GetMouseButtonDown(0))
        {
            isPressedDown = true;
        }


    }

    IEnumerator Co_FillHitForce()
    {
        while (true)
        {
            if (isPressedDown)
            {
                playerStat.HitForce = Mathf.Clamp(playerStat.HitForce, 0, playerStat.HitForceLimit);
                playerStat.HitForce += 4f;
                
                yield return new WaitForSeconds(0.1f);
            }

            yield return null;

        }

    }

    [PunRPC]
    public void ApplyForceToTarget()
    {

    }

#endregion







    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        gameController.GetComponent<PhotonView>().RPC("FindPlayersInRoom", RpcTarget.All);
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(hitPoint.position, hitRadius);
    }


 




}
































































