using Photon.Deterministic;

namespace Quantum
{
  public unsafe class PlayerActionsSystem : SystemMainThreadFilter<PlayerActionsSystem.Filter>
  {

    public struct Filter
    {
      public EntityRef entity; public CharacterController3D* kcc; public PlayerLink* playerLink; public Transform3D* transform3d;
    }

    public override void Update(Frame f, ref Filter filt)
    {
      Input* i = f.GetPlayerInput(filt.playerLink->Player);

      if (i->Jump.WasPressed) filt.kcc->Jump(f);
      filt.kcc->Move(f, filt.entity, i->Direction.XOY);

      // Spawn 100 entities per left click around the player, link ownership, and allocate their EntityRef hash sets
      if (i->SpawnBall.WasPressed) 
      {
        for (int j = 0; j < 100; j++)
        { 
          var ballEntity = f.Create(filt.playerLink->BallRef); 
          f.Unsafe.TryGetPointer<Transform3D>(ballEntity, out var ballTransform); 
          FPVector3 rngAboveFloor = new(f.Global->RngSession.Next(-10, 10), f.Global->RngSession.Next(0, 50), f.Global->RngSession.Next(-10, 10));
          ballTransform->Position = filt.transform3d->Position + rngAboveFloor;

          f.Unsafe.TryGetPointer<Ball>(ballEntity, out var ballComponent);
          ballComponent->Owner = filt.playerLink->Player;
          ballComponent->Neighbors = f.AllocateHashSet<EntityRef>();
        } 
      }
    } 
  }
}