using System.Collections;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public float explosionRadius = 10f;  // Maximum explosion radius
    public float safeRadius = 3f;        // Inner safe radius (no force applied)
    public float explosionForce = 1000f; // Strength of the explosion
    public float upwardsModifier = 2f;   // Extra lift effect
    public LayerMask carLayer;           // Layer mask for detecting cars

    private bool hasExploded = false;    // Prevent multiple explosions

    public void TriggerExplosion()
    {
        if (hasExploded) return;  // Prevent multiple explosions
        hasExploded = true;       // Mark explosion as triggered

        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius, carLayer);
        
        foreach (Collider col in colliders)
        {
            if (col.CompareTag("Car") && col.gameObject != gameObject) // Ignore the car causing the explosion
            {
                Rigidbody rb = col.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    float distance = Vector3.Distance(transform.position, col.transform.position);

                    if (distance > safeRadius) // Ignore cars inside the safe zone
                    {
                        Vector3 explosionDir = (col.transform.position - transform.position).normalized;

                        // Apply same force to all affected cars
                        rb.AddExplosionForce(explosionForce, transform.position, explosionRadius, upwardsModifier, ForceMode.Impulse);
                    }
                }
            }
        }

        StartCoroutine(ResetExplosion(0.1f)); // Reset explosion after 0.1 seconds
    }

    private IEnumerator ResetExplosion(float delay)
    {
        yield return new WaitForSeconds(delay);
        hasExploded = false; // Allow explosion to be triggered again
    }
}
