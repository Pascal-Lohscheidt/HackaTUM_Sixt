using System;
using System.Collections.Generic;
using UnityEngine;

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
    
    private Dictionary<NetworkEdge, GameObject> highlightedEdges = new Dictionary<NetworkEdge, GameObject>();
    private Dictionary<NetworkEdge, int> highlightedEdgesTokens = new Dictionary<NetworkEdge, int>();
    private Dictionary<NetworkNode, GameObject> highlightedNodes = new Dictionary<NetworkNode, GameObject>();
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

    public void ResetHighlight(NetworkEdge[,] graph)
    {
        foreach (GameObject node in highlightedNodes.Values)
        {
            MeshRenderer renderer = node.GetComponent<MeshRenderer>();
            renderer.material.color = standardNodeMaterial.color;
        }
        
        foreach (NetworkEdge edge in highlightedEdges.Keys)
        {
            LineRenderer renderer = highlightedEdges[edge].GetComponent<LineRenderer>();
            renderer.material.color = Color.HSVToRGB(0.3f, 1, 1 / (graph[edge.NodeA.Index, edge.NodeB.Index].Distance * 0.5f));
        }
    }

    public void HighlightPath(NetworkEdge[] edges)
    {
        AdjustColorOfNode(edges[0].NodeA, Color.green);
        
        if(!highlightedNodes.ContainsKey(edges[0].NodeA))
            highlightedNodes.Add(edges[0].NodeA, nodeRepresentations[edges[0].NodeA]);
        
        for (int i = 0; i < edges.Length; i++)
        {
            if (edges[i].Distance > 0)
            {
                AdjustColorOfEdge(edges[i], 3f);
                
                if(!highlightedEdges.ContainsKey(edges[i]))
                    highlightedEdges.Add(edges[i], edgeRepresentations[edges[i]]);
            }
        }
        AdjustColorOfNode(edges[edges.Length-1].NodeB, Color.red);
        if(!highlightedNodes.ContainsKey(edges[edges.Length-1].NodeB))
            highlightedNodes.Add(edges[edges.Length-1].NodeB, nodeRepresentations[edges[edges.Length-1].NodeB]);
    }

    private void UpdateHighlightState()
    {
        for (int i = 0; i < highlightedEdgesTokens.Keys.Count; i++)
        {
            
        }
    } 

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
