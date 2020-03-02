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
    public float slopeLimit;
    public float groundedRay = 1.1f;

    //State Enum//
    private enum PlayerStates { defaultMove, hanging, climbing };
    private PlayerStates _currentState;

    //Components//
    private Rigidbody rb;
    private CapsuleCollider cc;

    //Values//
    private float _speed, _xMove, _zMove, _lastYpos;
    private bool _grounded, _jumping, _crouching, _canUncrouch, _falling;
    private Vector3 _moveDirection;
    private RaycastHit slopeHit;

    [HideInInspector]
    public Vector3 cameraTarget;
    [HideInInspector]
    public bool isMoving, isCrouching;


    void Awake() {
        rb = this.GetComponent<Rigidbody>();
        cc = this.GetComponent<CapsuleCollider>();
        _currentState = PlayerStates.defaultMove;
    }

    void Update() {
        //Inputs//
        _xMove = Input.GetAxis("Horizontal");
        _zMove = Input.GetAxis("Vertical");

        _jumping = Input.GetButtonUp("Jump");

        if (Input.GetButtonDown("Crouch")) {
            if (_crouching) {
                if (_canUncrouch) _crouching = false;
            } else {
                _crouching = true;
            }
        }

        //Condition Checks//
        RaycastHit hit;
        _grounded = (Physics.Raycast(transform.position, Vector3.down, out hit, groundedRay));
        _canUncrouch = !(Physics.SphereCast(transform.position, .45f, Vector3.up, out hit, 1f));


        //CamAnimate
        isMoving = ((_xMove != 0 || _zMove != 0) && _grounded);
        isCrouching = _crouching;

        cameraTarget = transform.position;
        cameraTarget.y += _cameraTargetOffset;

    }

    void FixedUpdate() {

        checkFalling();

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

            if (_crouching) {
                targetSpeed = sneakSpeed;
            } else {
                targetSpeed = walkSpeed;
            }


            _speed = Mathf.Lerp(_speed, targetSpeed, .5f);
        }

        //Crouch
        if (_crouching) {
            cc.height = 1;
            groundedRay = .6f;
        } else {
            cc.height = 2;
            groundedRay = 1.1f;
        }


        //Movement//

        //Add directional movement
        float yDir = _moveDirection.y;

        _moveDirection = (_xMove * transform.right + _zMove * transform.forward).normalized;  //Oreintate based on direction
        _moveDirection *= _speed;

        //Check for gravity and jump

        if (!_grounded) {
            if (!_falling) {
                yDir -= gravity * Time.deltaTime;
            } else {
                yDir -= gravity * Time.deltaTime * 2;
            }

        } else {
            yDir = 0;
            if (_jumping) yDir = jumpForce;
        }

        if (_OnSlope()) {
            _moveDirection += Vector3.Cross(slopeHit.normal, slopeHit.transform.forward) * _speed;
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

    void checkFalling() {
        if(_lastYpos > transform.position.y) {
            _falling = true;
        } else {
            _falling = false;
        }
        _lastYpos = transform.position.y;
    }

    private bool _OnSlope() {

        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, 1.5f)) {

        }

        if (Vector3.Angle(slopeHit.normal, Vector3.up) > slopeLimit) {
            return true;
        } else {
            return false;
        }


    }
    private float _cameraTargetOffset {
        get {
            if (_crouching) {
                return (_canUncrouch) ? .5f : .3f;
            } else {
                return .5f;
            }
        }
    }
}
