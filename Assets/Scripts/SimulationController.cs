using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class SimulationController : Singleton<SimulationController>
{
    public static float timeScale;
    private bool generatedCars = false;
    private void Start()
    {
        RegenerateSimulationState();
    }

    public void RegenerateSimulationState()
    {
        NetworkController networkController = NetworkController.Instance;
        networkController.GenerateNewNetwork();
        SimulationVisualizer.Instance.CreateSimulationState(
            networkController.GetGraph(), 
            networkController.GetNodes(), 
            networkController.graphWidth
            );
    }
    
    public void StartSimulation()
    {
        Debug.Log("Start Simulation!");
        if (!generatedCars)
        {
            generatedCars = true;
            CarController.Instance.GenerateCars();
        }
        timeScale = 1;
    }

    public void PauseSimulation()
    {
        Debug.Log("Pause Simulation!");
        timeScale = 0;
    }
    public void SpawnBooking()
    {
        BookingController.Instance.MakeBooking();
    }

    public void EndSimulation()
    {
        
    }
}
