using Photon.Deterministic;
using Quantum;
using System.Linq;

public unsafe class SpatialHashSystem : SystemMainThreadFilter<SpatialHashSystem.Filter>
{
  public struct Filter { public EntityRef entity; public Ball* ball; public Transform3D* transform3d; }

  public override void OnInit(Frame f)
  {
    base.OnInit(f);
    f.Unsafe.GetPointerSingleton<SpatialHashTable>()->Grid = f.AllocateDictionary<Int3, Node>();
  }

  public override unsafe void Update(Frame f, ref Filter filt)
  {
    var table = f.Unsafe.GetPointerSingleton<SpatialHashTable>();
    var gridConfig = f.FindAsset<GridConfig>(table->GridConfig.Id);
    var grid = f.ResolveDictionary(table->Grid);
    var hashKey = Int3.QuantizeToInt3(filt.transform3d->Position, gridConfig.CellSize); 

    FP extents = gridConfig.CellSize / 2; FPVector3 boxExtents = new(extents, extents, extents);
    var prevPosition = filt.ball->PrevPosition;

    Input* i = f.GetPlayerInput(filt.ball->Owner); 

    if (!i->Query.IsDown)
    { 
      if (gridConfig.GridBoxes)
      {
        // Convert to FPVector3 for drawing
        Draw.Box(f.Unpack(hashKey) * gridConfig.CellSize, boxExtents, null, gridConfig.CurrentNode);
        Draw.Box(f.Unpack(hashKey) * gridConfig.CellSize, boxExtents * 3, null, gridConfig.Neighborhood);
      }

      // Initialize a node at quantized position
      if (!grid.TryGetValue(hashKey, out Node newNode))
      {
        if (grid.Count < 64)
        {
          // Node struct holds the hash set
          newNode = new() { Entities = f.AllocateHashSet<EntityRef>() };

          // Add this entity to the node, and node to the grid, using the position as key
          if (f.TryResolveHashSet(newNode.Entities, out var newSet)) { newSet.Add(filt.entity); grid.Add(hashKey, newNode); }
        }
        else if (f.TryResolveHashSet(filt.ball->Neighbors, out var oldNeighbors)) { oldNeighbors.Clear(); }
      }

      // Remove from previous node
      if (grid.TryGetValue(prevPosition, out Node deadCheck) && hashKey != prevPosition && f.TryResolveHashSet(deadCheck.Entities, out var prevNode))
      {
        prevNode.Remove(filt.entity); 

        // Clear this balls neighbors list to rebuild
        if (f.TryResolveHashSet(filt.ball->Neighbors, out var oldNeighbors)) { oldNeighbors.Clear(); }

        // Clean up if empty
        if (prevNode.Count == 0 && f.TryFreeHashSet(deadCheck.Entities)) { deadCheck.Entities = default; grid.Remove(prevPosition); }
      }

      // Add entity to an active cell
      if (grid.TryGetValue(hashKey, out Node activeNode) && f.TryResolveHashSet(activeNode.Entities, out var activeSet) && !activeSet.Contains(filt.entity) && activeSet.Count < activeSet.Capacity)
      {
        activeSet.Add(filt.entity);
      }

      var range = Enumerable.Range(-1, 3);
      Int3[] neighborOffsets = (from x in range from y in range from z in range select new Int3((short)x, (short)y, (short)z)).ToArray();

      foreach (var offset in neighborOffsets)
      {
        var neighborKey = hashKey + offset;

        // Get neighbor nodesets and check for entities
        if (grid.TryGetValue(neighborKey, out Node hood) && f.TryResolveHashSet(hood.Entities, out var entities) && entities.Count >= 0)
        {
          foreach (var otherEntity in entities)
          {
            if (f.Unsafe.TryGetPointer(otherEntity, out Ball* otherAgent) &&
              otherEntity.Index != filt.entity.Index && f.TryResolveHashSet(filt.ball->Neighbors, out var neighbors) &&
              !neighbors.Contains(otherEntity) && neighbors.Count < 32)
            {
              neighbors.Add(otherEntity);
            }
            else break;
          }
        }
        // Update previous position for next cleanup 
        filt.ball->PrevPosition = hashKey;
      }
    }
    else
    {
      // Remove old entries when not querying
      if (f.TryResolveHashSet(filt.ball->Neighbors, out var oldNeighbors)) { oldNeighbors.Clear(); }
      if (grid.TryGetValue(hashKey, out Node cutoffNode) && f.TryFreeHashSet(cutoffNode.Entities)) { cutoffNode.Entities = default; grid.Remove(hashKey); }
    } 

    // Free hash set and destroy entity when out of bounds
    if (filt.transform3d->Position.Y < -1)
    { 
      if (grid.TryGetValue(hashKey, out Node deadNode) && f.TryFreeHashSet(deadNode.Entities)) { deadNode.Entities = default; grid.Remove(hashKey); }
      f.Destroy(filt.entity);
    }
  }
}