using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SixtCar
{
    public int Index { get; }
    public NetworkNode CurrentNode { get; set; }
    public BookingData AssignedBooking { get; set; }
    private NetworkEdge[] path;
    private int currentEdge;
    private bool customerOnBoard = false;

    public SixtCar(int index, NetworkNode networkNode)
    {
        Index = index;
        CurrentNode = networkNode;
        RegisterForBooking();
    }

    private void RegisterForBooking()
    {
        CarController.Instance.ReceivedNewBooking += OnNewBookingAvailable;
    }

    private void UnregisterFromBooking()
    {
        CarController.Instance.ReceivedNewBooking -= OnNewBookingAvailable;
    }

    private void OnNewBookingAvailable()
    {
        AssignedBooking = BookingController.Instance.GetBookingEntry();
        if (AssignedBooking != null) StartCar();
    }

    private void StartCar()
    {
        currentEdge = 0;
        UnregisterFromBooking();
        CarController.Instance.ExecuteCarLogic += Execute;
        NetworkController networkController = NetworkController.Instance;
        path = ShortestPathCalculator.SolveShortestPathProblem(
            networkController.GetGraph(),
            (edge) => edge.Distance,
            CurrentNode,
            networkController.GetNodes()[AssignedBooking.StartNode]);
        SimulationVisualizer.Instance.ResetHighlight(networkController.GetGraph());
        SimulationVisualizer.Instance.HighlightPath(path);
        customerOnBoard = false;
    }
    
    private void Execute()
    {
        Debug.Log("Car at node: " + CurrentNode.Index);
        if (path != null)
        {
            if (currentEdge < path.Length - 1)
            {
                currentEdge++;
                CurrentNode = path[currentEdge].NodeA;
            }
            else
            {
                CurrentNode = path[currentEdge].NodeB;
                ReachedEnd();
            }
        }
        else
        {
            ReachedEnd();
        }
    }

    private void ReachedEnd()
    {
        currentEdge = 0;
        if (customerOnBoard)
        {
            AssignedBooking.TimeWhenArrived = Time.time;
            BookingController.Instance.CompleteBooking(AssignedBooking);
            AssignedBooking = null;
            CarController.Instance.ExecuteCarLogic -= Execute;
            RegisterForBooking();
            OnNewBookingAvailable();
            path = null;
        }
        else
        {
            customerOnBoard = true;
            NetworkController networkController = NetworkController.Instance;
            path = ShortestPathCalculator.SolveShortestPathProblem(
                networkController.GetGraph(),
                (edge) => edge.Distance,
                CurrentNode,
                networkController.GetNodes()[AssignedBooking.EndNode]);
            
            SimulationVisualizer.Instance.ResetHighlight(networkController.GetGraph());
            SimulationVisualizer.Instance.HighlightPath(path);
        }
    }
    
}
