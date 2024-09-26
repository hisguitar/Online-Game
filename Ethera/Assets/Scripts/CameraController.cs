using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	[SerializeField] private CinemachineVirtualCamera virtualCamera; // Reference to the Cinemachine Virtual Camera in player
	[SerializeField] private float minZoom = 2f;
	[SerializeField] private float maxZoom = 5f;
	[SerializeField] private float zoomSpeed = 1f;
	[SerializeField] private float smoothSpeed = 5f; // Speed of smooth zooming

	private float targetZoom; // Target zoom value

	private void Start()
	{
		// Set the initial target zoom to 5
		targetZoom = 5f;

		// Check if the camera is Perspective or Orthographic
		if (virtualCamera.m_Lens.Orthographic)
		{
			// If it is an Orthographic camera, set the OrthographicSize
			virtualCamera.m_Lens.OrthographicSize = Mathf.Clamp(targetZoom, minZoom, maxZoom);
		}
		else
		{
			// If it is a Perspective camera, set the FieldOfView
			virtualCamera.m_Lens.FieldOfView = Mathf.Clamp(targetZoom, minZoom, maxZoom);
		}
	}

	private void Update()
	{
		// Get input from the mouse scroll wheel
		float scrollInput = Input.GetAxis("Mouse ScrollWheel");

		if (scrollInput != 0f)
		{
			// Check if the camera is Perspective or Orthographic
			if (virtualCamera.m_Lens.Orthographic)
			{
				// Adjust the target zoom for Orthographic camera
				targetZoom -= scrollInput * zoomSpeed;
				targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
			}
			else
			{
				// Adjust the target Field of View for Perspective camera
				targetZoom -= scrollInput * zoomSpeed;
				targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
			}
		}

		// Smoothly interpolate the zoom value towards the target value using Lerp
		if (virtualCamera.m_Lens.Orthographic)
		{
			virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(virtualCamera.m_Lens.OrthographicSize, targetZoom, Time.deltaTime * smoothSpeed);
		}
		else
		{
			virtualCamera.m_Lens.FieldOfView = Mathf.Lerp(virtualCamera.m_Lens.FieldOfView, targetZoom, Time.deltaTime * smoothSpeed);
		}
	}
}