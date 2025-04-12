using System.Collections.Generic;
using UnityEngine;

public class SpawnerSpinner : MonoBehaviour
{
    public Vector2 orbitRadius = new Vector2(15f, 25f);
    public Vector2 minMaxOrbitSpeed = new Vector2(10f, 50f);

    private List<float> orbitSpeed = new List<float>();  
    private List<float> angle = new List<float>();                          
    private List<float> radius = new List<float>();                          
    private Vector3 orbitCentre;

    void Start()
    {
        orbitCentre = transform.position;

        foreach (Transform child in transform)
        {
            angle.Add(Random.Range(0f, Mathf.PI * 2f));
            orbitSpeed.Add(Random.Range(minMaxOrbitSpeed.x, minMaxOrbitSpeed.y) * Mathf.Deg2Rad);
            radius.Add(Random.Range(orbitRadius.x, orbitRadius.y));
        }

    }

    void Update()
    {
        Vector3 orbitOffset;
        float x;
        float y;

        for (int i = 0; i < transform.childCount; i++)
        {
            angle[i] += orbitSpeed[i] * Time.deltaTime;
            x = Mathf.Cos(angle[i]) * radius[i];
            y = Mathf.Sin(angle[i]) * radius[i];

            orbitOffset = new Vector2(x, y);
            transform.GetChild(i).position = orbitCentre + orbitOffset;
        }
    }
}
