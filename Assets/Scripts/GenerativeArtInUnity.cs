using UnityEngine;

public class GenerativeArtInUnity : MonoBehaviour
{
    public int numCircles = 100;
    public float maxRadius = 0.5f;

    void Start()
    {
        for (int i = 0; i < numCircles; i++)
        {
            GameObject circle = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            circle.transform.position = new Vector3(Random.Range(-5.0f, 5.0f), Random.Range(-5.0f, 5.0f), 0);
            float radius = Random.Range(0.1f, maxRadius);
            circle.transform.localScale = new Vector3(radius, radius, radius);
            Renderer renderer = circle.GetComponent<Renderer>();
            renderer.material.color = new Color(Random.value, Random.value, Random.value);
        }
    }
}
