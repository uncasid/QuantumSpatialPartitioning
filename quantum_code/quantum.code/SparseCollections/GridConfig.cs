using Photon.Deterministic;
using Quantum.Inspector;

namespace Quantum
{
  partial class GridConfig
  {
    [Header("Partitioning")]
    public FP CellSize; 
    [Space(5)]
    public bool NetLines;
    public ColorRGBA NetColor; 
    [Space(5)]
    public bool GridBoxes;
    public ColorRGBA CurrentNode;
    public ColorRGBA Neighborhood;
  }
}