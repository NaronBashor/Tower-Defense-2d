using UnityEngine;
using TMPro;

public class FloatingDamageText : MonoBehaviour
{
    public TextMeshProUGUI textMesh; // Reference to TextMeshPro component
    public float moveSpeed = 1f; // Speed at which the text floats upward
    public float fadeDuration = 1f; // Time for the text to fade out
    private Color originalColor;
    private float timer;

    private void Start()
    {
        // Set the initial color
        originalColor = textMesh.color;
        Destroy(gameObject, fadeDuration); // Destroy after fade duration
        timer = fadeDuration;
    }

    private void Update()
    {
        // Move the text upward in world space
        transform.position += Vector3.up * moveSpeed * Time.deltaTime;

        // Gradually fade out the text
        timer -= Time.deltaTime;
        float fadeAmount = Mathf.Clamp01(timer / fadeDuration);
        //textMesh.color = new Color(originalColor.r, originalColor.g, originalColor.b, fadeAmount);
    }

    public void SetText(string text, Color color)
    {
        textMesh.text = text;
        textMesh.color = color;
    }
}
