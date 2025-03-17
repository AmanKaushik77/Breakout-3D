using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : SingletonMonoBehavior<GameManager>
{
    [SerializeField] private int maxLives = 3;
    [SerializeField] private Ball ball;
    [SerializeField] private Transform bricksContainer;
    [SerializeField] private TMPro.TMP_Text scoreText;

    private int currentBrickCount;
    private int totalBrickCount;
    private int score;
    private GameObject uiCanvas;
    private bool isGameOver;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
        Debug.Log($"GameManager Awake - Instance ID: {GetInstanceID()}, Score: {score}");

        uiCanvas = GameObject.FindGameObjectWithTag("UICanvas");
        if (uiCanvas != null)
        {
            DontDestroyOnLoad(uiCanvas);
            scoreText = uiCanvas.GetComponentInChildren<TMPro.TMP_Text>();
            Debug.Log($"Found UICanvas: {uiCanvas.name}, ScoreText: {(scoreText != null ? scoreText.name : "null")}");
            UpdateScoreUI();
        }
        else
        {
            Debug.LogError("UICanvas not found! Please tag your Canvas with 'UICanvas'.");
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
        InputHandler.Instance.OnFire.RemoveListener(FireBall);
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"Scene loaded: {scene.name}, Score before reset: {score}, ScoreText: {(scoreText != null ? scoreText.name : "null")}");
        ResetLevel();
        UpdateScoreUI();
        Debug.Log($"Scene loaded: {scene.name}, Score after reset: {score}, ScoreText: {(scoreText != null ? scoreText.name : "null")}");
    }

    private void ResetLevel()
    {
        ball = FindFirstObjectByType<Ball>();
        if (ball != null)
        {
            ball.ResetBall();
        }
        else
        {
            Debug.LogWarning("No Ball found in the scene!");
        }

        bricksContainer = GameObject.Find("Bricks")?.transform;
        if (bricksContainer != null)
        {
            totalBrickCount = bricksContainer.childCount;
            currentBrickCount = totalBrickCount;
            Debug.Log($"Level initialized with {totalBrickCount} bricks.");
        }
        else
        {
            totalBrickCount = 0;
            currentBrickCount = 0;
            Debug.LogWarning("BricksContainer not found in the scene!");
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
            UpdateScoreUI();
            Debug.Log($"Destroyed Brick at {position}, {currentBrickCount}/{totalBrickCount} remaining, Score: {score}");
            if (currentBrickCount == 0)
            {
                if (SceneHandler.Instance != null)
                {
                    SceneHandler.Instance.LoadNextScene();
                }
                else
                {
                    Debug.LogError("SceneHandler.Instance is null! Cannot load next scene.");
                    SceneManager.LoadScene("MainMenu");
                }
            }
        }
        else
        {
            Debug.LogWarning("Attempted to destroy a brick when currentBrickCount is already 0!");
        }
    }

    public void KillBall()
    {
        maxLives--;
        Debug.Log($"Ball killed, Lives: {maxLives}, Score: {score}");
        UpdateScoreUI();
        if (maxLives < 0 && !isGameOver)
        {
            isGameOver = true;
            score = 0;
            UpdateScoreUI();
            Debug.Log("Game Over - Score reset to 0");
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

    private void UpdateScoreUI()
    {
        if (scoreText == null || !scoreText.gameObject.activeInHierarchy)
        {
            Debug.LogWarning("ScoreText is null or inactive! Attempting to recover...");
            if (uiCanvas != null && uiCanvas.activeInHierarchy)
            {
                scoreText = uiCanvas.GetComponentInChildren<TMPro.TMP_Text>(true); // Include inactive objects
                Debug.Log($"Recovered ScoreText: {(scoreText != null ? scoreText.name : "null")}");
            }
            else
            {
                uiCanvas = GameObject.FindGameObjectWithTag("UICanvas");
                if (uiCanvas != null)
                {
                    scoreText = uiCanvas.GetComponentInChildren<TMPro.TMP_Text>(true);
                    Debug.Log($"Re-found UICanvas: {uiCanvas.name}, ScoreText: {(scoreText != null ? scoreText.name : "null")}");
                }
            }
        }
        if (scoreText != null)
        {
            scoreText.text = $"Score: {score}";
        }
        else
        {
            Debug.LogError("Failed to recover ScoreText!");
        }
    }

    public int GetScore()
    {
        return score;
    }
}