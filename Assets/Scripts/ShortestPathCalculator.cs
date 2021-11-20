using System.Collections.Generic;
using System.Linq;

/// <summary>
/// We went for a simple Dijkstra algorithm
/// </summary>
public class ShortestPathCalculator
{
    // This delegate let's you decide what generates the prioty queue
    public delegate float EdgeCost(NetworkEdge e);
    
    public static NetworkEdge[] SolveShortestPathProblem(NetworkEdge[,] graph, EdgeCost GetEdgeCost, NetworkNode entry, NetworkNode target)
    {
        List<DijkstraSolution> solutionQueue = new List<DijkstraSolution>();
        DijkstraSolution bestSolution = new DijkstraSolution();
        List<NetworkEdge> queue = new List<NetworkEdge>(); //NICE <.NET6 doesn't have priority queue! Thanks guys Thanks!?!?!?!

        DijkstraSolution startPoint = new DijkstraSolution(graph[entry.Index, entry.Index], 0);
        solutionQueue.Add(startPoint);

        int iterationBreak = 0;
        // Iterate
        
        while (solutionQueue.Count > 0)
        {
            DijkstraIteration(graph, solutionQueue, GetEdgeCost, out bestSolution);
            
            //Terminate early to reduce redundant calls => possible there are no negative edge costs
            if (bestSolution.LastNode().Index == target.Index &&
                solutionQueue.Count > 0 &&
                solutionQueue[0].GetSolutionCost() >= bestSolution.GetSolutionCost()) break; // Found a solution

            if (iterationBreak == 3000) break;
            iterationBreak++;
        }
        
        // Convert & return solution
        return bestSolution.GetNetworkEdges();
    }


    private static void DijkstraIteration(NetworkEdge[,] graph, List<DijkstraSolution> queue, EdgeCost GetEdgeCost,  out DijkstraSolution bestSolution)
    {
        // 1) get min
        DijkstraSolution minSolution = GetFirstInQueue(queue);
        
        // 2) Add all adjacent nodes
        for (int i = 0; i < graph.GetLength(0); i++)
        {
            NetworkEdge edge = graph[minSolution.LastNode().Index, i];
            float cost = GetEdgeCost(edge);
            if (cost > 0)
            {
                DijkstraSolution newSolution = minSolution.CopySolution();
                newSolution.AddNetworkEdge(edge, cost);
                queue.Add(newSolution);
            }
        }

        if (queue.Count > 0)
        {
            SortQueue(queue);    
        }
        
        bestSolution = minSolution;
    }

    private static void SortQueue(List<DijkstraSolution> queue)
    {
        queue.OrderBy(a => a.GetSolutionCost());
    }

    private static DijkstraSolution GetFirstInQueue(List<DijkstraSolution> edges)
    {
        if (edges.Count > 0)
        {
            DijkstraSolution solution = edges[0];
            edges.RemoveAt(0);
            return solution;
        }

        return new DijkstraSolution();
    }

    public struct DijkstraSolution
    {
        private List<NetworkEdge> networkEdges;
        private float solutionCost;

        public DijkstraSolution(NetworkEdge start, float startCost)
        {
            solutionCost = 0;
            networkEdges = new List<NetworkEdge>();
            AddNetworkEdge(start, startCost);
        }

        private DijkstraSolution(List<NetworkEdge> networkEdges, float solutionCost)
        {
            this.networkEdges = networkEdges;
            this.solutionCost = solutionCost;
        }

        public DijkstraSolution CopySolution() => new DijkstraSolution(networkEdges, solutionCost);

        public void AddNetworkEdge(NetworkEdge networkEdge, float cost)
        {
            networkEdges.Add(networkEdge);
            solutionCost += cost;
        }

        public float GetSolutionCost() => solutionCost;

        public NetworkEdge[] GetNetworkEdges() => networkEdges.ToArray();
        public NetworkNode LastNode() => networkEdges.Last().NodeB;
    }
    
    
    
    
}
