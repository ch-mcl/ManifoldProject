﻿using StarkTools.IO;
using System.IO;
using UnityEngine;

[CreateAssetMenu(menuName = "Manifold/Import/" + "GMA Importer")]
public class GMAImporter : ImportSobj<GMASobj>
{
    public override string ProcessMessage => $"{GetType().Name} process";

    public override string HelpBoxMessage => string.Empty; //$"{GetType().Name} help box message";
}
