using Photon.Deterministic;
using System;

namespace Quantum
{
  public partial struct Int3 : IEquatable<Int3>
  {

    public Int3(short x, short y, short z)
    {
      X = x;
      Y = y;
      Z = z;
    }

    public readonly bool Equals(Int3 other)
    {
      return X == other.X && Y == other.Y && Z == other.Z;
    }

    public override readonly bool Equals(object obj)
    {
      if (obj is Int3 @short)
      {
        return Equals(@short);
      }
      return false;
    }


    #region INT3 / FPMATH

    #region INT3 / FPVECTOR3 

    public static implicit operator FPVector3(Int3 short3)
    {
      return new(short3.X, short3.Y, short3.Z);
    }

    public readonly FP Magnitude => FPMath.Sqrt(X * X + Y * Y + Z * Z);

    /// <summary>
    /// Quantizes the given <paramref name="fpVector3"/> vector to the nearest shorteger values.
    /// </summary>
    /// <param name="fpVector3">The input FPVector3 to quantize.</param>
    /// <returns>The quantized Int3 values.</returns>
    public static Int3 QuantizeToInt3(FPVector3 fpVector3)
    {
      short x = (short)FPMath.RoundToInt(fpVector3.X);
      short y = (short)FPMath.RoundToInt(fpVector3.Y);
      short z = (short)FPMath.RoundToInt(fpVector3.Z);

      return new Int3 { X = x, Y = y, Z = z };
    }

    /// <summary>
    /// Quantizes the given <paramref name="FPVector3"/> to the nearest <paramref name="Int3"/> values with respect to the specified <paramref name="size"/>.
    /// </summary>
    /// <param name="fpVector3">The input FPVector3 to quantize.</param>
    /// <param name="size">The size to divide the vector components by before quantizing.</param>
    /// <returns>The quantized Int3 values.</returns>
    /// <remarks>
    /// Use this overload to set the grid size.
    /// </remarks>
    public static Int3 QuantizeToInt3(FPVector3 fpVector3, FP size)
    {
      short x = (short)FPMath.RoundToInt(fpVector3.X / size);
      short y = (short)FPMath.RoundToInt(fpVector3.Y / size);
      short z = (short)FPMath.RoundToInt(fpVector3.Z / size);

      return new Int3 { X = x, Y = y, Z = z };
    }

    public static FPVector3 operator +(FPVector3 fpVector3, Int3 short3)
    {
      FP newX = fpVector3.X + (FP)short3.X;
      FP newY = fpVector3.Y + (FP)short3.Y;
      FP newZ = fpVector3.Z + (FP)short3.Z;
      return new FPVector3(newX, newY, newZ);
    }

    public static FPVector3 operator -(FPVector3 fpVector3, Int3 short3)
    {
      FP newX = fpVector3.X - (FP)short3.X;
      FP newY = fpVector3.Y - (FP)short3.Y;
      FP newZ = fpVector3.Z - (FP)short3.Z;
      return new FPVector3(newX, newY, newZ);
    }
    #endregion

    #region INT3 / FP

    public static Int3 operator +(Int3 short3, FP fp)
    {
      short3.X += (short)fp.RawValue;
      short3.Y += (short)fp.RawValue;
      short3.Z += (short)fp.RawValue;
      return short3;
    }

    public static Int3 operator -(Int3 short3, FP fp)
    {
      short3.X -= (short)fp.RawValue;
      short3.Y -= (short)fp.RawValue;
      short3.Z -= (short)fp.RawValue;
      return short3;
    }

    public static Int3 operator *(Int3 short3, FP fp)
    {
      short3.X *= (short)fp.RawValue;
      short3.Y *= (short)fp.RawValue;
      short3.Z *= (short)fp.RawValue;
      return short3;
    }

    public static Int3 operator /(Int3 short3, FP fp)
    {
      if (fp.RawValue == 0)
        throw new DivideByZeroException("Division by zero");

      short3.X /= (short)fp.RawValue;
      short3.Y /= (short)fp.RawValue;
      short3.Z /= (short)fp.RawValue;
      return short3;
    }

    #endregion

    #endregion

    #region INT3 / INT3

    public static Int3 Zero => new(0, 0, 0);
    public static Int3 operator -(Int3 short3)
    {
      short3.X = (short)-short3.X;
      short3.Y = (short)-short3.Y;
      short3.Z = (short)-short3.Z;
      return short3;
    }

    public static bool operator !=(Int3 short3A, Int3 short3B)
    {
      return !(short3A == short3B);
    }

    public static bool operator ==(Int3 short3A, Int3 short3B)
    {
      return short3A.X == short3B.X && short3A.Y == short3B.Y && short3A.Z == short3B.Z;
    }

    public static Int3 operator +(Int3 short3A, Int3 short3B)
    {
      short3A.X += short3B.X;
      short3A.Y += short3B.Y;
      short3A.Z += short3B.Z;
      return short3A;
    }

    public static Int3 operator -(Int3 short3A, Int3 short3B)
    {
      short3A.X -= short3B.X;
      short3A.Y -= short3B.Y;
      short3A.Z -= short3B.Z;
      return short3A;
    }

    public static Int3 operator *(Int3 short3A, Int3 short3B)
    {
      short3A.X *= short3B.X;
      short3A.Y *= short3B.Y;
      short3A.Z *= short3B.Z;
      return short3A;
    }

    public static Int3 operator /(Int3 short3A, Int3 short3B)
    {
      if (short3B.X == 0 || short3B.Y == 0 || short3B.Z == 0)
        throw new DivideByZeroException("Division by zero");

      short3A.X /= short3B.X;
      short3A.Y /= short3B.Y;
      short3A.Z /= short3B.Z;
      return short3A;
    }

    #endregion
  }
}