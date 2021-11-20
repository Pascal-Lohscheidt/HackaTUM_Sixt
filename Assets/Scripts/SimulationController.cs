using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class SimulationController : MonoBehaviour
{
    public static float timeScale;
    [SerializeField] private NetworkController networkController;
    [SerializeField] private GraphVisualizer graphVisualizer;
    [SerializeField] private SimulationVisualizer simulationVisualizer;
    
    private void Start()
    {
        
    }

    public void RegenerateSimulationState()
    {
        networkController.GenerateNewNetwork();
        simulationVisualizer.CreateSimulationState(networkController.GetGraph(), networkController.GetNodes());

        // Get shortest route test
        NetworkEdge[] solution = ShortestPathCalculator
            .SolveShortestPathProblem(networkController.GetGraph(),
                (edge) => edge.Distance,
                networkController.GetNodes()[2],
                networkController.GetNodes()[5]);
        
        Debug.Log(solution.Length);
        String path = "Path is: ";
        for (int i = 0; i < solution.Length; i++)
        {
            path += solution[i].NodeA.Index + " (C: " + solution[i].Distance + ") - ";
        }

        path += solution[solution.Length - 1].NodeB.Index;
        Debug.Log(path);
        
        simulationVisualizer.HighlightPath(solution);
    }
    
    public void StartSimulation()
    {
        Debug.Log("Start Simulation!");
        RegenerateSimulationState();
    }

    public void PauseSimulation()
    {
        Debug.Log("Pause Simulation!");
    }

    public void EndSimulation()
    {
        
    }
}
