using Photon.Deterministic; 

namespace Quantum
{
  unsafe class PlayerSpawnSystem : SystemSignalsOnly, ISignalOnPlayerDataSet
  { 
    public void OnPlayerDataSet(Frame f, PlayerRef player)
    {   
      var data = f.GetPlayerData(player);
      EntityRef entity = default;
      entity = f.Create(f.FindAsset<EntityPrototype>(data.CharacterPrototype.Id));
      f.Unsafe.TryGetPointer<PlayerLink>(entity, out var playerLink);
      playerLink->Player = player;

      if (f.Unsafe.TryGetPointer<Transform3D>(entity, out var tf))
      {
        tf->Position = new FPVector3(0, (int)player * 50, -300);
      }
    }
  }
}