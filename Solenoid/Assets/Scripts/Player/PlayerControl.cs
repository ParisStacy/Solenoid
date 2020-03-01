using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour {
    
    //Public Variables//
    [Header("Movement")]
    public float walkSpeed;
    public float sneakSpeed;
    public float runSpeed;
    public float jumpForce;
    [Header("Physics")]
    public float gravity;

    //State Enum//
    private enum PlayerStates { defaultMove, hanging, climbing };
    private PlayerStates _currentState;

    //Components//
    private Rigidbody rb;

    //Values//
    private float _speed, _xMove, _zMove;
    private bool _grounded, _canUncrouch, _jumping, _crouching;
    private Vector3 _moveDirection;

    void Awake() {
        rb = this.GetComponent<Rigidbody>();
        _currentState = PlayerStates.defaultMove;
    }

    void Update() {
        //Inputs//
        _xMove = Input.GetAxis("Horizontal");
        _zMove = Input.GetAxis("Vertical");

        _jumping = Input.GetButton("Jump");

        //Condition Checks//
        RaycastHit hit;
        _grounded = (Physics.Raycast(transform.position, Vector3.down, out hit, 1.1f));
        _canUncrouch = !(Physics.Raycast(transform.position, Vector3.up, out hit, 1.5f));

    }

    void FixedUpdate() {
        //Decide which state to run
        switch (_currentState) {
            case PlayerStates.defaultMove:
                defaultMoveUpdate();
                break;
            case PlayerStates.hanging:
                hangingUpdate();
                break;
            case PlayerStates.climbing:
                climbingUpdate();
                break;
        }
    }

    void defaultMoveUpdate() {

        //Determine Speed
        if (_xMove != 0 || _zMove != 0) {

            float targetSpeed;

            targetSpeed = walkSpeed;

            _speed = Mathf.Lerp(_speed,targetSpeed,.5f);
        }

        //Movement//

        //Add directional movement
        float yDir = _moveDirection.y;

        _moveDirection = (_xMove * transform.right + _zMove * transform.forward).normalized;  //Oreintate based on direction
        _moveDirection *= _speed;

        //Check for gravity and jump
        if (!_grounded) {
            yDir -= gravity * Time.deltaTime;
        } else {
            yDir = 0;
            if (_jumping) yDir = jumpForce;
        }

        _moveDirection.y = yDir;

        //Apply direction, if no direction then decay
        if (_moveDirection != Vector3.zero) {
            rb.velocity = (_moveDirection);
        } else {
            rb.velocity *= .6f;
        }

    }

    void hangingUpdate() {

    }

    void climbingUpdate() {

    }
}
