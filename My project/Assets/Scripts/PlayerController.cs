using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.Rendering.Universal;
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;

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
    Rigidbody2D myRb;
    Vector3 movementInput;
    [SerializeField] float speed;

    //To set character color
    private Color dogColor = Color.white;
    SpriteRenderer dogSprite;

    //To related role
    [SerializeField] bool isCat;
    [SerializeField] InputAction BONK;

    List<PlayerController> targets;
    [SerializeField] InputAction reportBody;
    public bool isDead = false;
    Follow_player followCamera;

    //lighting
    //[SerializeField] FieldOfView fov;
    Light2D vision;

    //found and report body
    [SerializeField] GameObject deadBody;
    public static List<Transform> allBodies;
    List<Transform> bodiesFound;
    [SerializeField] private LayerMask ignoreForBody;
    private int myVote = -1;
    bool hasVoted;

    //Interact object
    [SerializeField] InputAction mouse;
    Interactive interactiveObject;
    Camera myCam;
    [SerializeField] InputAction interaction;
    [SerializeField] LayerMask interactLayer;

    public PhotonView myPov;


    //debug
    //public GameObject ShowRay;

    private void Awake()
    {
        myPov = GetComponent<PhotonView>();
        if (myPov.IsMine)
        {
            localPlayer = this;
        }
        dogSprite = gameObject.GetComponent<SpriteRenderer>();
        var owner = myPov.Owner;
        if (owner.CustomProperties.TryGetValue("colorR", out object rObj) &&
            owner.CustomProperties.TryGetValue("colorG", out object gObj) &&
            owner.CustomProperties.TryGetValue("colorB", out object bObj) &&
            owner.CustomProperties.TryGetValue("colorA", out object aObj))
        {
            dogColor = new Color32((byte)rObj, (byte)gObj, (byte)bObj, (byte)aObj);
        }
        else
        {
            dogColor = Color.white;
        }
        dogSprite.color = dogColor;
        BONK.performed += BonkTargets;
        reportBody.performed += ReportBody;
        interaction.performed += Interaction;
    }
    public void EnableInputActions()
    {
        movement.Enable();
        BONK.Enable();
        reportBody.Enable();
        mouse.Enable();
        interaction.Enable();
    }
    public void DisableInputActions()
    {
        movement.Disable();
        BONK.Disable();
        reportBody.Disable();
        mouse.Disable();
        interaction.Disable();
    }
    private void OnEnable()
    {
        EnableInputActions();
    }

    private void OnDisable()
    {
        DisableInputActions();
    }
    private void Start()
    {
        dogSprite.color = dogColor;
        allBodies = new List<Transform>();
        myAvatar = gameObject.GetComponent<Transform>();
        targets = new List<PlayerController>();
        vision = gameObject.GetComponent<Light2D>();
        myRb = gameObject.GetComponent<Rigidbody2D>();
        myCam = transform.GetChild(2).GetComponent<Camera>();
        bodiesFound = new List<Transform>();
        vision.enabled = true;
        interactiveObject = GameObject.FindWithTag("InteractiveObject").GetComponent<Interactive>();
        followCamera = FindAnyObjectByType<Follow_player>();
        if (followCamera != null)
        {
            followCamera.UpdateCullingMask(isDead);
        }
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
        Vector2 movementInput = movement.ReadValue<Vector3>();
        Vector3 targetPosition = myRb.position + movementInput * speed * Time.fixedDeltaTime;
        myRb.MovePosition(targetPosition);
    }

    //Set and sync color
    public void SetColor(Color newColor)
    {
        Debug.Log("I'm called");
        if (myPov){
            dogColor = newColor;
            dogSprite.color = dogColor;
        }
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
                if (tempTarget.isCat | tempTarget.isDead)
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
        object[] instData = new object[] {
            (float)dogColor.r / 255f,
            (float)dogColor.g / 255f,
            (float)dogColor.b / 255f,
            (float)dogColor.a / 255f
        };
        Body tempBody = PhotonNetwork.Instantiate(
            "DedDog", 
            transform.position + new Vector3(1f, -2f, direction),
            transform.rotation,
            0,
            instData
        ).GetComponent<Body>();
        PhotonView bodyView = tempBody.GetComponent<PhotonView>();
        if (!bodyView.IsMine || PhotonNetwork.LocalPlayer.ActorNumber != PhotonNetwork.MasterClient.ActorNumber)
            bodyView.TransferOwnership(PhotonNetwork.MasterClient.ActorNumber);
        PlayerController bonkedTarget = GetComponent<PlayerController>();
        bonkedTarget.isDead = true;
        followCamera.UpdateCullingMask(isDead);
        bonkedTarget.myPov.RPC("RPC_SetDeadLayer", RpcTarget.All);

        animator.SetBool("isDead", true);
        tempBody.myPov.RPC("RPC_SetColorDeadBody", RpcTarget.All,
            (float)dogColor.r,
            (float)dogColor.g,
            (float)dogColor.b,
            (float)dogColor.a
        );
    }

    [PunRPC]
    void RPC_SetDeadLayer()
    {
        SetLayerRecursive(gameObject, LayerMask.NameToLayer("DeadPlayerLayer"));
    }
    void SetLayerRecursive(GameObject obj, int newLayer)
    {
        if (obj == null) 
            return;
        obj.layer = newLayer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursive(child.gameObject, newLayer);
        }
    }

    //for report
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
        if (bodiesFound == null || bodiesFound.Count == 0)
            return;
        Transform tempBody = bodiesFound[bodiesFound.Count - 1];
        PhotonView bodyView = tempBody.GetComponent<PhotonView>();
        myPov.RPC("RPC_ReportToMaster", RpcTarget.MasterClient, bodyView.ViewID);
        Debug.Log("report in Player Controller");
        myPov.RPC("RPC_ClearBodies", RpcTarget.All);
        CallMeeting();
    }
    [PunRPC]
    void RPC_ClearBodies()
    {
        bodiesFound.Clear();
        allBodies.Clear();
    }
    [PunRPC]
    void RPC_ReportToMaster(int bodyViewID)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        PhotonView bodyView = PhotonView.Find(bodyViewID);
        if (bodyView != null)
        {
            PhotonNetwork.Destroy(bodyView.gameObject);
        }
    }

    //meeting and voting
    public void CallMeeting()
    {
        if (myPov.IsMine)
        {
            InGameController.Instance.myPov.RPC("RPC_Meeting", RpcTarget.All);
        }
    }
    public void OnPlayerVote(int votedPlayerId)
    {
        if (hasVoted)
        {
            Debug.LogWarning("You have already voted!");
            return;
        }
        myVote = votedPlayerId;
        if (myPov != null)
            InGameController.Instance.myPov.RPC("RPC_SubmitVote", RpcTarget.MasterClient, votedPlayerId, PhotonNetwork.LocalPlayer.ActorNumber);
        else
            Debug.LogError("PhotonView reference is missing!");
        Debug.Log($"Player {PhotonNetwork.LocalPlayer.ActorNumber} voted for {votedPlayerId}");
        hasVoted = true;

    }
    [PunRPC]
    public void RPC_ResetVote()
    {
        hasVoted = false;
        myVote = -1;
    }
    [PunRPC]
    void RPC_DieByVote()
    {
        if (!myPov.IsMine)
            return;
        PlayerController votedTarget = GetComponent<PlayerController>();
        votedTarget.isDead = true;
        followCamera.UpdateCullingMask(isDead);
        votedTarget.myPov.RPC("RPC_SetDeadLayer", RpcTarget.All);
        animator.SetBool("isDead", true);
    }

    //do nothing currently
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
    
    //sync data stream
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //show direction
            stream.SendNext(direction);
            //set role
            stream.SendNext(isCat);
            //set status 
            stream.SendNext(isDead);
            //set color
            stream.SendNext(dogColor.r);
            stream.SendNext(dogColor.g);
            stream.SendNext(dogColor.b);
            stream.SendNext(dogColor.a);
        }
        else
        {
            //receive direction
            direction = (float)stream.ReceiveNext();
            //receive role
            isCat = (bool)stream.ReceiveNext();
            //receive status
            isDead = (bool)stream.ReceiveNext();
            //receive color
            float r = (float)stream.ReceiveNext();
            float g = (float)stream.ReceiveNext();
            float b = (float)stream.ReceiveNext();
            float a = (float)stream.ReceiveNext();
            dogColor = new Color(r, g, b, a);
            if (dogSprite != null)
                dogSprite.color = dogColor;
        }
    }
    public void BecomeCat(int catNumber)
    {
        if (PhotonNetwork.LocalPlayer == PhotonNetwork.PlayerList[catNumber])
            isCat = true;
    }
}