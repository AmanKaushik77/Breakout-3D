﻿using UnityEngine;
using TMPro;

public class GameManager : SingletonMonoBehavior<GameManager>
{
    [SerializeField] private int maxLives = 3;
    [SerializeField] private Ball ball;
    [SerializeField] private Transform bricksContainer;
    [SerializeField] private TMP_Text livesText;
    [SerializeField] private GameObject gameOverCanvas;

    private int currentBrickCount;
    private int totalBrickCount;

    private void OnEnable()
    {
        InputHandler.Instance.OnFire.AddListener(FireBall);
        ball.ResetBall();
        totalBrickCount = bricksContainer.childCount;
        currentBrickCount = bricksContainer.childCount;
        UpdateLivesUI();
        gameOverCanvas.SetActive(false); 
    }

    private void OnDisable()
    {
        InputHandler.Instance.OnFire.RemoveListener(FireBall);
    }

    private void FireBall()
    {
        ball.FireBall();
    }

    public void OnBrickDestroyed(Vector3 position)
    {
        if (bricksContainer == null) return;

        currentBrickCount--;
        Debug.Log($"Destroyed Brick at {position}, {currentBrickCount}/{totalBrickCount} remaining");

        if (currentBrickCount == 0) 
        {
            SceneHandler.Instance.LoadNextScene();
        }
    }

    public void KillBall()
    {
        maxLives--;
        UpdateLivesUI();
        
        Debug.Log($"Lives remaining: {maxLives}");

        if (maxLives <= 0)
        {
            ball.ResetBall();
            ShowGameOverScreen();
        }
        else
        {
            ball.ResetBall();
        }
    }

    private void ShowGameOverScreen()
    {
        if (gameOverCanvas != null)
        {
            gameOverCanvas.SetActive(true); 
            Invoke("ReturnToMainMenu", 1.5f); 
        }
        else
        {
            Debug.LogError("Game Over Canvas is not assigned in the Inspector.");
            ReturnToMainMenu();
        }
    }

    private void UpdateLivesUI()
    {
        if (livesText != null)
        {
            livesText.text = $"Lives: {Mathf.Max(maxLives, 0)}";
        }
    }

    private void ReturnToMainMenu()
    {
        if (gameOverCanvas != null)
        {
            gameOverCanvas.SetActive(false);
        }
        SceneHandler.Instance.LoadMenuScene();
    }
}
