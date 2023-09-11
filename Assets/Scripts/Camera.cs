using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{

    public float speedLeft = 0.2f;
    public float speedRight = -0.2f;
    public bool rotateLeft;

    void Start()
    {
        rotateLeft = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (rotateLeft == true)
        {
            Debug.Log("Turning left");
            RotateLeft();
        }
        else
        {
            Debug.Log("Turning right");
            RotateRight();
        }
    }

    public void RotateLeft()
    {
        transform.Rotate(0, speedLeft * Time.deltaTime, 0);
        StartCoroutine(LeftTimer(20));
    }

    public void RotateRight()
    {
        transform.Rotate(0, speedRight * Time.deltaTime, 0);
        StartCoroutine(RightTimer(20));
    }

    IEnumerator LeftTimer(float time)
    {

        yield return new WaitForSeconds(time);  // Code execution delay
        rotateLeft = false;
    }

    IEnumerator RightTimer(float time)
    {

        yield return new WaitForSeconds(time);  // Code execution delay
        rotateLeft = true;
    }
}
