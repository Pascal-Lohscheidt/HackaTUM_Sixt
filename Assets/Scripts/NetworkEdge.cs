public struct NetworkEdge
{
    public NetworkNode NodeA { get; }
    public NetworkNode NodeB { get; }
    public float Distance { get; set; }
    public float ElectricityCost { get; set; }
    
    public NetworkEdge(NetworkNode nodeA, NetworkNode nodeB, float distance, float electricityCost)
    {
        NodeA = nodeA;
        NodeB = nodeB;
        Distance = distance;
        ElectricityCost = electricityCost;
    }
}
