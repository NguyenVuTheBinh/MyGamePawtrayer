using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.Rendering.Universal;
using Photon.Pun;

public class PlayerController : MonoBehaviour, IPunObservable
{
    //To control character want to control
    //[SerializeField] bool hasControl;
    public static PlayerController localPlayer;

    //To animation
    [SerializeField] Animator animator;
    Transform myAvatar;

    float direction = 1;
    //To move
    [SerializeField] InputAction movement;
    Vector3 movementInput;
    [SerializeField] float speed;

    //To set character color
    static Color dogColor;
    SpriteRenderer dogSprite;
    PlayerSetColor initialColor;

    //To related role
    [SerializeField] bool isCat;
    [SerializeField] InputAction BONK;

    List<PlayerController> targets;
    [SerializeField] InputAction reportBody;
    bool isDead;

    //lighting
    //[SerializeField] FieldOfView fov;
    Light2D vision;

    //found and report body
    [SerializeField] GameObject deadBody;
    public static List<Transform> allBodies;
    List<Transform> bodiesFound;
    [SerializeField] private LayerMask ignoreForBody;

    //Interact object
    [SerializeField] InputAction mouse;
    Interactive interactiveObject;
    Camera myCam;
    [SerializeField] InputAction interaction;
    [SerializeField] LayerMask interactLayer;

    PhotonView myPov;


    //debug
    //public GameObject ShowRay;

    private void Awake()
    {
        BONK.performed += BonkTargets;
        reportBody.performed += ReportBody;
        interaction.performed += Interaction;
    }
    private void OnEnable()
    {
        movement.Enable();
        BONK.Enable();
        reportBody.Enable();
        mouse.Enable();
        interaction.Enable();
    }
    private void OnDisable()
    {
        movement.Disable();
        BONK.Disable();
        reportBody.Disable();
        mouse.Disable();
        interaction.Disable();
    }
    private void Start()
    {
        myPov = GetComponent<PhotonView>();
        if (myPov.IsMine) 
        {
            localPlayer = this;
        }
        allBodies = new List<Transform>();
        myAvatar = gameObject.GetComponent<Transform>();
        targets = new List<PlayerController>();
        dogSprite = gameObject.GetComponent<SpriteRenderer>();
        if (initialColor.allColors == null)
        {
            Debug.Log("you're right");
            return;
        }
        else
        {
            dogColor = initialColor.allColors[Random.Range(0, 8)];
        }
        dogSprite.color = dogColor;
        vision = gameObject.GetComponent<Light2D>();
        myCam = transform.GetChild(2).GetComponent<Camera>();
        bodiesFound = new List<Transform>();
        vision.enabled = true;
        interactiveObject = GameObject.FindWithTag("InteractiveObject").GetComponent<Interactive>();
        if (!myPov.IsMine)
        {
            myCam.gameObject.SetActive(false);
            vision.enabled = false;
            return;
        }
        
    }
    private void Update()
    {
        myAvatar.localScale = new Vector3(direction, 1, 1);
        if (!myPov.IsMine)
            return;

        //fov.GetOrigin(transform.position);
        movementInput = movement.ReadValue<Vector3>();

        if (movementInput.x != 0)
        {
            direction = Mathf.Sign(movementInput.x);
        }
        if (movementInput != Vector3.zero)
        {
            animator.SetBool("isRunning", true);
        }
        else
        {
            animator.SetBool("isRunning", false);
        }
        if (allBodies.Count > 0)
        {
            SearchBody();
        }
        
    }
    private void FixedUpdate()
    {
        if (!myPov.IsMine)
            return;
        this.transform.Translate(movementInput * speed * Time.deltaTime);
    }

    //Set and sync color
    public void SetColor(Color newColor)
    {
        if (!myPov)
            return;
        if (dogSprite != null){
            myPov.RPC("RPC_SetColor", RpcTarget.All, newColor);
        }
    }
    [PunRPC]
    void RPC_SetColor(Color newColor)
    {
        dogColor = newColor;
        dogSprite.color = dogColor;
    }


