// Created by robertbu & CallToAdventure on https://answers.unity.com/questions/443031/sinus-for-rolling-waves.html

using UnityEngine;
using System.Collections;

public class PlaneWaving : MonoBehaviour
{
    [SerializeField] private float scale = 0.1f;        // Größe/Höhe
    [SerializeField] private float speed = 1.0f;        // Geschwndigkeit
    [SerializeField] private float noiseStrength = 1f;  // Wellenstärke
    [SerializeField] private float noiseWalk = 1f;      // Wellenlänge

    private Vector3[] baseHeight;

    void Update()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;

        if (baseHeight == null)
            baseHeight = mesh.vertices;

        Vector3[] vertices = new Vector3[baseHeight.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 vertex = baseHeight[i];
            vertex.y += Mathf.Sin(Time.time * speed + baseHeight[i].x + baseHeight[i].y + baseHeight[i].z) * scale;
            vertex.y += Mathf.PerlinNoise(baseHeight[i].x + noiseWalk, baseHeight[i].y + Mathf.Sin(Time.time * 0.1f)) * noiseStrength;
            vertices[i] = vertex;
        }
        mesh.vertices = vertices;
        mesh.RecalculateNormals();
    }
}