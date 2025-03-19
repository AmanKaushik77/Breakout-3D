using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class GameManager : SingletonMonoBehavior<GameManager>
{
    [SerializeField] private int maxLives = 3;
    [SerializeField] private Ball ball;
    [SerializeField] private Transform bricksContainer;
    [SerializeField] private UIScript uiScript; // New reference to UIScript

    private int currentBrickCount;
    private int totalBrickCount;
    private int score;
    private GameObject uiCanvas;
    private bool isGameOver;
    private static GameManager persistentInstance;

    protected override void Awake()
    {
        if (persistentInstance != null && persistentInstance != this)
        {
            Destroy(gameObject);
            return;
        }

        persistentInstance = this;
        base.Awake();
        DontDestroyOnLoad(gameObject);

        SetupUICanvas();
    }

    private void SetupUICanvas()
    {
        uiCanvas = GameObject.FindGameObjectWithTag("UICanvas");
        if (uiCanvas != null)
        {
            DontDestroyOnLoad(uiCanvas);
            // Ensure uiScript is assigned or found
            if (uiScript == null)
            {
                uiScript = uiCanvas.GetComponentInChildren<UIScript>();
            }
        }
    }

    private void OnEnable()
    {
        InputHandler.Instance.OnFire.AddListener(FireBall);
        SceneManager.sceneLoaded += OnSceneLoaded;
        ResetLevel();
    }

    private void OnDisable()
    {
        if (InputHandler.Instance != null)
            InputHandler.Instance.OnFire.RemoveListener(FireBall);
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SetupUICanvas();
        ResetLevel();
        if (scene.name != "MainMenu" && uiScript != null)
        {
            uiScript.UpdateScore(score); // Ensure score is updated on scene load
        }
    }

    private void ResetLevel()
    {
        ball = FindFirstObjectByType<Ball>();
        if (ball != null)
        {
            ball.ResetBall();
        }

        bricksContainer = GameObject.Find("Bricks")?.transform;
        if (bricksContainer != null)
        {
            totalBrickCount = bricksContainer.childCount;
            currentBrickCount = totalBrickCount;
        }
        else
        {
            totalBrickCount = 0;
            currentBrickCount = 0;
        }
    }

    private void FireBall()
    {
        if (ball != null) ball.FireBall();
    }

    public void OnBrickDestroyed(Vector3 position)
    {
        if (currentBrickCount > 0)
        {
            currentBrickCount--;
            score++;
            if (uiScript != null)
            {
                uiScript.UpdateScore(score);
            }
            if (currentBrickCount == 0)
            {
                if (SceneHandler.Instance != null)
                {
                    SceneHandler.Instance.LoadNextScene();
                }
                else
                {
                    SceneManager.LoadScene("MainMenu");
                }
            }
        }
    }

    public void KillBall()
    {
        maxLives--;
        if (uiScript != null)
        {
            uiScript.UpdateScore(score);
        }
        if (maxLives < 0 && !isGameOver)
        {
            isGameOver = true;
            score = 0;
            if (uiScript != null)
            {
                uiScript.UpdateScore(score);
            }
            if (SceneHandler.Instance != null)
            {
                SceneHandler.Instance.LoadMenuScene();
            }
            else
            {
                SceneManager.LoadScene("MainMenu");
            }
            maxLives = 3;
            isGameOver = false;
        }
        if (ball != null) ball.ResetBall();
    }

    public int GetScore()
    {
        return score;
    }
}