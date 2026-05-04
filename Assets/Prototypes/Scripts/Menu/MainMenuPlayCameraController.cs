using DG.Tweening;
using UnityEngine;

public class MainMenuPlayCameraController : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] private float duration;

    [Header("Canvas Settings")]
    [SerializeField] private GameObject currentCanvas;
    [SerializeField] private GameObject nextCanvas;
    [SerializeField] private float activationDelay;

    public void LookAt(Transform target)
    {
        if (currentCanvas != null)
        {
            currentCanvas.SetActive(false);
        }
        
        transform.DOLookAt(target.position, duration);

        ActivationDelay();
    }

    private void ActivationDelay()
    {
        DOVirtual.DelayedCall(activationDelay, () =>
        {
            if (nextCanvas != null)
            {
                nextCanvas.SetActive(true);
            }
            else
            {
                Debug.LogError("Next Canvas is not assigned in the inspector.");
            }
        });
        }
}
