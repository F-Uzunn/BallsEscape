using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SwipeableObject : MonoBehaviour
{
    private Vector3 screenPoint;
    private Vector3 startPosition;
    [Space(10)]
    private GameManager gameManager;
    protected abstract bool RayCheck(Vector3 direction);
    protected abstract void RotateMove(Vector3 direction);
    protected abstract void Move(Vector3 direction);
    protected abstract void FakeMove(Vector3 direction);

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    private void OnMouseDown()
    {
        if (gameManager.isGameFinished) 
            return;

        startPosition = Input.mousePosition;
    }

    private void OnMouseUp()
    {
        if (gameManager.isGameFinished)
            return;

        Vector3 swipeDirection = Input.mousePosition - startPosition;
        float swipeAngle = Mathf.Atan2(swipeDirection.y, swipeDirection.x) * Mathf.Rad2Deg;

        if (swipeDirection.magnitude < 10f)
            return;

        Vector3 direction;
        Vector3 rotateDirection;

        if (Mathf.Abs(swipeAngle) < 45.0f)
        {
            direction = Vector3.right;
            rotateDirection = new Vector3(0, -1, 0);
        }
        else if (Mathf.Abs(swipeAngle) > 135.0f)
        {
            direction = -Vector3.right;
            rotateDirection = new Vector3(0, 1, 0);
        }
        else if (swipeDirection.y > 0)
        {
            direction = Vector3.up;
            rotateDirection = new Vector3(1, 0, 0);
        }
        else
        {
            direction = -Vector3.up;
            rotateDirection = new Vector3(-1, 0, 0);
        }
        startPosition = Input.mousePosition;

        ControlSwipe(direction,rotateDirection);
    }

    private void ControlSwipe(Vector3 direction,Vector3 rotateDirection)
    {
        RotateMove(rotateDirection);
        //Check for every cell of the complete piece...
        for (int i = 0; i < transform.parent.childCount; i++)
        {
            //If on of them hits another bar you cant move it...
            if (transform.parent.GetChild(i).GetComponent<Bar>().RayCheck(direction))
            {
                Debug.Log("cant move");
                FakeMove(direction/10f); //For fake move...
                return;
            }
        }
        Move(direction);
    }
}
