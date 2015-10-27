#region usages

using System;

using UnityEngine;

using System.Collections;

using Random = UnityEngine.Random;

#endregion

public static class RandomExtensions
{
    #region Public Methods and Operators

    public static Int64 NextInt64(this System.Random rnd)
    {
        var buffer = new byte[sizeof(Int64)];
        rnd.NextBytes(buffer);
        return BitConverter.ToInt64(buffer, 0);
    }

    #endregion
}
