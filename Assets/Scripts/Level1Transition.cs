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

    [Header("Scene")]
    [SerializeField] private string nextSceneName = "Level2";
    [SerializeField] private bool disableMovementDuringTransition = true;

    private CharacterMovement characterMovement;
    private CharacterInputController inputController;
    private bool transitionStarted;

    private void Awake()
    {
        characterMovement = GetComponent<CharacterMovement>();
        inputController = GetComponent<CharacterInputController>();

        if (targetAnimator == null)
        {
            targetAnimator = GetComponent<Animator>();
        }
    }

    private void Update()
    {
        if (transitionStarted)
        {
            return;
        }

        if (IsOnTransitionNode())
        {
            StartTransition();
        }
    }

    private bool IsOnTransitionNode()
    {
        return characterMovement != null
            && characterMovement.CurrentNode != null
            && characterMovement.CurrentNode.IsTransition;
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
