using UnityEngine;
using System.Collections;
using TMPro;

public class LoadingAnimation : MonoBehaviour
{
    public TMP_Text loadingText;

    private void Start()
    {
        StartCoroutine(AnimateText());
    }

    IEnumerator AnimateText()
    {
        while (true)
        {
            loadingText.text = "Loading";
            yield return new WaitForSeconds(0.5f);
            loadingText.text = "Loading.";
            yield return new WaitForSeconds(0.5f);
            loadingText.text = "Loading..";
            yield return new WaitForSeconds(0.5f);
            loadingText.text = "Loading...";
            yield return new WaitForSeconds(0.5f);
        }
    }
}