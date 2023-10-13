using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using DG.Tweening;
using Cinemachine;

public class GameManager : InstanceManager<GameManager>
{

    [field: SerializeField] private GameData gameData;
    [Space(10)]
    [field: SerializeField] private List<GameObject> levelList;
    [Space(10)]
    [field: SerializeField] private GameObject grid;
    [field: SerializeField] private GameObject box;
    [field: SerializeField] private GameObject gameCam;
    [field: SerializeField] private GameObject winCam;
    [field: SerializeField] private GameObject finalBox;
    [Space(10)]
    [field: SerializeField] private Transform boxJumpPos;
    [field: SerializeField] private Transform ballPosLeft;
    [field: SerializeField] private Transform ballPosRight;
    [Space(10)]
    [field: SerializeField] private int totalBallCount;
    [field: SerializeField] private int currentBallCount;
    [Space(10)]
    public bool isGameFinished;

    private float initialPosY = 15;

    public Transform BallPosLeft
    {
        get { return ballPosLeft; }
    }
    public Transform BallPosRight
    {
        get { return ballPosRight; }
    }

    public int TotalBallCount
    {
        get { return totalBallCount; }
        set
        {
            if (value < 0)
                totalBallCount = 0;
            else
                totalBallCount = value;
        }
    }

    public int CurrentBallCount
    {
        get { return currentBallCount; }
        set
        {
            if (value < 0)
                currentBallCount = 0;
            else
                currentBallCount = value;
        }
    }

    void Awake()
    {
        Application.targetFrameRate = 60;

#if !UNITY_EDITOR
        SaveManager.LoadData(gameData);
#endif
    }

    private void Start()
    {
        OnNewLevel();
        EventManager.Broadcast(GameEvent.OnUpdateBallCountText, currentBallCount, totalBallCount);

    }
    private void OnEnable()
    {
        EventManager.AddHandler(GameEvent.OnGameFinished, OnGameFinished);
        EventManager.AddHandler(GameEvent.OnNewLevel, OnNewLevel);
    }

    private void OnDisable()
    {
        EventManager.RemoveHandler(GameEvent.OnGameFinished, OnGameFinished);
        EventManager.RemoveHandler(GameEvent.OnNewLevel, OnNewLevel);
    }

    void OnNewLevel()
    {
        currentBallCount = 0;
        totalBallCount = 0;

        isGameFinished = false;
        finalBox.transform.localScale = Vector3.zero;
        foreach (Transform item in finalBox.transform)
        {
            Destroy(item.gameObject);
        }

        gameCam.SetActive(true);
        winCam.SetActive(false);

        grid = levelList[gameData.LevelIndex - 1];

        grid.SetActive(true);
        EventManager.Broadcast(GameEvent.OnGenerateBalls);

        grid.transform.position = new Vector3(0, initialPosY, 0);
        box.transform.position = new Vector3(0, -initialPosY*2, 0);
        box.transform.rotation = Quaternion.Euler(Vector3.zero);

        //Start animations
        grid.transform.DOMoveY(0f, 1f).SetEase(Ease.OutBack);
        box.transform.DOJump(boxJumpPos.position, 1f, 1, 1f).SetEase(Ease.OutBack).SetDelay(2f);
        box.transform.GetChild(0).DOLocalRotate(new Vector3(box.transform.GetChild(0).rotation.x, box.transform.GetChild(0).rotation.y, 135f), 0.5f).SetDelay(1f);
        box.transform.GetChild(1).DOLocalRotate(new Vector3(box.transform.GetChild(1).rotation.x, box.transform.GetChild(1).rotation.y, -135f), 0.5f).SetDelay(1f);

        EventManager.Broadcast(GameEvent.OnUpdateBallCountText, currentBallCount, totalBallCount);
    }
    public void CheckIfGameFinished()
    {
        if (totalBallCount == currentBallCount)
            EventManager.Broadcast(GameEvent.OnGameFinished);
    }

    public void OnGameFinished()
    {
        StartCoroutine(Win());
    }

    public IEnumerator Win()
    {
        Debug.Log("win");
        isGameFinished = true;
        gameCam.SetActive(false);
        winCam.SetActive(true);

        yield return new WaitForSeconds(1f);
        grid.transform.DOMoveY(initialPosY, 1f).SetEase(Ease.OutBack).OnComplete(() => { grid.SetActive(false); });
        yield return new WaitForSeconds(0.5f);
        finalBox.transform.DOScale(Vector3.one * 3f, 1f).SetEase(Ease.OutBack);
        box.transform.GetChild(0).DOLocalRotate(Vector3.zero, 0.5f);
        box.transform.GetChild(1).DOLocalRotate(Vector3.zero, 0.5f);
        yield return new WaitForSeconds(1f);

        box.transform.root.DOShakeRotation(1f, 10, 10, 90, false, ShakeRandomnessMode.Full);
        yield return new WaitForSeconds(1f);
        box.transform.GetChild(0).DOLocalRotate(new Vector3(box.transform.GetChild(0).rotation.x, box.transform.GetChild(0).rotation.y, 135f), 0.1f);
        box.transform.GetChild(1).DOLocalRotate(new Vector3(box.transform.GetChild(1).rotation.x, box.transform.GetChild(1).rotation.y, -135f), 0.1f);

        yield return new WaitForSeconds(0.1f);
        EventManager.Broadcast(GameEvent.OnBallArrange);
        yield return new WaitForSeconds(0.75f);
        EventManager.Broadcast(GameEvent.OnWinPanelActive);
    }
}