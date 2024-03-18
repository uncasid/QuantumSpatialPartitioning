using Photon.Deterministic;

namespace Quantum
{
  public unsafe class BallSystem : SystemMainThreadFilter<BallSystem.Filter>
  {

    public struct Filter
    {
      public EntityRef entity; public Ball* ball; public Transform3D* transform3d; public PhysicsBody3D* rigidBody;
    }

    public override void Update(Frame f, ref Filter filt)
    {
      var table = f.Unsafe.GetPointerSingleton<SpatialHashTable>();
      var gridConfig = f.FindAsset<GridConfig>(table->GridConfig.Id);

      Input* i = f.GetPlayerInput(filt.ball->Owner);

      // Move entities around after spawn
      if (i->SpawnBall.WasReleased)
      {
        FPVector3 rngAboveFloor = new(f.Global->RngSession.Next(-10, 10), f.Global->RngSession.Next(0, 25), f.Global->RngSession.Next(-10, 10));
        filt.rigidBody->AddLinearImpulse(rngAboveFloor);
      }

      // Query from the neighbor list given by the spatial hash table
      if (!i->Query.IsDown)
      {
        var neighborhood = f.ResolveHashSet(filt.ball->Neighbors);
        foreach (var neighbor in neighborhood)
        {
          if (gridConfig.NetLines) Draw.Line(filt.transform3d->Position, f.Get<Transform3D>(neighbor).Position, gridConfig.NetColor);
        }
      }

      // Or query all ball entities
      else
      {
        var otherBalls = f.Filter<Ball, Transform3D>();
        while (otherBalls.NextUnsafe(out var ballEntity, out var ballComponent, out var ballTransform))
        {
          if (gridConfig.NetLines && ballEntity != filt.entity) Draw.Line(filt.transform3d->Position, ballTransform->Position, gridConfig.NetColor);
        }
      }

      // Free hash set and destroy entity when out of bounds
      if (filt.transform3d->Position.Y < -1)
      {
        f.TryFreeHashSet(filt.ball->Neighbors); filt.ball->Neighbors = default; f.Destroy(filt.entity);
      }
    }
  }
}