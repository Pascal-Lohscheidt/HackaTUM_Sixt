using System;
using UnityEngine;

public class SimulationController : MonoBehaviour
{
    public static float timeScale;
    [SerializeField] private NetworkController networkController;
    [SerializeField] private GraphVisualizer graphVisualizer;

    private void Start()
    {
        networkController.GenerateNewNetwork();
        graphVisualizer.CreateGraphVisualization(networkController.GetGraph());
    }

    public void StartSimulation()
    {
        Debug.Log("Start Simulation!");
    }

    public void PauseSimulation()
    {
        Debug.Log("Pause Simulation!");
    }

    public void EndSimulation()
    {
        
    }
}