    //The part of doing kill method 
    public void SetRole(bool newRole)
    {
        isCat = newRole;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {            
            if (this.isCat)
            {
                PlayerController tempTarget = other.GetComponent<PlayerController>();
                if (tempTarget.isCat)
                    return;
                else
                {
                    targets.Add(tempTarget);
                }
            }
        }
        if (other.tag == "InteractiveObject")
        {
            if (!myPov.IsMine)
                return;
            interactiveObject.CloseToInteract();
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            PlayerController tempTarget = other.GetComponent<PlayerController>();
            if (targets.Contains(tempTarget))
                targets.Remove(tempTarget);
        }
        if (other.tag == "InteractiveObject")
        {
            if (!myPov.IsMine)
                return;
            interactiveObject.FarToInteract();
        }
    }
    void BonkTargets(InputAction.CallbackContext context)
    {
        if (!myPov.IsMine)
            return;
        if (!isCat)
            return;
        if (isDead)
            return;
        if (context.phase == InputActionPhase.Performed)
        {
            if (targets.Count == 0)
                return;
            else
            {
                if (targets[targets.Count - 1].isDead)
                    return;
                transform.position = targets[targets.Count - 1].transform.position;
                targets[targets.Count - 1].myPov.RPC("RPC_DieByBONK", RpcTarget.All);
                targets.RemoveAt(targets.Count - 1);
            }
        }
    }

    [PunRPC]
    void RPC_DieByBONK()
    {
        if (!myPov.IsMine)
            return;
        Body tempBody = PhotonNetwork.Instantiate(deadBody.name, transform.position + new Vector3(1f, -2f, direction), transform.rotation).GetComponent<Body>();

        isDead = true;
        animator.SetBool("isDead", isDead);
        tempBody.SetColor(dogSprite.color);
    }

    void SearchBody()
    {
        if (!myPov.IsMine)
            return;
        if (isDead)
            return;
        foreach (Transform body in allBodies)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, body.position, 1000f, ~ignoreForBody);
            Debug.DrawRay(transform.position, body.position - transform.position, Color.red);
            if (hit.transform == body)
            {
                if (bodiesFound.Contains(body.transform))
                    return;
                bodiesFound.Add(body.transform);
            } else
            {
                bodiesFound.Remove(body.transform);
            }
        }
    }
    private void ReportBody(InputAction.CallbackContext obj)
    {
        if (!myPov.IsMine)
            return;
        if (bodiesFound == null)
            return;
        myPov.RPC("RPC_Report", RpcTarget.All);
    }
    [PunRPC]
    void RPC_Report()
    {
        if (bodiesFound.Count > 0)
        {
            Transform tempBody = bodiesFound[bodiesFound.Count - 1];
            bodiesFound.Remove(tempBody);
            tempBody.GetComponent<Body>().Report();
        }
    }

    private void Interaction(InputAction.CallbackContext obj)
    {
        //if (obj.phase == InputActionPhase.Performed)
        //{
        //    Debug.Log("you clicked");
        //    Debug.Log(mousePositionInput.x);
        //    Debug.Log(mousePositionInput.y);
        //    RaycastHit2D hitInteract = Physics2D.Raycast(transform.position, mousePositionInput, interactLayer);
        //    Debug.DrawRay(transform.position, mousePositionInput, Color.blue, 20f);
        //    if (hitInteract)
        //    {
        //        if (hitInteract.transform.tag == "InteractiveObject")
        //        {
        //            if (!hitInteract.transform.GetChild(0).gameObject.activeInHierarchy)
        //                return;
        //            Interactive temp = GetComponent<Interactive>();
        //            temp.PlayMiniGame();
        //        }
        //    }
        //}
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(direction);
            stream.SendNext(isCat);
        }
        else
        {
            direction = (float)stream.ReceiveNext();
            isCat = (bool)stream.ReceiveNext();
        }
    }
    public void BecomeCat(int catNumber)
    {
        if (PhotonNetwork.LocalPlayer == PhotonNetwork.PlayerList[catNumber])
            isCat = true;
    }
}