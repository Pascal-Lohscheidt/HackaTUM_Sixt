using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains the current state (network and cars)
/// </summary>
public class NetworkController : MonoBehaviour
{
    [SerializeField] private int minAmountOfNodes;
    [SerializeField] private int maxAmountOfNodes;
    [SerializeField] private float edgeDensity;
    [SerializeField] private int seed;
    
    [SerializeField] private int minDistance;
    [SerializeField] private int maxDistance;
    [SerializeField] private int minElectricityCost;
    [SerializeField] private int maxElectricityCost;

    private NetworkNode[] nodes;
    private NetworkEdge[,] graph;

    
    
    public void GenerateNewNetwork()
    {
        // 1) Generate Seed and Random object
        if (seed == 0) seed = UnityEngine.Random.Range(0, 40000);
        var random = new System.Random(seed);
        
        // 2) Generate Nodes
        nodes = new NetworkNode[random.Next(minAmountOfNodes, maxAmountOfNodes + 1)];

        for (var index = 0; index < nodes.Length; index++)
        {
            // TODO: Add random position if needed
            nodes[index] = new NetworkNode(index, Vector2.zero);
        }
        
        // 3) populate graph 
        graph = new NetworkEdge[nodes.Length, nodes.Length];
        
        // DFS like Generation! 

        Dictionary<int, int> nodeEdgeCount = new Dictionary<int, int>();
        for (int i = 0; i < nodes.Length; i++) nodeEdgeCount[i] = 0; // Populate the dictonary;
        
        PopulateRecursively(0, random, nodeEdgeCount);

        /*for (int i = 0; i < nodes.Length; i++)
        {
            for (int j = 0; j < nodes.Length; j++)
            {
                Debug.Log(graph[i, j].NodeA.Index + " -> " + graph[i, j].NodeB.Index + ": " + graph[i, j].Distance);
            }
        }*/
    }

    private void PopulateRecursively(int node, System.Random random, Dictionary<int, int> nodeEdgeCount)
    {
        if (node < graph.GetLength(0) - 1)
        {
            PopulateRecursively(node + 1, random, nodeEdgeCount);
        }

        for (int i = 0; i < graph.GetLength(0); i++)
        {
            if (graph[node, i].Distance == 0)
            {
                if (edgeDensity >= (float)random.NextDouble() && node != i &&
                    nodeEdgeCount[i] < 4 && nodeEdgeCount[node] < 4)
                {
                    float newDistance = minDistance + (float)random.NextDouble() * maxDistance;
                    float newElectricityCost = minElectricityCost + (float)random.NextDouble() * maxElectricityCost;
                    NetworkEdge edge = 
                    graph[node, i] = new NetworkEdge(nodes[node], nodes[i], newDistance, newElectricityCost);
                    graph[i, node] = new NetworkEdge(nodes[i], nodes[node], newDistance, newElectricityCost);
                    
                    // Making sure it doesn't have more than 4 edges
                    nodeEdgeCount[node] += 1;
                    nodeEdgeCount[i] += 1;
                }
                else
                {
                    graph[node, i] = new NetworkEdge(nodes[node], nodes[i], 0, 0);
                }
            }
        }


    }
    
    
    public NetworkEdge[,] GetGraph() => graph;

    public NetworkNode[] GetNodes() => nodes;
    
    /// <summary>
    /// Get Distance with 0(1)
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public float GetDistance(NetworkNode a, NetworkNode b) => graph[a.Index, b.Index].Distance;
    
    /// <summary>
    ///  Get Electricity Cost with 0(1)
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public float GetElectricityCost(NetworkNode a, NetworkNode b) => graph[a.Index, b.Index].ElectricityCost;

}

/*

public delegate void GenerateGraph(
        int seed, 
        NetworkNode[] nodes, 
        float minAmountOfNodes, 
        float maxAmountOfNodes,
        NetworkEdge[,] graph,
        float edgeDensity,
        float minDistance,
        float maxDistance,
        float minElectricityCost,
        float maxElectricityCost
    );

    private GenerateGraph generationA = (
        seed, 
        nodes, 
        minAmountOfNodes, 
        maxAmountOfNodes, 
        graph, 
        edgeDensity, 
        minDistance,
        maxDistance,
        minElectricityCost,
        maxElectricityCost) =>
    {
        // 1) Generate Seed and Random object
        if (seed == 0) seed = UnityEngine.Random.Range(0, 40000);
        var random = new System.Random(seed);
        
        // 2) Generate Nodes
        nodes = new NetworkNode[random.Next(minAmountOfNodes, maxAmountOfNodes + 1)];

        for (var index = 0; index < nodes.Length; index++)
        {
            // TODO: Add random position if needed
            nodes[index] = new NetworkNode(index, Vector2.zero);
        }
        
        // 3) populate graph 
        graph = new NetworkEdge[nodes.Length, nodes.Length];
        
        for (var i = 0; i < nodes.Length; i++)
        {
            for (var z = 0; z < nodes.Length; z++)
            {
                if (z == i)
                {
                    graph[i, z] = new NetworkEdge(nodes[i], nodes[z], 0, 0);
                    continue; // skip -> no reflexive edges!
                }

                if (edgeDensity >= (float)random.NextDouble()) // => edge density 1 => complete graph
                {
                    float newDistance = minDistance + (float)random.NextDouble() * maxDistance;
                    float newElectricityCost = minElectricityCost + (float)random.NextDouble() * maxElectricityCost;
                    graph[i, z] = new NetworkEdge(nodes[i], nodes[z], newDistance, newElectricityCost);
                }
                else // no edge
                {
                    graph[i, z] = new NetworkEdge(nodes[i], nodes[z], 0, 0);
                }
            }
        }

        for (int i = 0; i < nodes.Length; i++)
        {
            for (int j = 0; j < nodes.Length; j++)
            {
                Debug.Log(graph[i, j].NodeA.Index + " -> " + graph[i, j].NodeB.Index + ": " + graph[i, j].Distance);
            }
        }
    };
 */
