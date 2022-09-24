using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerMovment : MonoBehaviourPunCallbacks
{
    // Variables related to 'Animation and Physics'.

    [SerializeField] private Animator animator;

    [SerializeField] private Rigidbody hips;

    [SerializeField] ConfigurableJoint hipscj;

    public float MoveSpeed;
    [SerializeField] private Camera mainCam;
    // Variables related to 'Jump'.
    [SerializeField] private JumpScript LeftFoot;
    [SerializeField] private JumpScript RightFoot;
    [SerializeField] private float jumpForce;

    // Variables related to 'Hit'.
    float timer;
    [Range(0, 20)]
    [SerializeField] private float HitForce = 9;
    [SerializeField] private float hitDelay;
    [SerializeField] private float hitRadius;

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

    private void Start()
    {
        view = GetComponent<PhotonView>();
        playerStat = GetComponent<PlayerStat>();
        gameController = FindObjectOfType<PhotonInGameController>();
       
        fieldTeleportPointYellow = GameObject.Find("FieldTeleportPointYellow");
        fieldTeleportPointRed = GameObject.Find("FieldTeleportPointRed");
        
       
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
    [PunRPC]
    public void TeleportToFieldYellow()
    {
        FindPos(playerParent, fieldTeleportPointYellow.transform);
        foreach (GameObject limb in Limbs)
        {
            limb.GetComponent<MeshRenderer>().material = YellowTeamMat;

        }

    }
    [PunRPC]
    public void TeleportToFieldRed()
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
   
    [PunRPC]
    public void OnYellowScoredP()
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
    [PunRPC]
    public void OnRedScoredP()
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
    Transform FindPos(Transform _myPos,Transform _targetPos)
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
        Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;


       


        if (direction.magnitude >= 0.1f)
        {

            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + mainCam.transform.eulerAngles.y;

          
            hipscj.targetRotation = Quaternion.Euler(0, -targetAngle, 0);

            animator.SetBool("isrun", true);
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            hips.AddForce(moveDir.normalized * MoveSpeed * Time.deltaTime);
            hips.velocity = new Vector3(0, hips.velocity.y, 0);

        }
        //set animation to idle.
        else
        {

            animator.SetBool("isrun", false);
            hips.velocity = new Vector3(0, hips.velocity.y, 0);

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
        timer += Time.deltaTime;

        if (Input.GetMouseButtonDown(0))
        {
            if (timer >= hitDelay)
            {
                // make anim work.
                timer = 0;
                animator.SetTrigger("hit");


                // Apply force to the target object.
                if(view.IsMine)
                view.RPC("ApplyForceToTarget",RpcTarget.All);

            }


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
                }

                _hitSphere[i].GetComponent<Rigidbody>().AddForce(hitPoint.forward * HitForce, ForceMode.Impulse);
                Debug.Log("Vurduk Paþam. " + _hitSphere[i].name);
               

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
   
    [PunRPC]
    public void OnFallen()
    {
        playerParent.transform.position = Vector3.zero;
    }

   

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        gameController.GetComponent<PhotonView>().RPC("FindPlayersInRoom", RpcTarget.All);
    }
}
