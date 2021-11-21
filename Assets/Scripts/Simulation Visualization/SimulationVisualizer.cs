using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class SimulationVisualizer : Singleton<SimulationVisualizer>
{
    [SerializeField] private GameObject nodePrefab;
    [SerializeField] private GameObject lineRendererPrefab;
    [SerializeField] private Material standardNodeMaterial;

    [SerializeField] private float minPos;
    [SerializeField] private float maxPos;
    [SerializeField] private float nodeDistance;
    [SerializeField] private Vector3 startOffset;

    private Dictionary<NetworkNode, GameObject> nodeRepresentations = new Dictionary<NetworkNode, GameObject>();
    private Dictionary<NetworkEdge, GameObject> edgeRepresentations = new Dictionary<NetworkEdge, GameObject>();
    
    private Dictionary<NetworkEdge, int> highlightedEdgesTokens = new Dictionary<NetworkEdge, int>();
    private Dictionary<NetworkNode, int> highlightedNodesTokens = new Dictionary<NetworkNode, int>();
    public void CreateSimulationState(NetworkEdge[,] graph, NetworkNode[] nodes, int graphWidth)
    {
        DeleteOldVisualization();
        SpawnNodes(nodes, graphWidth);
        SpawnEdges(graph);
    }
    
    private void SpawnNodes(NetworkNode[] nodes, int graphWidth)
    {
        Vector3 currentPosition = startOffset;
        for (int i = 0; i < nodes.Length; i++)
        {
            GameObject newNode = Instantiate(nodePrefab, currentPosition, Quaternion.identity);
            nodeRepresentations.Add(nodes[i], newNode);
            newNode.GetComponent<VisualizedNode>().Node = nodes[i];

            float direction = (int)Math.Floor((float)i / graphWidth) % 2 == 0 ? 1 : -1; 
            float x = ((i + 1) % graphWidth) == 0 ? 0 : 1;
            float z = ((i + 1) % graphWidth) == 0 ? 1 : 0;
            currentPosition += new Vector3(x * direction, 0, z) * nodeDistance;
        }
    }

    private void SpawnEdges(NetworkEdge[,] graph)
    {
        for (int i = 0; i < graph.GetLength(0); i++)
        {
            for (int j = 0; j < graph.GetLength(0); j++)
            {
                if (graph[i, j].Distance > 0 && !edgeRepresentations.ContainsKey(graph[i, j]) && !edgeRepresentations.ContainsKey(graph[j, i]))
                {
                    NetworkEdge edge = graph[i, j];
                    NetworkEdge counterEdge = graph[j, i];
                    GameObject edgeRepresentation = Instantiate(lineRendererPrefab, Vector3.zero, Quaternion.identity);
                    
                    edgeRepresentations.Add(edge, edgeRepresentation);
                    edgeRepresentations.Add(counterEdge, edgeRepresentation);
                    LineRenderer renderer = edgeRepresentation.GetComponent<LineRenderer>();
                    
                    renderer.SetPositions(new Vector3[]{
                        nodeRepresentations[edge.NodeA].transform.position, 
                        nodeRepresentations[edge.NodeB].transform.position
                    });
                    renderer.material.color = Color.HSVToRGB(0.3f, 1, 1 / (graph[i, j].Distance * 0.5f));
                }
            }
        }
    }

    public void HighlightPath(NetworkEdge[] edges)
    {
        if (!highlightedNodesTokens.ContainsKey(edges[0].NodeA))
            highlightedNodesTokens.Add(edges[0].NodeA, 1);
        else
            highlightedNodesTokens[edges[0].NodeA] += 1;
        
        for (int i = 0; i < edges.Length; i++)
        {
            if (edges[i].Distance > 0)
            {
                if (!highlightedEdgesTokens.ContainsKey(edges[i]))
                    highlightedEdgesTokens.Add(edges[i], 1);
                else
                    highlightedEdgesTokens[edges[i]] += 1;
            }
        }
        
        if (!highlightedNodesTokens.ContainsKey(edges[edges.Length-1].NodeB))
            highlightedNodesTokens.Add(edges[edges.Length-1].NodeB, 1);
        else
            highlightedNodesTokens[edges[edges.Length-1].NodeB] += 1;
        
        UpdateHighlightState(NetworkController.Instance.GetGraph());
    }

    public void HidePath(NetworkEdge[] edges)
    {
        if (!highlightedNodesTokens.ContainsKey(edges[0].NodeA))
            highlightedNodesTokens.Add(edges[0].NodeA, 0);
        else
            highlightedNodesTokens[edges[0].NodeA] = Math.Max(0, highlightedNodesTokens[edges[0].NodeA] - 1);


        for (int i = 0; i < edges.Length; i++)
        {
            if (edges[i].Distance > 0)
            {
                if (!highlightedEdgesTokens.ContainsKey(edges[i]))
                    highlightedEdgesTokens.Add(edges[i], 0);
                else
                    highlightedEdgesTokens[edges[i]] = Math.Max(0, highlightedEdgesTokens[edges[i]] - 1);
            }
        }
        
        if (!highlightedNodesTokens.ContainsKey(edges[edges.Length-1].NodeB))
            highlightedNodesTokens.Add(edges[edges.Length-1].NodeB, 0);
        else
            highlightedNodesTokens[edges[edges.Length-1].NodeB] = Math.Max(0, highlightedNodesTokens[edges[edges.Length-1].NodeB] - 1);
        
        UpdateHighlightState(NetworkController.Instance.GetGraph());
    }
    
    private void UpdateHighlightState(NetworkEdge[,] graph)
    {
        foreach (NetworkEdge edge in highlightedEdgesTokens.Keys)
        {
            if (highlightedEdgesTokens[edge] > 0)
            {
                AdjustColorOfEdge(edge, 3f);
            }
            else
            {
                LineRenderer renderer = edgeRepresentations[edge].GetComponent<LineRenderer>();
                renderer.material.color = Color.HSVToRGB(0.3f, 1, 1 / (graph[edge.NodeA.Index, edge.NodeB.Index].Distance * 0.5f));
            }
        }
        
        foreach (NetworkNode node in highlightedNodesTokens.Keys)
        {
            if (highlightedNodesTokens[node] > 0)
            {
                AdjustColorOfNode(node, Color.green);
            }
            else
            {
                MeshRenderer renderer = nodeRepresentations[node].GetComponent<MeshRenderer>();
                renderer.material.color = standardNodeMaterial.color;
            }
        }
    }

    public Dictionary<NetworkNode, GameObject> GetNodeRepresentations() => nodeRepresentations;

    private void DeleteOldVisualization()
    {
        foreach (GameObject node in nodeRepresentations.Values)
            Destroy(node);
        
        foreach (GameObject edge in edgeRepresentations.Values)
            Destroy(edge);
        
        nodeRepresentations.Clear();
        edgeRepresentations.Clear();
    }

    private void AdjustColorOfNode(NetworkNode node, Color color)
    {
        GameObject nodeRepresentation = nodeRepresentations[node];
        nodeRepresentation.GetComponent<MeshRenderer>().material.color = color;
    }

    private void AdjustColorOfEdge(NetworkEdge edge, float intensity)
    {
        GameObject edgeRepresentation = edgeRepresentations[edge];
        LineRenderer renderer = edgeRepresentation.GetComponent<LineRenderer>();
        renderer.material.color = Color.yellow * intensity;
    }
 }
