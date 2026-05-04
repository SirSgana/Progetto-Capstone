using System.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;

//Definiamo gli stati fuori dalla classe per renderli accessibili da altri script (tipo l'animator)
public enum PlayerState { Idle, Walking, Running, Sprinting, Jumping, Falling, DoubleJumping, Equipping }

[RequireComponent(typeof(CharacterController))]

public class PlayerController_FSM : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float runSpeed = 6f;
    [SerializeField] private float sprintSpeed = 12f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float turnSmoothTime = 0.1f;

    [Header("Jump Settings")]
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float gravity = -14f;
    [SerializeField] private float doubleJumpHeight = 2f;
    [SerializeField] private float fallMultiplier = 2.5f;

    [Header("Weapon Settings")]
    [SerializeField] private bool isSwordEquipped = false;
    private static readonly int IsEquippedHash = Animator.StringToHash("IsSwordEquipped");

    [Header("Sword Anim")]
    [SerializeField] private Animator swordAnim;
    private static readonly int swordOpenHash = Animator.StringToHash("isEquipped");

    [Header("Rigging")]
    [SerializeField] private Rig armRig;
    [SerializeField] private float lerpSpeed = 10f;

    //Stati ed Input
    private PlayerState currentState;
    private CharacterController controller;
    private GameInput inputActions;

    //Variabili di calcolo
    private Vector3 rawInput;
    private Vector3 smoothMoveVelocity;
    private Vector3 verticalVelocity;
    private float turnSmoothVelocity;
    private int jumpCount = 0;
    private bool isSprintHeld;

    //Variabili per l'attacco
    private bool comboRequested2 = false;
    private bool comboRequested = false;
    private bool isAttacking = false;

    private Animator anim;
    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int GroundedHash = Animator.StringToHash("isGrounded");
    private static readonly int IsFirstAttackHash = Animator.StringToHash("IsFirstAttack");
    private static readonly int IsSecondAttackHash = Animator.StringToHash("IsSecondAttack");
    private static readonly int IsThirdAttackHash = Animator.StringToHash("IsThirdAttack");

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        inputActions = new GameInput();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {

        if (DialogueManager.GetInstance()?.dialogueIsPlaying == true)
        {
            return;
        }

        ReadInput();
        UpdateState();
        ApplyRotation();
        ApplyMovement();
        ApplyGravity();
        UpdateAnimator();
        HandleWeaponRig();
        UpdateAttack();
    }
    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Jump.performed += OnJumpPerformed;

        //Attacco 
        inputActions.Player.Attack.performed += OnAttackPerformed;

        //Equippaggiamento della spada
        inputActions.Player.Equip.performed += ctx => ToggleEquip();

        //Sprint del player
        inputActions.Player.Sprint.started += ctx => isSprintHeld = true;
        inputActions.Player.Sprint.canceled += ctx => isSprintHeld = false;
    }

    private void HandleWeaponRig()
    {
        if (armRig != null)
        {
            // Controlliamo se la spada è attiva ED se ci stiamo muovendo
            // 'rawInput.magnitude' misura quanto è veloce il movimento
            float targetWeight = (isSwordEquipped && rawInput.magnitude > 0.1f) ? 1f : 0f;

            armRig.weight = Mathf.Lerp(armRig.weight, targetWeight, lerpSpeed * Time.deltaTime);
        }
    }
    private void ReadInput()
    {
        Vector2 input = inputActions.Player.Move.ReadValue<Vector2>();
        rawInput = new Vector3(input.x, 0, input.y);
    }

    private void UpdateState()
    {
        if (controller.isGrounded)
        {
            jumpCount = 0;

            if (rawInput.magnitude < 0.1f)
                currentState = PlayerState.Idle;
            else if (isSprintHeld)
                currentState = PlayerState.Sprinting;
            else if (rawInput.magnitude > 0.6f)
                currentState = PlayerState.Running;
            else
                currentState = PlayerState.Walking;
        }
        else
        {
            if (verticalVelocity.y > 0)
                currentState = (jumpCount > 1) ? PlayerState.DoubleJumping : PlayerState.Jumping;
            else
                currentState = PlayerState.Falling;
        }
    }

    private void ToggleEquip()
    {
        isSwordEquipped = !isSwordEquipped;
        anim.SetBool(IsEquippedHash, isSwordEquipped);

        if (swordAnim != null)
        {
            swordAnim.SetBool(swordOpenHash, isSwordEquipped);
        }
    }
    private void ApplyRotation()
    {
        if (rawInput.magnitude >= 0.1f)
        {
            //Calcolo angolo basato sulla telecamera
            float targetAngle = Mathf.Atan2(rawInput.x, rawInput.z) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
        }
    }

    private void ApplyMovement()
    {
        float targetSpeed = 0f;

        //Blocco il movimento durante l'attacco
        if (isAttacking)
        {
            smoothMoveVelocity = Vector3.zero;
            return;
        }
        //Determiniamo la velocità in base allo stato attuale
        switch (currentState)
        {
            case PlayerState.Idle: targetSpeed = 0f; break;
            case PlayerState.Walking: targetSpeed = walkSpeed; break;
            case PlayerState.Running: targetSpeed = runSpeed; break;
            case PlayerState.Sprinting: targetSpeed = sprintSpeed; break;
            case PlayerState.Jumping:
            case PlayerState.DoubleJumping:
            case PlayerState.Falling:
                targetSpeed = isSprintHeld ? sprintSpeed : runSpeed;
                break;
        }

        //Calcolo la direzione di movimento direttamente dall'input e dalla camera
        //Non uso il transform.forward del player perchè potrebbe trovarsi a metà di una rotazione
        Vector2 input = inputActions.Player.Move.ReadValue<Vector2>();
        Vector3 inputDir = new Vector3(input.x, 0, input.y).normalized;

        Vector3 moveDir = Vector3.zero;

        if (inputDir.magnitude >= 0.1f)
        {
            //Calcolo l'angolo basato sulla camera 
            float targetAngle = Mathf.Atan2(inputDir.x, inputDir.z) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
            //Creiamo il vettore di movimento basato su quell'angolo
            moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
        }

        ////Calcolo direzione di movimento relativa alla telecamera
        //Vector3 moveDir = Quaternion.Euler(0f, transform.eulerAngles.y, 0f) * Vector3.forward;

        //float currentTarget = (rawInput.magnitude > 0.1f) ? targetSpeed : 0f; QUESTA è UN AGGIUNTA PER L'AGGIUSTAMENTO AUTOMATICO DELLA CAMERA VEDRò SE LASCIARLA

        //Fluidità di movimento con il lerp (evito stop bruschi)
        smoothMoveVelocity = Vector3.Lerp(smoothMoveVelocity, moveDir * (targetSpeed), acceleration * Time.deltaTime);

        //Con questo if fixo il fatto che la speed scende a numeri minuscoli
        if (smoothMoveVelocity.magnitude < 0.01f)
            smoothMoveVelocity = Vector3.zero;

        controller.Move(smoothMoveVelocity * Time.deltaTime);
    }

    private void ApplyGravity()
    {
        if (controller.isGrounded && verticalVelocity.y < 0)
        {
            verticalVelocity.y = -2f;
        }

        //Se cade, la gravità è più forte
        float currentGravity = (verticalVelocity.y < 0) ? gravity * fallMultiplier : gravity;
        verticalVelocity.y += currentGravity * Time.deltaTime;
        controller.Move(verticalVelocity * Time.deltaTime);
    }

    private void OnAttackPerformed(InputAction.CallbackContext context)
    {
        if (!isSwordEquipped || !controller.isGrounded) return;

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("First Attack"))
        {
            AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);
            if (state.normalizedTime >= 0.8f)
                anim.SetTrigger(IsSecondAttackHash);
            else
                comboRequested = true;
            return;
        }

        if (!isAttacking)
        {
            isAttacking = true;
            comboRequested = false;
            anim.SetTrigger(IsFirstAttackHash);
        }
    }

    private void UpdateAttack()
    {
        AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);

        if (comboRequested && state.IsName("First Attack") && state.normalizedTime >= 0.8f)
        {
            comboRequested = false;
            anim.SetTrigger(IsSecondAttackHash);
            return;
        }

        bool inFirstAttack = state.IsName("First Attack");
        bool inSecondAttack = state.IsName("Second Attack");

        if (inFirstAttack && !comboRequested && state.normalizedTime >= 0.95f)
            isAttacking = false;

        if (inSecondAttack && state.normalizedTime >= 0.95f)
        {
            isAttacking = false;
            comboRequested = false;
        }

        // Fallback
        if (!inFirstAttack && !inSecondAttack && isAttacking)
            isAttacking = false;
    }


    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        //Logica Double Jump
        if (controller.isGrounded)
        {
            jumpCount = 1;
            verticalVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
        else if (jumpCount == 1)
        {
            jumpCount = 2;
            verticalVelocity.y = Mathf.Sqrt(doubleJumpHeight * -2f * gravity);
        }
    }

    private void UpdateAnimator()
    {
        //Se non abbiamo la ref all'animator lo cerchiamo nel child
        if (anim == null) anim = GetComponentInChildren<Animator>();

        Vector3 horizontalSpeedVec = new Vector3(smoothMoveVelocity.x, 0, smoothMoveVelocity.z);
        float speedForAnimator = horizontalSpeedVec.magnitude;

        //Taglio netto della speed
        if (speedForAnimator < 0.01f) speedForAnimator = 0f;

        //Logica di invio all'animator
        if (speedForAnimator > 0)
        {
            //Se mi muovo uso il Damping (0.1f) per rendere la transizione fluida
            anim.SetFloat(SpeedHash, speedForAnimator, 0.1f, Time.deltaTime);
        }
        else
        {
            //Se sono fermo, Forzo lo zero
            anim.SetFloat(SpeedHash, 0f);
        }

        ////Comunico con l'animator (imposto 0.1f per smoothare il cambio delle animazioni
        //anim.SetFloat(SpeedHash, speedForAnimator, 0.1f, Time.deltaTime);
        anim.SetBool(GroundedHash, controller.isGrounded);
    }

    private void OnDisable()
    {
        inputActions.Player.Attack.performed -= OnAttackPerformed;
        inputActions.Player.Disable();
    }

    public PlayerState CurrentState => currentState;

}
