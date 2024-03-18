using Photon.Deterministic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Quantum {
  unsafe partial class Frame {
    public FPVector3 Unpack(Int3 int3) { return new FPVector3(int3.X, int3.Y, int3.Z); }
  }
}
