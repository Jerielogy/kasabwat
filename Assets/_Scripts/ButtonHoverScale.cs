using UnityEngine;
using UnityEngine.EventSystems; // Required for hover detection

public class ButtonHoverScale : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Scaling Settings")]
    public Vector3 normalScale = new Vector3(1f, 1f, 1f);
    public Vector3 hoverScale = new Vector3(1.15f, 1.15f, 1.15f);
    public float lerpSpeed = 10f;

    private Vector3 targetScale;

    void Start()
    {
        // Start at the normal size
        transform.localScale = normalScale;
        targetScale = normalScale;
    }

    void Update()
    {
        // Smoothly transition to the target scale every frame
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.unscaledDeltaTime * lerpSpeed);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // When mouse enters the button area
        targetScale = hoverScale;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // When mouse leaves the button area
        targetScale = normalScale;
    }
    void OnDisable()
    {
        // I-reset ang scale sa normal kapag tinago ang menu
        transform.localScale = normalScale;
        targetScale = normalScale;
    }
}