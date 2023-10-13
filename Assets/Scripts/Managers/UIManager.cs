using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
public class UIManager : InstanceManager<UIManager>
{
    [field: SerializeField] private GameData gameData;

    [field: SerializeField] private GameObject gamePanel;
    [field: SerializeField] private GameObject winPanel;

    [field: SerializeField] private TextMeshProUGUI ballCountText;
    [field: SerializeField] private TextMeshProUGUI levelText;
    private void Start()
    {
        levelText.text = "LEVEL " + gameData.FakeLevelIndex.ToString();
    }
    private void OnEnable()
    {
        EventManager.AddHandler(GameEvent.OnUpdateBallCountText, OnUpdateBallCountText);
        EventManager.AddHandler(GameEvent.OnGameFinished, OnGameFinished);
        EventManager.AddHandler(GameEvent.OnWinPanelActive, OnWinPanelActive);
    }

    private void OnDisable()
    {
        EventManager.RemoveHandler(GameEvent.OnUpdateBallCountText, OnUpdateBallCountText);
        EventManager.RemoveHandler(GameEvent.OnGameFinished, OnGameFinished);
        EventManager.RemoveHandler(GameEvent.OnWinPanelActive, OnWinPanelActive);
    }

   
    private void OnGameFinished()
    {
        gamePanel.SetActive(false);
    }
    private void OnWinPanelActive()
    {
        winPanel.SetActive(true);
    }

    void OnUpdateBallCountText(object currentCount, object totalCount)
    {
        ballCountText.text = (int)currentCount + "/" + (int)totalCount;
    }

    public void NextLevelButton()
    {
        gameData.LevelIndex++;
        gameData.FakeLevelIndex++;

        winPanel.SetActive(false);
        gamePanel.SetActive(true);

        levelText.text = "LEVEL " + gameData.FakeLevelIndex.ToString();

        EventManager.Broadcast(GameEvent.OnNewLevel);
        SaveManager.SaveData(gameData);
    }

    public void RestartButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}