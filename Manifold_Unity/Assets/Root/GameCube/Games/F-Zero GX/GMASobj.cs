﻿using StarkTools.IO;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class GMASobj : ManifoldAsset<GMA>
{
    public static implicit operator GMA(GMASobj sobj)
    {
        return sobj.Value;
    }
}