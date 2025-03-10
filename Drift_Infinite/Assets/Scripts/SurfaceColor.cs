using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfaceColor : MonoBehaviour
{
    public Renderer targetRenderer; // Object whose material will be affected
    public Color highlightColor = Color.red;
    public float maskRadius = 1.5f;

    private Material material;

    private void Start()
    {
        if (targetRenderer == null) return;

        // Get the material instance
        material = targetRenderer.material;
        material.SetColor("_HighlightColor", highlightColor);
        material.SetFloat("_MaskRadius", maskRadius);
    }

    private void Update()
    {
        if (material == null) return;

        // Update shader with this object's position
        material.SetVector("_MaskPosition", transform.position);
    }
}
