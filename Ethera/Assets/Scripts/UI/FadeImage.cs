using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeImage : MonoBehaviour
{
	[SerializeField] private Image fadeImage;
	[SerializeField] private float fadeDuration = 2f;
	
	private void Start()
	{
		StartCoroutine(FadeOutImage());
	}
	
	private void SetImageAlpha(float alpha)
	{
		Color color = fadeImage.color;
		color.a = alpha;
		fadeImage.color = color;
	}

	// Coroutine fade Alpha from 1 to 0
	private IEnumerator FadeOutImage()
	{
		float elapsedTime = 0f;

		while (elapsedTime < fadeDuration)
		{
			float newAlpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
			SetImageAlpha(newAlpha);

			elapsedTime += Time.deltaTime;
			yield return null;
		}

		// Make sure alpha = 0
		SetImageAlpha(0f);

		// Set active (false)
		fadeImage.gameObject.SetActive(false);
	}
	
	// Coroutine for Fade Alpha from 0 to 1
	private IEnumerator FadeInImage()
	{
		float elapsedTime = 0f;

		// Make sure Image start Alpha = 0 and set active true again
		SetImageAlpha(0f);
		fadeImage.gameObject.SetActive(true);

		while (elapsedTime < fadeDuration)
		{
			float newAlpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
			SetImageAlpha(newAlpha);

			elapsedTime += Time.deltaTime;
			yield return null;
		}

		// Ensure alpha = 1
		SetImageAlpha(1f);
	}
}