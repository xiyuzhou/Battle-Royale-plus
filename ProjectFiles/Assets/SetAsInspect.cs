using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SetAsInspect : MonoBehaviour
{
    private float rotX;
    private float rotY;
    private bool isSpectator;
    void Update()
    {
        if (isSpectator)
        {
            rotX += Input.GetAxis("Mouse X") * 5;
            rotY += Input.GetAxis("Mouse Y") * 5;

            // clamp the vertical rotation
            rotY = Mathf.Clamp(rotY, -90, 90);
            // rotate the cam vertically
            transform.rotation = Quaternion.Euler(-rotY, rotX, 0);
            // movement
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");
            float y = 0;
            if (Input.GetKey(KeyCode.E))
                y = 1;
            else if (Input.GetKey(KeyCode.Q))
                y = -1;
            Vector3 dir = transform.right * x + transform.up * y + transform.forward * z;
            transform.position += dir * 10 * Time.deltaTime;
        }
    }
    public void SetAsSpectator()
    {
        isSpectator = true;
        transform.parent = null;
    }
}
