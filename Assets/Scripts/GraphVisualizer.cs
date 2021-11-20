using System;
using System.Collections.Generic;
using UnityEngine;

public class GraphVisualizer : MonoBehaviour
{
    [SerializeField] private GameObject graphNodePrefab;
    [SerializeField] private float gridSpacing;
    private List<GameObject> spawnedNodes = new List<GameObject>();

    public void CreateGraphVisualization(NetworkEdge[,] graph)
    {
        ClearVisualization();
        
        // based on graph size create an offset
        Vector3 baseOffset = new Vector3((graph.GetLength(0) * gridSpacing) / 2, 0, (graph.GetLength(0) * gridSpacing) / 2);
        
        for (int i = 0; i < graph.GetLength(0); i++)
        {
            for (int j = 0; j < graph.GetLength(0); j++)
            {
                Color color = graph[i, j].Distance > 0 ? Color.green : Color.red;
                SpawnNode(color, new Vector3(i * gridSpacing, 0, j * gridSpacing) - baseOffset);
            }
        }
    }

    public void UpdateGraphVisualization()
    {
        throw new NotImplementedException();
    }

    public void ClearVisualization()
    {
        if (spawnedNodes.Count > 0)
        {
            for (int i = 0; i < spawnedNodes.Count; i++)
                Destroy(spawnedNodes[i]);
            spawnedNodes.Clear();
        }
    }

    private void SpawnNode(Color color, Vector3 position)
    {
        GameObject newNode = Instantiate(graphNodePrefab, position, Quaternion.identity);
        spawnedNodes.Add(newNode);

        newNode.GetComponent<MeshRenderer>().material.color = color;
    }

}
