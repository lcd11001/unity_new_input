using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] float rotateSpeed;
    Vector2 _movement;
    private void OnMovement(InputValue value)
    {
        _movement = value.Get<Vector2>();
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
