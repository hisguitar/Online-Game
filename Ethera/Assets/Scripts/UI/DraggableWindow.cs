using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableWindow : MonoBehaviour, IDragHandler, IPointerDownHandler
{
	[SerializeField] private RectTransform rectTransform;
	[SerializeField] private Canvas canvas;

	private void Awake()
	{
		if (rectTransform == null)
		{
			rectTransform = transform.parent.GetComponent<RectTransform>();
		}
		
		if (canvas == null)
		{
			Transform canvasTransform = transform.parent;
			while (canvasTransform != null)
			{
				canvas = canvasTransform.GetComponent<Canvas>();
				if (canvas != null)
				{
					break;
				}
				canvasTransform = canvasTransform.parent;
			}
		}
	}

	public void OnDrag(PointerEventData eventData)
	{
		rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		rectTransform.SetAsLastSibling();
	}
}