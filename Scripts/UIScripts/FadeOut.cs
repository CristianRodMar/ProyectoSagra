using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeOut : MonoBehaviour
{
    public Image image; // Referencia al componente Image del objeto
    public float initialDelay = 0.5f; // Retardo inicial antes de que comience el fade out
    public float fadeDuration = 4.0f; // Duración de la transición de fade out

    private void OnEnable()
    {
        StartCoroutine(FadeOutCoroutine());
    }
    IEnumerator FadeOutCoroutine()
    {
        yield return new WaitForSeconds(initialDelay); // Esperar medio segundo antes de iniciar el fade out
        Color startColor = image.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0f); // Color transparente
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            float t = elapsedTime / fadeDuration;
            image.color = Color.Lerp(startColor, endColor, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        image.color = endColor; // Asegurarse de que el objeto sea completamente transparente al final del fade out
    }
}

