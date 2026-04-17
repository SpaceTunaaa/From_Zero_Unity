using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EndGoalPoint : MonoBehaviour
{
    [Header("Player Lift")]
    [SerializeField] private float upwardDistance = 3f;
    [SerializeField] private Vector3 floatDestination = Vector3.up;
    [SerializeField] private Transform floatDestinationPoint;
    [SerializeField] private float floatDuration = 1f;
    [SerializeField] private float spinSpeed = 90f;
    [SerializeField] private bool freezeCameraRotation = true;

    [Header("Ending")]
    [SerializeField] private float waitTime = 1f;
    [SerializeField] private float fadeDuration = 1f;

    private bool endingStarted;

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryStartEnding(other);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TryStartEnding(collision.collider);
    }

    private void TryStartEnding(Component other)
    {
        GameObject player = GetPlayer(other);

        if (endingStarted || player == null)
        {
            return;
        }

        StartCoroutine(PlayEnding(player));
    }

    private GameObject GetPlayer(Component other)
    {
        if (other == null)
        {
            return null;
        }

        Character2DInputController level2Input = other.GetComponentInParent<Character2DInputController>();
        if (level2Input != null)
        {
            return level2Input.gameObject;
        }

        CharacterMovement graphMovement = other.GetComponentInParent<CharacterMovement>();
        if (graphMovement != null)
        {
            return graphMovement.gameObject;
        }

        return null;
    }

    private IEnumerator PlayEnding(GameObject player)
    {
        endingStarted = true;
        DisablePlayerControl(player);

        Vector3 startPosition = player.transform.position;
        Vector3 endPosition = GetFloatDestination(startPosition);
        Camera[] childCameras = freezeCameraRotation ? player.GetComponentsInChildren<Camera>(true) : null;
        Quaternion[] childCameraRotations = GetCameraRotations(childCameras);
        float elapsed = 0f;
        float duration = Mathf.Max(0.01f, floatDuration);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            player.transform.position = Vector3.Lerp(startPosition, endPosition, t);
            SpinPlayer(player);
            RestoreCameraRotations(childCameras, childCameraRotations);
            yield return null;
        }

        player.transform.position = endPosition;
        RestoreCameraRotations(childCameras, childCameraRotations);

        yield return SpinForSeconds(player, waitTime, childCameras, childCameraRotations);
        yield return FadeToBlack(player, childCameras, childCameraRotations);

        EndGame();
    }

    private IEnumerator SpinForSeconds(GameObject player, float duration, Camera[] childCameras, Quaternion[] childCameraRotations)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            SpinPlayer(player);
            RestoreCameraRotations(childCameras, childCameraRotations);
            yield return null;
        }
    }

    private void SpinPlayer(GameObject player)
    {
        player.transform.Rotate(0f, 0f, spinSpeed * Time.deltaTime);
    }

    private Quaternion[] GetCameraRotations(Camera[] cameras)
    {
        if (cameras == null || cameras.Length == 0)
        {
            return null;
        }

        Quaternion[] rotations = new Quaternion[cameras.Length];

        for (int i = 0; i < cameras.Length; i++)
        {
            rotations[i] = cameras[i].transform.rotation;
        }

        return rotations;
    }

    private void RestoreCameraRotations(Camera[] cameras, Quaternion[] rotations)
    {
        if (cameras == null || rotations == null)
        {
            return;
        }

        for (int i = 0; i < cameras.Length; i++)
        {
            if (cameras[i] != null)
            {
                cameras[i].transform.rotation = rotations[i];
            }
        }
    }

    private Vector3 GetFloatDestination(Vector3 startPosition)
    {
        if (floatDestinationPoint != null)
        {
            return floatDestinationPoint.position;
        }

        return startPosition + GetFloatOffset();
    }

    private Vector3 GetFloatOffset()
    {
        if (floatDestination != Vector3.up || Mathf.Approximately(upwardDistance, 0f))
        {
            return floatDestination;
        }

        return Vector3.up * upwardDistance;
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 start = transform.position;
        Vector3 destination = floatDestinationPoint != null
            ? floatDestinationPoint.position
            : start + GetFloatOffset();

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(start, destination);
        Gizmos.DrawSphere(destination, 0.15f);
    }

    private void DisablePlayerControl(GameObject player)
    {
        Character2DInputController level2Input = player.GetComponent<Character2DInputController>();
        if (level2Input != null)
        {
            level2Input.enabled = false;
        }

        CharacterInputController level1Input = player.GetComponent<CharacterInputController>();
        if (level1Input != null)
        {
            level1Input.enabled = false;
        }

        CharacterMovement graphMovement = player.GetComponent<CharacterMovement>();
        if (graphMovement != null)
        {
            graphMovement.enabled = false;
        }

        Rigidbody2D body = player.GetComponent<Rigidbody2D>();
        if (body != null)
        {
            body.linearVelocity = Vector2.zero;
            body.angularVelocity = 0f;
            body.gravityScale = 0f;
            body.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }

    private IEnumerator FadeToBlack(GameObject player, Camera[] childCameras, Quaternion[] childCameraRotations)
    {
        Image fadeImage = CreateFadeImage();
        Color color = Color.black;
        color.a = 0f;
        fadeImage.color = color;

        float elapsed = 0f;
        float duration = Mathf.Max(0.01f, fadeDuration);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Clamp01(elapsed / duration);
            fadeImage.color = color;
            SpinPlayer(player);
            RestoreCameraRotations(childCameras, childCameraRotations);
            yield return null;
        }

        color.a = 1f;
        fadeImage.color = color;
    }

    private Image CreateFadeImage()
    {
        GameObject canvasObject = new GameObject("End Screen Fade");
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = short.MaxValue;

        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;

        canvasObject.AddComponent<GraphicRaycaster>();

        GameObject imageObject = new GameObject("Black Screen");
        imageObject.transform.SetParent(canvasObject.transform, false);

        Image image = imageObject.AddComponent<Image>();
        RectTransform rectTransform = image.rectTransform;
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;

        return image;
    }

    private void EndGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
