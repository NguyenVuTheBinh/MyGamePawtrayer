using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;

public class PlayerController : MonoBehaviour
{
    //Control character to control
    [SerializeField] bool hasControl;
    public static PlayerController localPlayer;

    //Animation
    [SerializeField] Animator animator;

    //To move
    [SerializeField] InputAction movement;
    Vector3 movementInput;
    [SerializeField] float speed;

    //Set character color
    static Color dogColor;
    SpriteRenderer dogSprite;

    private void Awake()
    {

    }
    private void OnEnable()
    {
        movement.Enable();
    }
    private void OnDisable()
    {
        movement.Disable();
    }
    private void Start()
    {
        if (hasControl) 
        {
            localPlayer = this;
        }
        dogSprite = gameObject.GetComponent<SpriteRenderer>();
        if (dogSprite.color == Color.clear)
            dogColor = Color.white;
        dogSprite.color = dogColor;

        
    }
    private void Update()
    {
        movementInput = movement.ReadValue<Vector3>();

        if (movementInput.x != 0)
        {
            
             transform.localScale = new Vector3(Mathf.Sign(movementInput.x), 1, 1);
           
        }
        if (movementInput != Vector3.zero)
        {
            animator.SetBool("isRunning", true);
        }
        else
        {
            animator.SetBool("isRunning", false);
        }
    }
    private void FixedUpdate()
    {
        this.transform.Translate(movementInput * speed * Time.deltaTime);
    }

    public void SetColor(Color newColor)
    {
        dogColor = newColor;
        if (dogSprite != null)
            dogSprite.color = dogColor;
    }
}