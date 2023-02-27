using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] float rotateSpeed;

    [SerializeField] GameObject Tap, Hold, HoldReleased, HoldReleasedSuccess, HoldReleasedCancel;

    Vector2 _movement;

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
    }
}
