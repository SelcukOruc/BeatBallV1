using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerMovment : MonoBehaviourPunCallbacks
{
    // Variables will be rearranged. 30.09.2022;

    // Variables related to 'Animation and Physics'.

    [SerializeField] private Animator animator;

    [SerializeField] private Rigidbody hips;

    [SerializeField] ConfigurableJoint hipscj;

    public float MoveSpeed; //-
    [SerializeField] private Camera mainCam;
    // Variables related to 'Jump'.
    [SerializeField] private JumpScript LeftFoot;
    [SerializeField] private JumpScript RightFoot;
    [SerializeField] private float jumpForce; //-

    // Variables related to 'Hit'.
   
  
   
    [SerializeField] private float hitRadius; //-

    [SerializeField] private Transform hitPoint;
    [SerializeField] LayerMask WhomToHit;
    public GameObject PumVFX;

    // Server related Variables.
    PhotonView view;
    

    GameObject fieldTeleportPointYellow;
    GameObject fieldTeleportPointRed;
    PhotonInGameController gameController;
    PlayerStat playerStat;
    [SerializeField] private List<GameObject> Limbs = new List<GameObject>();
    
    [SerializeField] private Material RedTeamMat;
    [SerializeField] private Material YellowTeamMat;
    
    [SerializeField] private LayerMask RedTeamLayer;

    [SerializeField] private Transform playerParent;

    Vector3 direction;
   
    float staminaTimer;


    bool IsPressedDown = false;
    private void Start()
    {
        view = GetComponent<PhotonView>();
        playerStat = GetComponent<PlayerStat>();
        gameController = FindObjectOfType<PhotonInGameController>();
       
        fieldTeleportPointYellow = GameObject.Find("FieldTeleportPointYellow");
        fieldTeleportPointRed = GameObject.Find("FieldTeleportPointRed");

        StartCoroutine(FillHitForce());

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
            staminaTimer += Time.deltaTime;

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
        FindPos(playerParent, fieldTeleportPointYellow.transform);
        foreach (GameObject limb in Limbs)
        {
            limb.GetComponent<MeshRenderer>().material = YellowTeamMat;

        }

    }

    [PunRPC]
    public void Initial_TeleportToFieldRed()
    {
        FindPos(playerParent, fieldTeleportPointRed.transform);

        foreach (GameObject limb in Limbs)
        {
            limb.GetComponent<MeshRenderer>().material = RedTeamMat;

            limb.layer = RedTeamLayer;
        }

    }
    #endregion


    #region Teams Scored


    public void OnYellowScored_Player()
    {
        if (playerStat.IsPlayerInYellowTeam)
        {

            FindPos(playerParent, fieldTeleportPointYellow.transform);
        }
        else
        {

            FindPos(playerParent, fieldTeleportPointRed.transform);
        }




    }

    public void OnRedScoredP_Player()
    {
        if (!playerStat.IsPlayerInYellowTeam)
        {

            FindPos(playerParent, fieldTeleportPointRed.transform);
        }
        else
        {

            FindPos(playerParent, fieldTeleportPointYellow.transform);
        }




    }

    Transform FindPos(Transform _myPos, Transform _targetPos)
    {
        _myPos.position = _targetPos.position;
        _myPos.rotation = _targetPos.rotation;

        return _myPos;
    }


    #endregion


    #region physics
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
            FillStamina();
        }


    }

    Vector3 move(Vector3 _movedir)
    {
        if (Input.GetKey(KeyCode.LeftShift) && playerStat.Stamina > 0)
        {
            _movedir = _movedir.normalized * MoveSpeed * 2.4f * Time.deltaTime;
            playerStat.Stamina -= 2;

        }
        else
        {
            _movedir = _movedir.normalized * MoveSpeed * Time.deltaTime;
            FillStamina();
        }

        return _movedir;
    }

    void FillStamina()
    {
        if (staminaTimer > 0.15f && playerStat.Stamina < 100)
        {
            staminaTimer = 0;
            playerStat.Stamina += 1;


        }


    }


    public void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (LeftFoot.IsGrounded || RightFoot.IsGrounded)
            {

                hips.AddForce(0, jumpForce * Time.fixedDeltaTime, 0);

            }


        }


    }

    public void Hit()
    {

        if (Input.GetMouseButtonUp(0))
        {
            // make anim work.
            animator.SetTrigger("hit");
            // Apply force to the target object.
            view.RPC("ApplyForceToTarget", RpcTarget.All);
            // Reset Hitforce
            IsPressedDown = false;
            playerStat.HitForce = 0;

        }

        if (Input.GetMouseButtonDown(0))
        {
            IsPressedDown = true;
        }


    }

    IEnumerator FillHitForce()
    {
        while (true)
        {
            if (IsPressedDown)
            {
                playerStat.HitForce = Mathf.Clamp(playerStat.HitForce, 0, playerStat.HitForceLimit);
                playerStat.HitForce += 2f;
                Debug.Log(playerStat.HitForce);
                yield return new WaitForSeconds(0.1f);
            }

            yield return null;

        }

    }



    [PunRPC]
    public void ApplyForceToTarget()
    {

        Collider[] _hitSphere = Physics.OverlapSphere(hitPoint.position, hitRadius, WhomToHit);
        for (int i = 0; i < _hitSphere.Length; i++)
        {

            if (_hitSphere[i].GetComponent<Rigidbody>() != null)
            {
                if (_hitSphere[i].tag == "Ball" && _hitSphere[i].GetComponent<SpringJoint>() != null)
                {
                    SpringJoint _balljoint = _hitSphere[i].GetComponent<SpringJoint>();
                    _balljoint.spring = 0;
                    _balljoint.breakForce = 0;
                    _balljoint.breakTorque = 0;
                    _balljoint.connectedBody = null;

                    _hitSphere[i].GetComponent<Rigidbody>().AddForce(hitPoint.forward * playerStat.HitForce, ForceMode.Impulse);



                }


                if (_hitSphere[i].tag == "Player")
                {
                    _hitSphere[i].GetComponent<Rigidbody>().AddForce(hitPoint.forward * playerStat.HitForce * 30, ForceMode.Impulse);

                }

                // Will be excecuted in a better way when switched to online.
                Instantiate(PumVFX, _hitSphere[i].transform.position, Quaternion.identity);

            }


        }


    }



    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(hitPoint.position, hitRadius);
    }

    #endregion



    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        gameController.GetComponent<PhotonView>().RPC("FindPlayersInRoom", RpcTarget.All);
    }


}
































