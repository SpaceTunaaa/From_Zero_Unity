using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level1Transition : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private Animator targetAnimator;
    [SerializeField] private AnimationClip targetAnimation;
    [SerializeField] private string targetAnimationStateName;
    [SerializeField] private float fallbackAnimationLength = 1f;
    [SerializeField] private float sceneLoadEarlyOffset = 0.05f;
    [SerializeField] private bool placeAnimationOnPlayer = true;
    [SerializeField] private bool hidePlayerSpriteDuringTransition = true;
    [SerializeField] private float transitionAnimationScale = 0.15f;

    [Header("Scene")]
    [SerializeField] private string nextSceneName = "Level2";
    [SerializeField] private bool disableMovementDuringTransition = true;

    private CharacterMovement characterMovement;
    private CharacterInputController inputController;
    private SpriteRenderer playerSpriteRenderer;
    private bool transitionStarted;

    private void Awake()
    {
        CacheComponents();
    }

    private void OnEnable()
    {
        CacheComponents();

        if (characterMovement != null)
        {
            characterMovement.NodeReached += HandleNodeReached;
        }
    }

    private void OnDisable()
    {
        if (characterMovement != null)
        {
            characterMovement.NodeReached -= HandleNodeReached;
        }
    }

    private void CacheComponents()
    {
        if (characterMovement == null)
        {
            characterMovement = GetComponent<CharacterMovement>();
        }

        if (inputController == null)
        {
            inputController = GetComponent<CharacterInputController>();
        }

        if (playerSpriteRenderer == null)
        {
            playerSpriteRenderer = GetComponent<SpriteRenderer>();
        }

        if (targetAnimator == null)
        {
            targetAnimator = GetComponent<Animator>();
        }
    }

    private void HandleNodeReached(Node node)
    {
        if (transitionStarted || node == null || !node.IsTransition)
        {
            return;
        }

        StartTransition();
    }

    private void StartTransition()
    {
        if (transitionStarted)
        {
            return;
        }

        StartCoroutine(PlayAnimationThenLoadScene());
    }

    private IEnumerator PlayAnimationThenLoadScene()
    {
        transitionStarted = true;

        if (disableMovementDuringTransition)
        {
            if (characterMovement != null)
            {
                characterMovement.enabled = false;
            }

            if (inputController != null)
            {
                inputController.enabled = false;
            }
        }

        float animationLength = fallbackAnimationLength;

        if (targetAnimator != null)
        {
            targetAnimator.enabled = true;
            PrepareTransitionVisual();

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

    private void PrepareTransitionVisual()
    {
        if (targetAnimator == null)
        {
            return;
        }

        Transform animationTransform = targetAnimator.transform;

        if (placeAnimationOnPlayer)
        {
            if (animationTransform.parent == transform)
            {
                animationTransform.localPosition = Vector3.zero;
                animationTransform.localRotation = Quaternion.identity;
            }
            else
            {
                animationTransform.position = transform.position;
                animationTransform.rotation = transform.rotation;
            }
        }

        float scale = Mathf.Max(0.01f, transitionAnimationScale);
        animationTransform.localScale = Vector3.one * scale;

        SpriteRenderer transitionSpriteRenderer = targetAnimator.GetComponent<SpriteRenderer>();

        if (transitionSpriteRenderer != null)
        {
            transitionSpriteRenderer.enabled = true;

            if (playerSpriteRenderer != null)
            {
                transitionSpriteRenderer.sortingLayerID = playerSpriteRenderer.sortingLayerID;
                transitionSpriteRenderer.sortingOrder = playerSpriteRenderer.sortingOrder + 1;
            }
        }

        if (hidePlayerSpriteDuringTransition && playerSpriteRenderer != null)
        {
            playerSpriteRenderer.enabled = false;
        }
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
