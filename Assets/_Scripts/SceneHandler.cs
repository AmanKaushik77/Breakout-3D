using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHandler : SingletonMonoBehavior<SceneHandler>
{
    [Header("Scene Data")]
    [SerializeField] private List<string> levels;
    [SerializeField] private string menuScene;
    [Header("Transition Animation Data")]
    [SerializeField] private Ease animationType = Ease.InOutQuad; // Default value
    [SerializeField] private float animationDuration = 1f; // Default value
    [SerializeField] private RectTransform transitionCanvas;

    private int nextLevelIndex;
    private float initXPosition;
    private bool isTransitioning; // Prevent multiple transitions

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject); // Ensure SceneHandler persists across scenes

        if (transitionCanvas == null)
        {
            Debug.LogError("TransitionCanvas is not assigned in SceneHandler!");
        }
        else
        {
            initXPosition = transitionCanvas.transform.localPosition.x;
        }

        SceneManager.sceneLoaded += OnSceneLoad;
        // Only load menu scene if not already in a scene (e.g., first load)
        if (SceneManager.GetActiveScene().buildIndex == 0) // Assuming initial scene is a bootstrap
        {
            SceneManager.LoadScene(menuScene);
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoad;
    }

    private void OnSceneLoad(Scene scene, LoadSceneMode _)
    {
        if (transitionCanvas != null)
        {
            transitionCanvas.DOLocalMoveX(initXPosition, animationDuration).SetEase(animationType);
        }
        isTransitioning = false; // Reset transition flag
    }

    public void LoadNextScene()
    {
        if (isTransitioning)
        {
            Debug.LogWarning("Scene transition already in progress!");
            return;
        }

        if (nextLevelIndex >= levels.Count)
        {
            LoadMenuScene();
        }
        else
        {
            if (transitionCanvas != null)
            {
                isTransitioning = true;
                transitionCanvas.DOLocalMoveX(initXPosition + transitionCanvas.rect.width, animationDuration).SetEase(animationType);
                StartCoroutine(LoadSceneAfterTransition(levels[nextLevelIndex]));
                nextLevelIndex++;
            }
            else
            {
                Debug.LogError("Cannot transition: transitionCanvas is null!");
                SceneManager.LoadScene(levels[nextLevelIndex]); // Fallback
                nextLevelIndex++;
            }
        }
    }

    public void LoadMenuScene()
    {
        if (isTransitioning)
        {
            Debug.LogWarning("Scene transition already in progress!");
            return;
        }

        if (transitionCanvas != null)
        {
            isTransitioning = true;
            transitionCanvas.DOLocalMoveX(initXPosition + transitionCanvas.rect.width, animationDuration).SetEase(animationType);
            StartCoroutine(LoadSceneAfterTransition(menuScene));
            nextLevelIndex = 0;
        }
        else
        {
            Debug.LogError("Cannot transition: transitionCanvas is null!");
            SceneManager.LoadScene(menuScene); // Fallback
            nextLevelIndex = 0;
        }
    }

    private IEnumerator LoadSceneAfterTransition(string scene)
    {
        yield return new WaitForSeconds(animationDuration);
        SceneManager.LoadScene(scene);
    }
}