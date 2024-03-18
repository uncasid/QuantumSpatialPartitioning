// This is a singleton(dictionary(node-struct(hash-set))) sparse 3d grid partitioning system that i made while optimizing alongside Uncasid's BOIDS
// For my game ByteSect 🦗
// paranoidentity.itch.io/bytesect 
// pass: teameevee

using Photon.Deterministic;
using Quantum;
using System.Linq;
using static Quantum.Navigation.FindPathResult;

public unsafe class SpatialHashSystem : SystemMainThreadFilter<SpatialHashSystem.Filter>
{
    public struct Filter { public EntityRef entity; public DroneAgent* agent; }

    public override unsafe void Update(Frame f, ref Filter filt)
    {
        var table = f.Unsafe.GetPointerSingleton<SpatialHashTable>();
        var grid = f.ResolveDictionary(table->Grid);

        // Use QuantizeToInt3 from the Int3 Arithmetic struct :)
        var hashKey = filt.agent->Position;

        if (filt.agent->Flocking)
        {
            var flockConfig = f.FindAsset<FlockingSpec>(filt.agent->FlockingConfig.Id);

            FP extents = flockConfig.CellSize / 2; FPVector3 boxExtents = new(extents, extents, extents);
            var prevPosition = filt.agent->PrevPosition;

            if (flockConfig.GridBoxes)
            {
                Draw.Box(f.Unpack(filt.agent->Position) * flockConfig.CellSize, boxExtents, null, flockConfig.CurrentNode);
                Draw.Box(f.Unpack(filt.agent->Position) * flockConfig.CellSize, boxExtents * 3, null, flockConfig.Neighborhood);
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
                else if (f.TryResolveHashSet(filt.agent->Neighbors, out var oldNeighbors)) { oldNeighbors.Clear(); }
            }

            // Remove from previous node
            if (grid.TryGetValue(prevPosition, out Node deadCheck) && hashKey != prevPosition && f.TryResolveHashSet(deadCheck.Entities, out var prevNode))
            {
                prevNode.Remove(filt.entity);

                //if (!prevNode.Contains(EntityRef.None)) { prevNode.Add(EntityRef.None); } // test

                // Clear this agents neighbors list to rebuild
                if (f.TryResolveHashSet(filt.agent->Neighbors, out var oldNeighbors)) { oldNeighbors.Clear(); }

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
                Int3 neighborKey = filt.agent->Position + offset;

                // Get neighbor nodesets and check for entities
                if (grid.TryGetValue(neighborKey, out Node hood) && f.TryResolveHashSet(hood.Entities, out var entities) && entities.Count >= 0)
                {
                    foreach (var otherEntity in entities)
                    {
                        if (f.Unsafe.TryGetPointer(otherEntity, out DroneAgent* otherAgent) &&
                          otherEntity.Index != filt.entity.Index && otherAgent->Owner == filt.agent->Owner &&
                          f.TryResolveHashSet(filt.agent->Neighbors, out var neighbors) && !neighbors.Contains(otherEntity) && neighbors.Count < 10)
                        {
                            neighbors.Add(otherEntity);
                        }
                        else break;
                    }
                }
            }

            // Update previous position for next cleanup 
            filt.agent->PrevPosition = hashKey;
        }
        else
        {
            // Remove old entries if not flocking 
            if (f.TryResolveHashSet(filt.agent->Neighbors, out var oldNeighbors)) { oldNeighbors.Clear(); }
            if (grid.TryGetValue(hashKey, out Node cutoffNode) && f.TryFreeHashSet(cutoffNode.Entities)) { cutoffNode.Entities = default; grid.Remove(hashKey); }
        }
    }
}