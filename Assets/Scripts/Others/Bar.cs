using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Bar : SwipeableObject
{
    [field:SerializeField] private float rayLength;
    [Space(10)]
    [field: SerializeField] private LayerMask layerMask;

    Ray ray;
    RaycastHit hit;

    private Vector3 initialPos;
    private Vector3 parentInitialPos;

    private void Start()
    {
        initialPos = transform.localPosition;
        parentInitialPos = transform.parent.localPosition;
    }

    private void OnEnable()
    {
        EventManager.AddHandler(GameEvent.OnNewLevel, OnNewLevel);
    }

    private void OnDisable()
    {
        EventManager.RemoveHandler(GameEvent.OnNewLevel, OnNewLevel);
    }
    void OnNewLevel()
    {
        transform.localPosition = initialPos;
        transform.parent.localPosition = parentInitialPos;
    }

    protected override void Move(Vector3 direction)
    {
        transform.parent.DOLocalMove(transform.parent.localPosition + (new Vector3(direction.x*2f,direction.y*2f,0)), 0.1f).OnComplete(() => 
        {
            EventManager.Broadcast(GameEvent.OnCheckBallsRelease);
        });
    }

    protected override void FakeMove(Vector3 direction)
    {
        Vector3 initialPos = transform.parent.localPosition;
        transform.parent.DOLocalMove(transform.parent.localPosition + (new Vector3(direction.x * 2f, direction.y * 2f, 0)), 0.05f).OnComplete(() =>
        {
            transform.parent.DOLocalMove(initialPos, 0.05f);
        });
    }

    protected override bool RayCheck(Vector3 direction)
    {
        ray = new Ray(transform.position, direction * rayLength);
        if (Physics.Raycast(transform.position, direction, out hit, rayLength, layerMask))
        {
            return hit.transform.parent.gameObject != transform.parent.gameObject;
        }
        return false;
    }

    protected override void RotateMove(Vector3 direction)
    {
        transform.root.DORotate(direction*2.5f, 0.1f).OnComplete(()=> { transform.root.DORotate(Vector3.zero, 0.1f); });
    }
}
