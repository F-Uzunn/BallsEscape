using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CellHole : MonoBehaviour
{
    [field: SerializeField] private GameManager gameManager;
    [Space(10)]
    [field: SerializeField] private List<GameObject> ballList;
    [Space(10)]
    [field: SerializeField] private GameObject ballPrefab;
    [Space(10)]
    [field: SerializeField] private LayerMask layerMask;
    [Space(10)]
    [field: SerializeField] private float rayLength = 2f;
    [field: SerializeField] private int howManyBallsWillbeSpawned;
    [Space(10)]
    [field: SerializeField] private bool areBallsSent;

    Ray ray;
    RaycastHit hit;
    private void Awake()
    {
        gameManager = GameManager.Instance.GetComponent<GameManager>();
    }
    private void OnEnable()
    {
        EventManager.AddHandler(GameEvent.OnCheckBallsRelease, OnCheckBallsRelease);
        EventManager.AddHandler(GameEvent.OnGenerateBalls, OnGenerateBalls);
    }

    private void OnDisable()
    {
        EventManager.RemoveHandler(GameEvent.OnCheckBallsRelease, OnCheckBallsRelease);
        EventManager.RemoveHandler(GameEvent.OnGenerateBalls, OnGenerateBalls);
    }

 
    void OnGenerateBalls()
    {
        areBallsSent = false;

        float minX = -transform.localScale.x;
        float maxX = transform.localScale.x;
        for (int i = 0; i < howManyBallsWillbeSpawned; i++)
        {
            GameObject ball = Instantiate(ballPrefab, transform);
            ballList.Add(ball);
            ball.transform.localPosition = new Vector3(Random.Range(minX / 3f, maxX / 3f), Random.Range(minX / 3f, maxX / 3f), -0.5f);

            gameManager.TotalBallCount++;
        }
    }

    public void OnCheckBallsRelease()
    {
        ray = new Ray(transform.position, -Vector3.forward * rayLength);

        //if cellhole ray hits bar dont send balls.
        if (Physics.Raycast(transform.position, transform.TransformDirection(-Vector3.forward), out hit, rayLength, layerMask))
            Debug.Log("Dont send balls");
        else if(!areBallsSent)
            StartCoroutine(SendBalls());
    }

    IEnumerator SendBalls()
    {
        areBallsSent = true;

        int listCount = ballList.Count;

        int childIndex = transform.parent.GetSiblingIndex();

        Transform ballMovePos;

        if (childIndex < 6)
            ballMovePos = gameManager.BallPosLeft;
        else
            ballMovePos = gameManager.BallPosRight;


        for (int i = 0; i < listCount; i++)
        {
            Transform ballTransform = ballList[i].transform;
            Rigidbody ballRigidbody = ballList[i].GetComponent<Rigidbody>();

            ballTransform.SetParent(ballMovePos.root);
            ballTransform.DOJump(ballMovePos.position + new Vector3(0, 0.5f, Random.Range(-0.25f, 0.25f)), Random.Range(1f, 1.5f), 1, 0.5f).SetEase(Ease.InOutSine);
            ballTransform.DOScale(Vector3.one * 0.2f, 0.75f);

            ballRigidbody.isKinematic = false;
            ballRigidbody.useGravity = true;

            gameManager.CurrentBallCount++;
            yield return new WaitForSeconds(0.075f);
            EventManager.Broadcast(GameEvent.OnUpdateBallCountText, gameManager.CurrentBallCount, gameManager.TotalBallCount);
            ballMovePos.root.DOScale(Vector3.one * 1.025f,0.05f).SetDelay(0.35f).OnComplete(()=> { ballMovePos.root.DOScale(Vector3.one, 0.05f); });
        }

        //all balls are sent and list will be cleared...
        ballList.Clear();

        gameManager.CheckIfGameFinished();
    }
}
