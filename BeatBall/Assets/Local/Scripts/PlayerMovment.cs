using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovment : MonoBehaviourPun
{
    [SerializeField] private Camera mainCam;

    [Header("Movment")]
    [SerializeField] private Transform parentTransform;
    [SerializeField] private Rigidbody myRigidBody;
    [SerializeField] private ConfigurableJoint hipscj;
    [SerializeField] private Animator animator;
    Vector3 direction;
    bool isFillingStamina = true;
   
    [Header("Hit")]
    
    [SerializeField] private Transform hitPos;
    [SerializeField] private LayerMask whomToHit;
    [SerializeField] private PhotonView pv_BallController;
    [SerializeField] private float radius;
    public float ballMaxHeight = 18; //6
    bool isPressedDown = false;



    [Header("Server")]
    PlayerStat playerStat;
    PhotonView view;


    public List<GameObject> Limbs = new List<GameObject>();
    public LayerMask GreenTeamLayerMask;
    GameObject playerListPanel;

   

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        playerListPanel = GameObject.Find("PlayerListing");
        playerStat = GetComponent<PlayerStat>();
        view = GetComponent<PhotonView>();

        StartCoroutine(Co_FillStamina());
        StartCoroutine(Co_FillHitForce());

       
    }

    private void Update()
    {
        if (view.IsMine)
        {
           
            
                Move();
                Jump();
                Hit();

            // should be more efficient and decently organized.
            if (Input.GetKey(KeyCode.Tab))
                playerListPanel.SetActive(true);
            else
                playerListPanel.SetActive(false);

        }
     
    }


    #region Movment

    // Move
    public void Move()
    {


        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        direction = new Vector3(horizontal, 0, vertical).normalized;




        // On Move
        if (direction.magnitude >= 0.1f)
        {
            animator.SetBool("Walk", true);

            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + mainCam.transform.eulerAngles.y;


            hipscj.targetRotation = Quaternion.Euler(0, -targetAngle, 0);


            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            myRigidBody.AddForce(move(moveDir));

            myRigidBody.velocity = new Vector3(0, myRigidBody.velocity.y, 0);

        }
        // On Idle
        else
        {
            myRigidBody.velocity = new Vector3(0, myRigidBody.velocity.y, 0);
            isFillingStamina = true;
            animator.SetBool("Walk", false);
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
            if (playerStat.Stamina < (playerStat.staminaStartValue + 1) && isFillingStamina)
            {

                playerStat.Stamina += 40;
                yield return new WaitForSeconds(0.15f);


            }

            yield return null;
        }

    }

    //Jump
    public void Jump()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            myRigidBody.AddForce(0, playerStat.JumpForce * Time.fixedDeltaTime, 0);
        }


    }


    #endregion

    #region Hit


    public void Hit()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isPressedDown = true;
        }
        if (Input.GetMouseButtonUp(0))
        {
            ApplyForceToBall();
            isPressedDown = false;
            playerStat.HitForce = 0;
        }
    }

    public void ApplyForceToBall()
    {
        Collider[] _hitcol = Physics.OverlapSphere(hitPos.position, radius, whomToHit);
        for (int i = 0; i < _hitcol.Length; i++)
        {
            if (_hitcol[i].tag == "ball")
            {
                GameObject _ball = _hitcol[i].gameObject;

                pv_BallController.RPC("RPC_HitBall", RpcTarget.AllViaServer);

                _ball.GetComponent<Rigidbody>().AddForce((mainCam.transform.forward * playerStat.HitForce) + new Vector3(0, Mathf.Clamp(ballMaxHeight - mainCam.transform.position.y, 0, ballMaxHeight), 0), ForceMode.Impulse);

            }


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


    #endregion

    public void TeleportToPos()
    {
        if (playerStat.IsRedTeamMember) 
            parentTransform.position = GameManager.ins.RedTeamPos;
        else
            parentTransform.position = GameManager.ins.GreenTeamPos;

        if (!PhotonManager.ins.HasGameBegun)
            view.RPC("SetPlayersTeam", RpcTarget.All);
    }

    [PunRPC]
    public void SetPlayersTeam()
    {
        foreach (var limb in Limbs)
        {
            if(playerStat.IsRedTeamMember) // RED
            {
                if(limb.TryGetComponent(out SkinnedMeshRenderer _limbmat))
                    _limbmat.material.color = Color.red;

            }
            else // GREEN
            {
                limb.layer = LayerMask.NameToLayer("BodyGreen");

                if (limb.TryGetComponent(out SkinnedMeshRenderer _limbmat))
                {
                    _limbmat.material.color = Color.green;
                   
                 
                }
                    
            
            }
        
        
        }
    }

}




















