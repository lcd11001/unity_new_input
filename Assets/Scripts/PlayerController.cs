using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class PlayerController : MonoBehaviour
{
    [SerializeField] Camera cam;
    [SerializeField] Transform debugSphere;
    [SerializeField] float moveSpeed;
    [SerializeField] float rotateSpeed;

    [SerializeField] GameObject Tap, Hold, HoldReleased, HoldReleasedSuccess, HoldReleasedCancel;

    Vector2 _movement;
    Vector3 _touchPosition;
    bool isTouchPressed = false;

#if ENABLE_INPUT_SYSTEM
    // This method is called by PlayerInput with behavior SendMessages
    private void OnMovement(InputValue value)
    {
        MoveInput(value.Get<Vector2>());
    }

    // This method is called by PlayerInput with behavior Invoke Unity Events
    public void OnMovement(InputAction.CallbackContext context)
    {
        MoveInput(context.ReadValue<Vector2>());
    }

    public void OnInteraction(InputAction.CallbackContext context)
    {
        SetAllInactive();

        switch (context.phase)
        {
            case InputActionPhase.Started:
                //Debug.Log($"{context.interaction} Started");
                if (context.interaction is SlowTapInteraction)
                {
                    Hold.SetActive(true);
                }
                break;
            case InputActionPhase.Performed:
                //Debug.Log($"{context.interaction} Performed");
                if (context.interaction is SlowTapInteraction)
                {
                    HoldReleased.SetActive(true);
                    HoldReleasedSuccess.SetActive(true);
                }
                else
                {
                    Tap.SetActive(true);
                }
                break;
            case InputActionPhase.Canceled:
                //Debug.Log($"{context.interaction} Canceled");
                HoldReleased.SetActive(true);
                HoldReleasedCancel.SetActive(true);
                break;
            default:
                break;
        }
    }

    public void OnTouchPosition(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Performed:
                TouchPositionInput(context.ReadValue<Vector2>());
                break;
            //case InputActionPhase.Canceled:
            //    TouchPositionInput(Vector3.zero);
            //    break;
            default:
                break;
        }
    }

    public void OnTouchPress(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Performed:
                isTouchPressed = true;
                break;
            case InputActionPhase.Canceled:
                isTouchPressed = false;
                break;
            default:
                break;
        }
    }
#endif

    private void SetAllInactive()
    {
        Tap.SetActive(false);
        Hold.SetActive(false);
        HoldReleased.SetActive(false);
        HoldReleasedSuccess.SetActive(false);
        HoldReleasedCancel.SetActive(false);
    }

    private void MoveInput(Vector2 newMove)
    {
        _movement = newMove;
    }

    private void TouchPositionInput(Vector3 newPos)
    {
        //if (isTouchPressed)
        {
            Ray ray = cam.ScreenPointToRay(newPos);
            int layer = 1 << LayerMask.NameToLayer("Ground");
            if (Physics.Raycast(ray, out RaycastHit info, 100f, layer))
            {
                _touchPosition = info.point;
            }
        }
    }

    private void Awake()
    {
        SetAllInactive();
    }

    private void Update()
    {
        transform.position += new Vector3(_movement.x, 0, _movement.y) * moveSpeed * Time.deltaTime;
        if (_movement != Vector2.zero)
        {
            float targetAngle = Mathf.Atan2(_movement.x, _movement.y) * Mathf.Rad2Deg;
            float angle = Mathf.LerpAngle(transform.rotation.eulerAngles.y, targetAngle, rotateSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
        }

        if (isTouchPressed)
        {
            Vector3 facingDirection = _touchPosition - transform.position;
            float targetAngle = Mathf.Atan2(facingDirection.x, facingDirection.z) * Mathf.Rad2Deg;
            float angle = Mathf.LerpAngle(transform.rotation.eulerAngles.y, targetAngle, rotateSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            transform.position += (facingDirection.normalized * moveSpeed * Time.deltaTime);
        }

        if (debugSphere != null)
        {
            debugSphere.gameObject.SetActive(isTouchPressed);
            debugSphere.position = _touchPosition;
        }
    }
}
