using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerV2 : MonoBehaviour
{

    //SUMMARY: This is a depricated remake of the character controller, designed to fix some physics issues. 


    //Game Object Required Components
    private Rigidbody rb;

    //Configurable Variables
    [SerializeField]
    private float walkSpeed, sprintSpeed, crouchSpeed, jumpForce;

    //Internal Variables
    private float _xMove, _zMove, _currentSpeed;
    private bool _sprinting, _crouching, _grounded, _jumping, _jumpPrepped;
    private Vector3 _moveDirection;
    private enum PlayerState {defaultMove, mantle, slide};
    private PlayerState _currentState;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        _currentState = PlayerState.defaultMove;
    }

    void Update()
    {

        // Inputs
        _xMove = Input.GetAxis("Horizontal");
        _zMove = Input.GetAxis("Vertical");

        _sprinting = Input.GetButton("Sprint");

        checkJump();
    }

    void FixedUpdate() {

        //Determine Behavior
        switch(_currentState) {
            case PlayerState.defaultMove:
                defaultMove();
                break;
        }
    }

    private void defaultMove() {

        _moveDirection = new Vector3(_xMove, 0, _zMove).normalized * currentSpeed;

        rb.MovePosition(transform.position + (_moveDirection * Time.fixedDeltaTime));

        if (_jumping) {
            rb.AddForce(new Vector3(0, jumpForce, 0), ForceMode.Impulse);
            _jumping = false;
        }
        rb.AddForce(Physics.gravity * rb.mass);
    }




    //SUMMARY: When jump is held down, player with crouch slightly, in preperation. Jump on release
    private void checkJump() {
        if (Input.GetButtonDown("Jump")) {
            _jumpPrepped = true;
        }
        if (Input.GetButtonUp("Jump")) {
            if (_jumpPrepped) {
                _jumping = true;
                _jumpPrepped = false;
            }
        }
    }
    //SUMMARY: Determine speed at which player should be travelling, and lerp towards it.
    private float currentSpeed {
        get {

            float targetSpeed = 0;
 
            if (_xMove != 0 || _zMove != 0) {
                if (_sprinting) {
                    targetSpeed = sprintSpeed;
                } else if (_crouching) {
                    targetSpeed = crouchSpeed;
                } else {
                    targetSpeed = walkSpeed;
                }
            } else {
                targetSpeed = 0f;
            }

            _currentSpeed = Mathf.Lerp(_currentSpeed, targetSpeed, .08f);

            return _currentSpeed;
        }
    }


}
