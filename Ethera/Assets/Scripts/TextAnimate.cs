using UnityEngine;
using System.Collections;
using TMPro;

public class TextAnimate : MonoBehaviour
{
    [SerializeField] private TMP_Text textAnimate;
    [SerializeField] [Multiline] private string[] text;

    private void Start()
    {
        StartCoroutine(AnimateText());
    }

    private IEnumerator AnimateText()
    {
        while (true)
        {
            for (int i = 0; i < text.Length; i++)
            {
                textAnimate.text = text[i];
                yield return new WaitForSeconds(0.5f);
            }
        }
    }
}