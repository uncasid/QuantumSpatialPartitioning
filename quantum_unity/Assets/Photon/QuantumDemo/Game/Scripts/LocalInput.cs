using System;
using Photon.Deterministic;
using Quantum;
using UnityEngine;

public class LocalInput : MonoBehaviour {
    
  private void OnEnable() {
    QuantumCallback.Subscribe(this, (CallbackPollInput callback) => PollInput(callback));
  } 

  public void PollInput(CallbackPollInput callback) {
    Quantum.Input i = new(); 

    var x = UnityEngine.Input.GetAxisRaw("Horizontal"); var y = UnityEngine.Input.GetAxisRaw("Vertical");
    i.Direction = new Vector2(x, y).ToFPVector2();
    i.SpawnBall = UnityEngine.Input.GetMouseButton(0);
    i.Query = UnityEngine.Input.GetKey(KeyCode.Q);
    i.Jump = UnityEngine.Input.GetKey(KeyCode.Space);

    callback.SetInput(i, DeterministicInputFlags.Repeatable);
  }
}
