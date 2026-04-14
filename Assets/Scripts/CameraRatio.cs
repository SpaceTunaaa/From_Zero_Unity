using UnityEngine;

// Forces this camera to render inside a chosen aspect ratio using letterboxing or pillarboxing.
[ExecuteAlways]
[RequireComponent(typeof(Camera))]
public class CameraRatio : MonoBehaviour
{
    [Header("Target Aspect Ratio")]
    [SerializeField] private float targetWidth = 16f;
    [SerializeField] private float targetHeight = 9f;

    private Camera targetCamera;
    private int lastScreenWidth;
    private int lastScreenHeight;

    private void Awake()
    {
        targetCamera = GetComponent<Camera>();
    }

    private void OnEnable()
    {
        ApplyAspectRatio();
    }

    private void OnValidate()
    {
        if (targetWidth <= 0f)
        {
            targetWidth = 16f;
        }

        if (targetHeight <= 0f)
        {
            targetHeight = 9f;
        }

        ApplyAspectRatio();
    }

    private void Update()
    {
        if (Screen.width != lastScreenWidth || Screen.height != lastScreenHeight)
        {
            ApplyAspectRatio();
        }
    }

    // Recalculates the camera viewport so the rendered image keeps the chosen aspect ratio.
    private void ApplyAspectRatio()
    {
        if (targetCamera == null)
        {
            targetCamera = GetComponent<Camera>();
        }

        if (targetCamera == null || targetWidth <= 0f || targetHeight <= 0f || Screen.height == 0)
        {
            return;
        }

        float targetAspect = targetWidth / targetHeight;
        float screenAspect = (float)Screen.width / Screen.height;
        float scaleHeight = screenAspect / targetAspect;
        Rect rect = new Rect(0f, 0f, 1f, 1f);

        if (scaleHeight < 1f)
        {
            rect.width = 1f;
            rect.height = scaleHeight;
            rect.x = 0f;
            rect.y = (1f - scaleHeight) * 0.5f;
        }
        else
        {
            float scaleWidth = 1f / scaleHeight;
            rect.width = scaleWidth;
            rect.height = 1f;
            rect.x = (1f - scaleWidth) * 0.5f;
            rect.y = 0f;
        }

        targetCamera.rect = rect;
        lastScreenWidth = Screen.width;
        lastScreenHeight = Screen.height;
    }
}
