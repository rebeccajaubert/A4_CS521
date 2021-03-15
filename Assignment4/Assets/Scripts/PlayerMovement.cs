using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public Transform cam;
    public float speed = 6f;
    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;

    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y; // add cam angle so that player in direction of camera
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime); //to smooth player s turn
            transform.rotation = Quaternion.Euler(0f, angle, 0f); //player rotate 

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward; //move in direction of camera
            moveDir.y = 1.48f - transform.position.y;   //correct y pos
            controller.Move((moveDir.normalized) * speed * Time.deltaTime);
        }

        if (!Input.anyKey)
        {
            controller.Move(Vector3.zero);
        }

    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.CompareTag("Mouse"))   //push mice if collide (it means the player walked up to the mouse)
        {
            Rigidbody body = hit.collider.attachedRigidbody;
            Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
            body.velocity = pushDir * 10;
        }

        if (hit.collider.name.Equals("MonsterArea")) //monster will emerge
        {
            TerrainManager.playerDetected = true;
        }
    }
}
