using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerControl : MonoBehaviour {

    //Public Variables//
    [Header("Movement")]
    public float walkSpeed;
    public float sneakSpeed;
    public float runSpeed;
    public float jumpForce;
    [Header("Game Feel")]
    public int coyoteTime;
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
    private float _speed, _xMove, _zMove, _lastYpos, _coyoteTimer;
    private bool _grounded, _jumpButton, _jumpTrigger, _jumping, _crouching, _canUncrouch, _falling, _sprinting, _canMantle;
    private Vector3 _moveDirection, _launchVelocity, _mantlePos;
    private RaycastHit slopeHit;


    //Icky code TODO: find a better way to do cam animate//
    [HideInInspector]
    public Vector3 cameraTarget;
    [HideInInspector]
    public bool isMoving, isCrouching, isSprinting;

    void Awake() {
        rb = this.GetComponent<Rigidbody>();
        cc = this.GetComponent<CapsuleCollider>();
        _currentState = PlayerStates.defaultMove;
    }

    void Update() {

        if (Input.GetKeyDown(KeyCode.R)) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        //Inputs//
        _xMove = Input.GetAxis("Horizontal");
        _zMove = Input.GetAxis("Vertical");
        _sprinting = Input.GetButton("Sprint");

        //Jump Behavior (maybe put in own function?)
        _jumpButton = Input.GetButton("Jump");
        if (_grounded && _jumpButton) {
            _jumpTrigger = true;
        }

        if (_jumpTrigger && !_jumpButton) {
            _jumping = true;
            _jumpTrigger = false;
        }

        if (Input.GetButtonDown("Crouch")) {
            if (_crouching) {
                if (_canUncrouch) _crouching = false;
            } else {
                _crouching = true;
            }
        }


        //Condition Checks//
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, groundedRay)) {
            _grounded = true;
            _coyoteTimer = coyoteTime;
        } else {
            _coyoteTimer--;
            if (_coyoteTimer <= 0) {
                _grounded = false;
            }
        }
        _canUncrouch = !(Physics.SphereCast(transform.position, .45f, Vector3.up, out hit, 1f));


        //CamAnimate
        isMoving = ((_xMove != 0 || _zMove != 0) && _grounded);
        isCrouching = _crouching;
        isSprinting = _sprinting;

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

    //Default behavior for moving, crouching, jumping, etc
    void defaultMoveUpdate() {

        //Determine Speed
        if (_xMove != 0 || _zMove != 0) {

            float targetSpeed;

            if (_crouching) {
                targetSpeed = sneakSpeed;
            } else if (_sprinting) {
                targetSpeed = runSpeed;
            } else {
                targetSpeed = walkSpeed;
            }

            _speed = Mathf.Lerp(_speed, targetSpeed, .5f);
        }

        //Crouch
        if (_crouching) {
            cc.height = 1;
            groundedRay = .6f;
            _coyoteTimer = 0;
        } else {
            cc.height = 2;
            groundedRay = 1.1f;
        }


        //Movement//

        //Add directional movement
        float yDir = _moveDirection.y;

        if (_grounded) {
            _moveDirection = (_xMove * transform.right + _zMove * transform.forward).normalized;  //Oreintate based on direction
            _moveDirection *= _speed;
        }

        //Check for gravity and jump

        if (!_grounded) {
                yDir -= gravity * Time.fixedDeltaTime;
        } else {
            yDir = 0;
            _launchVelocity = Vector3.zero;
            if (_jumping) {
                yDir = jumpForce;
//                _launchVelocity = rb.velocity;
                _grounded = false;
            }
        }

        if (_OnSlope()) {
//            _moveDirection += Vector3.Cross(slopeHit.normal, slopeHit.transform.forward) * _speed;
        }

        _moveDirection.y = yDir;

        //Apply direction, if no direction then decay
        if (_moveDirection != Vector3.zero) {
            if (_OnSlope()) {
                _moveDirection += Vector3.Cross(slopeHit.normal, slopeHit.transform.forward) * .1f;
                _coyoteTimer = 0;
                _moveDirection.y = -3f;
            }
            rb.velocity = (_moveDirection + ((_xMove * transform.right + _zMove * transform.forward).normalized * _speed / 2));
        } else {
            rb.velocity *= .6f;
        }

    }

    //Behavior when hanging on ledge
    void hangingUpdate() {

    }

    //Behavior when mantling
    void climbingUpdate() {

    }

    //SUMMARY: is player's last ypos greater than current ypos? if so, falling
    void checkFalling() {
        if(_lastYpos > transform.position.y) {
            _falling = true;
            _jumping = false;
        } else {
            _falling = false;
        }
        _lastYpos = transform.position.y;
    }

    //SUMMARY: is player on slope greater than slope limit? if so, return true
    private bool _OnSlope() {

        if (_jumping) {
            return false;
        }

        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, 1.5f)) {

        }

        if (Vector3.Angle(slopeHit.normal, Vector3.up) > 0) {
            return true;
        } else {
            return false;
        }


    }

    //SUMMARY: determing player camera's target offset for things like crouching, jumping, etc
    private float _cameraTargetOffset {
        get {
            if (_crouching) {
                return (_canUncrouch) ? .5f : .3f; 
            } else {
                if (_grounded && _jumpButton) {
                    return .3f;
                } else {
                    return .5f;
                }
            }
        }
    }
}
