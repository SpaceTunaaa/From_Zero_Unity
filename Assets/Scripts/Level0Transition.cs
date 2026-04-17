using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Level0Transition : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private Animator targetAnimator;
    [SerializeField] private AnimationClip targetAnimation;
    [SerializeField] private string targetAnimationStateName;
    [SerializeField] private float fallbackAnimationLength = 1f;
    [SerializeField] private float sceneLoadEarlyOffset = 0.05f;

    [Header("Click")]
    [SerializeField] private Camera clickCamera;

    [Header("Scene")]
    [SerializeField] private string nextSceneName = "Level1";

    private SpriteRenderer spriteRenderer;
    private bool transitionStarted;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (targetAnimator == null)
        {
            targetAnimator = GetComponent<Animator>();
        }

        if (clickCamera == null)
        {
            clickCamera = Camera.main;
        }

        if (clickCamera == null)
        {
            clickCamera = FindFirstObjectByType<Camera>();
        }
    }

    private void Update()
    {
        if (transitionStarted || Mouse.current == null || !Mouse.current.leftButton.wasPressedThisFrame)
        {
            return;
        }

        if (WasPlayerClicked())
        {
            StartCoroutine(PlayAnimationThenLoadScene());
        }
    }

    private bool WasPlayerClicked()
    {
        if (clickCamera == null)
        {
            return false;
        }

        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPosition = clickCamera.ScreenToWorldPoint(mouseScreenPosition);
        mouseWorldPosition.z = transform.position.z;

        if (spriteRenderer != null)
        {
            return spriteRenderer.bounds.Contains(mouseWorldPosition);
        }

        return false;
    }

    private IEnumerator PlayAnimationThenLoadScene()
    {
        transitionStarted = true;
        float animationLength = fallbackAnimationLength;

        if (targetAnimator != null)
        {
            targetAnimator.enabled = true;

            string stateName = GetTargetStateName();

            if (!string.IsNullOrEmpty(stateName))
            {
                targetAnimator.Play(stateName, 0, 0f);
            }

            targetAnimator.Update(0f);
        }

        if (targetAnimation != null)
        {
            animationLength = targetAnimation.length;
        }

        float waitTime = Mathf.Max(0f, animationLength - sceneLoadEarlyOffset);
        yield return new WaitForSeconds(waitTime);
        SceneManager.LoadScene(nextSceneName);
    }

    private string GetTargetStateName()
    {
        if (!string.IsNullOrEmpty(targetAnimationStateName))
        {
            return targetAnimationStateName;
        }

        if (targetAnimation != null)
        {
            return targetAnimation.name;
        }

        return string.Empty;
    }
}
