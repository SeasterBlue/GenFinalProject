using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController2 : MonoBehaviour
{
    #region int
    int health = 50;
    #endregion

    #region floats
    [SerializeField] float currentSpeed;
    float rotationSpeed = 5.0f;
    float jumpForce = 6.0f;
    float groundDistance = 0.4f;
    #endregion

    #region bools
    private bool isGrounded;
    public bool isRunning = false;
    public bool oneArmChopped;
    public bool twoArmsChopped;
    public bool headChopped;
    public bool hasChainSaw;
    public bool hasMachete;
    #endregion

    #region weirdos
    LayerMask groundMask;
    Transform playerPickPoint;
    Transform leftArm;
    Transform gunPoint;
    Transform rightArm;
    Transform rightHand;
    Transform head;
    Transform plantHead;
    GameObject machete;
    public Seed seed;
    Rigidbody rb;
    Animator animator;
    GameManager gameManager;
    #endregion

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        groundMask = LayerMask.GetMask("Ground");
        playerPickPoint = GameObject.Find("PickPoint").GetComponent<Transform>();
        leftArm = GameObject.Find("Bone003").GetComponent<Transform>();
        machete = GameObject.Find("Machete").GetComponent<GameObject>();
        gunPoint = GameObject.Find("GunPoint").GetComponent<Transform>();
        rightArm = GameObject.Find("Bone032").GetComponent<Transform>();
        rightHand = GameObject.Find("Bone029").GetComponent<Transform>();
        head = GameObject.Find("Bone018").GetComponent<Transform>();
        plantHead = GameObject.Find("Bone019").GetComponent<Transform>();
        animator = GetComponent<Animator>();
        gameManager= FindObjectOfType<GameManager>().GetComponent<GameManager>();
        
    }

    private void Update()
    {
        CheckIfGrounded();
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            HandleJump();
        }


        if (Input.GetMouseButtonDown(0))
        {
            if(!hasChainSaw && !hasMachete) animator.SetTrigger("isHeadAttack");
            else animator.SetTrigger("isAttacking");

        }

       

        if (Input.GetKeyDown(KeyCode.L))
        {
            seed.RemoveSeedParent();
        }

        //if(Input.GetKeyDown(KeyCode.X))
        //{
        //    if (twoArmsChopped) CutHead();
        //    if (oneArmChopped) CutRightArm();
        //    CutLeftArm();
        //}
    }

    void FixedUpdate()
    {
        currentSpeed = Input.GetKey(KeyCode.LeftShift) ? 10.0f : 5.0f;
        isRunning = currentSpeed == 10.0f;

        HandleMovement(currentSpeed);
        
       
    }

    void Attack()
    {
        
    }

    void HandleMovement(float moveSpeed)
    {

        Vector2 inputVector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        bool isMoving = inputVector != Vector2.zero;

        animator.SetBool("isWalking", isMoving && !isRunning);
        animator.SetBool("isRunning", isRunning);

        //
        Vector3 moveDirection = new(inputVector.x, 0.0f, inputVector.y);
        float moveDistance = moveSpeed * Time.deltaTime;

        //Movement
        transform.position += moveDirection * moveDistance;
        transform.forward = Vector3.Slerp(transform.forward, moveDirection, Time.deltaTime * rotationSpeed);
    }

    void HandleJump()
    {

        if(isGrounded) rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    void CutLeftArm()
    {
        leftArm.localScale = new Vector3(0.0001f, 0.0001f, 0.0001f);
        oneArmChopped = true;
    }

    void CutRightArm()
    {
        rightArm.localScale = new Vector3(0.0001f, 0.0001f, 0.0001f);
        twoArmsChopped = true;
    }

    void CutHead()
    {
        head.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        plantHead.localScale = new Vector3(7, 7, 7);
        headChopped = true;
    }

    private void DestroySeed() //no usada aun
    {
        foreach (Transform child in playerPickPoint)
        {
            Seed seed = child.GetComponent<Seed>();
            if (seed != null)
            {
                seed.RemoveSeedParent();
                Destroy(seed.gameObject);
                break;
            }
        }
    }

    bool CheckIfGrounded()
    {
        isGrounded = Physics.CheckSphere(transform.position, groundDistance, groundMask);
        return isGrounded;
    }

    public bool HasSeed()
    {
        return seed != null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Machete") && !hasChainSaw)
        {
            Vector3 offsetMachete = new Vector3(-0.086f, 0.21f, -0.005f);
            Quaternion offsetMacheteRotation = Quaternion.Euler(100, -57, 292);
            other.transform.parent = GetMacheteNewTransform();
            other.transform.localPosition = offsetMachete;
            other.transform.localRotation = offsetMacheteRotation;
            hasMachete = true;
        }
        else if (other.CompareTag("Machete") && hasChainSaw)
        {
            other.transform.parent = GetGunPointTransform();
            other.transform.localPosition = Vector3.zero;
            other.transform.localRotation = Quaternion.Euler(0,90,0);
            other.transform.localScale = other.transform.localScale - new Vector3(0.5f, 0.5f, 0.5f);
            hasMachete = true;
        }

        if (other.CompareTag("Chainsaw") && !hasMachete)
        {
            Quaternion offsetChainSawRotation = Quaternion.Euler(45, -100, 110);
            other.transform.parent = GetGunsawNewTransform();
            other.transform.localPosition = Vector3.zero;
            other.transform.localRotation = offsetChainSawRotation; 
            hasChainSaw = true;
        }
        else if (other.CompareTag("Chainsaw") && hasMachete)
        {
            Quaternion offsetChainSawRotation = Quaternion.Euler(45, -100, 110);
            other.transform.parent = GetGunsawNewTransform();
            other.transform.localPosition = Vector3.zero;
            other.transform.localRotation = offsetChainSawRotation;
            hasChainSaw = true;

            machete.transform.parent = GetGunPointTransform();
            machete.transform.localPosition = Vector3.zero;
            machete.transform.localRotation = Quaternion.Euler(0, 90, 0);
            machete.transform.localScale = other.transform.localScale - new Vector3(0.5f, 0.5f, 0.5f);
            hasMachete = true;



        }








    }


    public Transform GetSeedNewTransform()
    {
        return playerPickPoint;
    }

    public Transform GetMacheteNewTransform()
    {
        return rightHand;
    }
    public Transform GetGunsawNewTransform()
    {
        return rightHand;
    }

    public Transform GetGunPointTransform()
    {
        return gunPoint;
    }


    public void RecieveDamage(int damage)
    {
        health-=damage;

        // CHECK LATER FOR ODD BEHAVIORS
        if (health < 35) CutLeftArm();
        if (health < 20 && oneArmChopped) CutRightArm();
        if (health <= 0 && twoArmsChopped)
        {
            CutHead();
            gameManager.OnGameOver();
        }
    }
}
