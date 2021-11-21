using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookingData
{
    public int StartNode { get;  }
    public int EndNode { get;  }
    
    public float TimeWhenBooked { get; }
    public float TimeWhenArrived { get; set; }

    public BookingData(int startNode, int endNode, float timeWhenBooked)
    {
        StartNode = startNode;
        EndNode = endNode;
        TimeWhenBooked = timeWhenBooked;
    }
}
