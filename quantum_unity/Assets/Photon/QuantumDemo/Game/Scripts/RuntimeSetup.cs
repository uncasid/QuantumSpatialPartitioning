using Quantum;
using UnityEngine;

public sealed class RuntimeSetup : MonoBehaviour
{
  public static RuntimeSetup Instance { get; private set; }

  //public RuntimeConfig GameConfig { get { return _gameConfig; } }
  public RuntimePlayer PlayerConfig { get { return _playerConfig; } }

  //[SerializeField] private RuntimeConfig _gameConfig;
  [SerializeField] private RuntimePlayer _playerConfig;

  private void Awake()
  {
    Instance = this;
  }
}