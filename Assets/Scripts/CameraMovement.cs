using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float XBound, YBound;
    public float speed;
    public Vector2Int VX, VY;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float change;
        if(Input.mousePosition.x < XBound)
        {
            change = (Input.mousePosition.x - XBound) * speed;
            Vector3 changeVector = new Vector3(change, 0, -change);
            transform.position += changeVector;
            if(transform.position.x - transform.position.z < VX.x)
            {
                transform.position -= changeVector;
                transform.position += changeVector.normalized * (transform.position.x - transform.position.z - VX.x) / Mathf.Sqrt(2);
            }
        }
        if(Input.mousePosition.x > Camera.main.pixelWidth - XBound)
        {
            change = (Input.mousePosition.x - (Camera.main.pixelWidth - XBound)) * speed;
            Vector3 changeVector = new Vector3(change, 0, -change);
            transform.position += changeVector;
            if(transform.position.x - transform.position.z > VX.y)
            {
                transform.position -= changeVector;
                transform.position += changeVector.normalized * -(transform.position.x - transform.position.z - VX.y) / Mathf.Sqrt(2);
            }
        }
        if (Input.mousePosition.y < YBound)
        {
            change = (Input.mousePosition.y - YBound) * speed;
            Vector3 changeVector = new Vector3(change, 0, change);
            transform.position += changeVector;
            if(transform.position.x + transform.position.z < VY.x)
            {
                transform.position -= changeVector;
                transform.position += changeVector.normalized * (transform.position.x + transform.position.z - VY.x) / Mathf.Sqrt(2);
            }
        }
        if (Input.mousePosition.y > Camera.main.pixelHeight - YBound)
        {
            change = (Input.mousePosition.y - (Camera.main.pixelHeight - YBound)) * speed;
            Vector3 changeVector = new Vector3(change, 0, change);
            transform.position += changeVector;
            if(transform.position.x + transform.position.z > VY.y)
            {
                transform.position -= changeVector;
                transform.position += changeVector.normalized * -(transform.position.x + transform.position.z - VY.y) / Mathf.Sqrt(2);
            }
        }

        //transform.position = new Vector3(Mathf.Clamp(transform.position.x, MinV.x, MaxV.x), transform.position.y, Mathf.Clamp(transform.position.z, MinV.y, MaxV.y));
    }
}
