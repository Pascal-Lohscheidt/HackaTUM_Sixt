using System.Collections.Generic;
using UnityEngine;

public class SimulationVisualizer : MonoBehaviour
{
    [SerializeField] private GameObject nodePrefab;
    [SerializeField] private GameObject lineRendererPrefab;
    [SerializeField] private Material standardNodeMaterial;

    [SerializeField] private float minPos;
    [SerializeField] private float maxPos;

    private Dictionary<NetworkNode, GameObject> nodeRepresentations = new Dictionary<NetworkNode, GameObject>();
    private Dictionary<NetworkEdge, GameObject> edgeRepresentations = new Dictionary<NetworkEdge, GameObject>();
    
    private Dictionary<NetworkEdge, GameObject> highlightedEdges = new Dictionary<NetworkEdge, GameObject>();
    private Dictionary<NetworkNode, GameObject> highlightedNodes = new Dictionary<NetworkNode, GameObject>();
    public void CreateSimulationState(NetworkEdge[,] graph, NetworkNode[] nodes)
    {
        DeleteOldVisualization();
        SpawnNodes(nodes);
        SpawnEdges(graph);
    }

    private void SpawnNodes(NetworkNode[] nodes)
    {
        for (int i = 0; i < nodes.Length; i++)
        {
            Vector3 position = new Vector3(Random.Range(minPos, maxPos), 0, Random.Range(minPos, maxPos));
            GameObject newNode = Instantiate(nodePrefab, position, Quaternion.identity);
            nodeRepresentations.Add(nodes[i], newNode);
            newNode.GetComponent<VisualizedNode>().Node = nodes[i];
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
                    GameObject edgeRepresentation = Instantiate(lineRendererPrefab, Vector3.zero, Quaternion.identity);
                    
                    edgeRepresentations.Add(edge, edgeRepresentation);
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
        highlightedNodes.Add(edges[0].NodeA, nodeRepresentations[edges[0].NodeA]);
        for (int i = 0; i < edges.Length; i++)
        {
            if(edges[i].Distance > 0)
                AdjustColorOfEdge(edges[i], 3f);
        }
        AdjustColorOfNode(edges[edges.Length-1].NodeB, Color.red);
        highlightedNodes.Add(edges[edges.Length-1].NodeB, nodeRepresentations[edges[edges.Length-1].NodeB]);
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
