using UnityEngine;
using UnityEngine.EventSystems;

//To make the UI elements more interactive and responsive, this script adds a "juice" effect to UI elements when hovered over or clicked. It scales the UI element up when hovered and scales it down when clicked, providing visual feedback to the user.
public class UIJuice : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    private Vector3 originalScale;
    private Vector3 targetScale;

    private readonly Vector3 hoverScale = new Vector3(1.1f, 1.1f, 1.1f);
    private readonly Vector3 clickScale = new Vector3(0.95f, 0.95f, 0.95f);
    private readonly float smoothSpeed = 15f;

    void Start()
    {
        originalScale = transform.localScale;
        targetScale = originalScale;
    }

    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * smoothSpeed);
    }

    public void OnPointerEnter(PointerEventData eventData) => targetScale = hoverScale;
    public void OnPointerExit(PointerEventData eventData) => targetScale = originalScale;
    public void OnPointerDown(PointerEventData eventData) => targetScale = clickScale;
    public void OnPointerUp(PointerEventData eventData) => targetScale = hoverScale;
}