using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public bool FreeRotate = false;

    void Update()
    {

        //This is axis locked movement
        if (FreeRotate == false)
        {
            this.transform.Rotate(Vector3.up, Input.GetAxis("Horizontal")); //Around Y-axis
            this.transform.Rotate(Vector3.right, Input.GetAxis("Vertical")); //Around X-axis

            //We hardcode Q and E to X-Axis, its bad way to do thing but works for a demo
            if (Input.GetKey(KeyCode.Q))
            {
                this.transform.Rotate(Vector3.forward, 1);
            }
            if (Input.GetKey(KeyCode.E))
            {
                this.transform.Rotate(Vector3.forward, -1);
            }
        }

        //This is free rotation using the power of quaternions
        if (FreeRotate == true)
        {
            //It will act like the centerpoint is stuck to a sphere
            this.transform.Rotate(this.transform.up, Input.GetAxis("Horizontal")); //Around Y-axis
            this.transform.Rotate(this.transform.right, Input.GetAxis("Vertical")); //Around X-axis

            //We hardcode Q and E to X-Axis, its bad way to do thing but works for a demo
            if (Input.GetKey(KeyCode.Q))
            {
                this.transform.Rotate(this.transform.forward, 1);
            }
            if (Input.GetKey(KeyCode.E))
            {
                this.transform.Rotate(this.transform.forward, -1);
            }
        }
    }
}
