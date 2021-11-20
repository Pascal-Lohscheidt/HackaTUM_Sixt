using UnityEngine;

public class NetworkNode
{
    public int Index { get; }
    public Vector3 Position { get; set;}
    
    public NetworkNode(int index, Vector3 position)
    {
        Index = index;
        Position = position;
    }
}
