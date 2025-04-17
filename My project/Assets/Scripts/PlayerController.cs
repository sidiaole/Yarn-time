using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using FMOD.Studio;

[RequireComponent(typeof(Rigidbody2D),typeof(TouchingDirections))]
public class PlayerController : MonoBehaviour
{

    private EventInstance PlayerFootSteps;
    //

    public float walkSpeed = 5f;
    public float airwalkSpeed = 2f;
    public float runSpeed = 7f;
    public float jumpImpulse = 10f;


    Vector2 startPosition;

    Vector2 moveInput;

    TouchingDirections touchingDirections;

    public float CurrentMoveSpeed { get 
        {
            if (!touchingDirections.IsOnWall)
            {
                if (touchingDirections.IsGrounded)
                {
                    if (IsRunning)
                    {
                        return runSpeed;
                    }
                    else
                    {
                        return walkSpeed;
                    }
                }
                else
                {
                    return airwalkSpeed;
                }
                
            }
            else
            {
                return 0;
            }
        } 
    }

    [SerializeField]
    private bool _isMoving = false;
    [SerializeField]
    private bool _isRunning = false;

    private bool _isDead = false;

    public bool IsMoving { get 
        {
            return _isMoving;
        } 
        private set 
        { 
            _isMoving = value;
            animator.SetBool("_isMoving", value);
        } 
    }

    public bool IsRunning
    {
        get
        {
            return _isRunning;
        }
        private set
        {
            _isRunning = value;
            animator.SetBool("_isRunning", value);
        }
    }

    public bool IsDead
    {
        get { return _isDead; }
        private set
        {
            _isDead = value;
            animator.SetBool("_isDead", value); 
        }
    }

    public bool _isFacingRight = true;
    public bool IsFacingRight { get{ return _isFacingRight; } private set{
            if(_isFacingRight != value)
            {
                transform.localScale *= new Vector2(-1,1);
            }
            _isFacingRight = value;
        } }

    Rigidbody2D rb;
    Animator animator;
    

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        touchingDirections = GetComponent<TouchingDirections>();
        startPosition = transform.position;
    }

    // Start is called before the first frame update
    private void Start()
    {
        PlayerFootSteps = AudioManager.instance.CreateInstance(FmodEvents.instance.playerFootSteps);
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(moveInput.x * CurrentMoveSpeed, rb.linearVelocity.y);

        animator.SetFloat("yVelocity",rb.linearVelocity.y);

        UpdateSound();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput =context.ReadValue<Vector2>();
        IsMoving = moveInput != Vector2.zero;

        setFacingDirection(moveInput);
    }

    private void setFacingDirection(Vector2 moveInput)
    {
        if(moveInput.x > 0 && !IsFacingRight)
        {
            IsFacingRight = true;
        }
        else if(moveInput.x < 0 && IsFacingRight)
        {
            IsFacingRight = false;
        }
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            IsRunning = true;
        }
        else if (context.canceled)
        {
            IsRunning = false;
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started && touchingDirections.IsGrounded)
        {
            animator.SetTrigger("jump");
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpImpulse);
        }
    
    }

    public void Die()
    {
        if (IsDead) return; // �����ظ�����
        IsDead = true; // ��ǽ�ɫ������
        animator.Play("player_die"); // ֱ�Ӳ�����������

        rb.linearVelocity = Vector2.zero; // ����ֹͣ��ɫ�ƶ�
        //rb.isKinematic = true; // �ý�ɫ������ʱ��������Ӱ��
        gameObject.GetComponent<PlayerInput>().enabled = false;

        StartCoroutine(RespawnAfterDeath());

    }

    private IEnumerator RespawnAfterDeath()
    {
        // �ȴ����������������
        yield return new WaitForSeconds(1f);

        Respawn();
    }

    public void Respawn()
    {
        transform.position = startPosition;
        //rb.isKinematic = false;
        IsDead = false;
        gameObject.GetComponent<PlayerInput>().enabled = true;
    }

    private void UpdateSound()
    {
        //start footsteps event if the player has an x velocity and is on the ground
        if (rb.linearVelocity.x != 0 && touchingDirections.IsGrounded)
        {
            //get the playback state
            PLAYBACK_STATE playbackState;
            PlayerFootSteps.getPlaybackState(out playbackState);
            if (playbackState.Equals(PLAYBACK_STATE.STOPPED))
            {
                PlayerFootSteps.start();
            }
        }
        else
        {
            PlayerFootSteps.stop(STOP_MODE.ALLOWFADEOUT);
        }
    }
}
