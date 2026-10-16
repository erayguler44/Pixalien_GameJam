//Implemented using Gemini

using UnityEngine;

public class GlowOnHover : MonoBehaviour
{
    private Renderer rend;
    
    [Tooltip("Choose the glow color in the inspector")]
    public Color glowColor = Color.yellow; 
    
    private Color originalEmission;

    void Start()
    {
        rend = GetComponent<Renderer>();
        // Tells the material that we want to use the Emission property
        rend.material.EnableKeyword("_EMISSION");
        // Save the default color (which we set to black/off)
        originalEmission = rend.material.GetColor("_EmissionColor");
    }

    void OnMouseEnter()
    {
        // When the mouse pointer enters the collider, change the emission color
        rend.material.SetColor("_EmissionColor", glowColor * 2f); // Multiplied for extra brightness
    }

    void OnMouseExit()
    {
        // When the mouse leaves, revert to the original dark color
        rend.material.SetColor("_EmissionColor", originalEmission);
    }
}
