using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

public class SelectionManager : IPrebuildSetup, IPostBuildCleanup
{
    public int[] selection;
    public void Setup()
    {
        selection = Selection.instanceIDs;
    }

    public void Cleanup()
    {
        Selection.instanceIDs = selection;
    }

}
