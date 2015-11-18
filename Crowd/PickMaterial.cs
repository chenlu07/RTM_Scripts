using UnityEngine;
using System.Collections;
using System;
//Should be used on the same GameObject on which the the SkinnedMeshRenderer is located
public class PickMaterial : MonoBehaviour
{
    public Material[] _materials;
    public SkinnedMeshRenderer smrenderer;//Attach the enabled SkinnedMeshRenderer using the inspector with desired settings
    static private System.Random rnd = new System.Random();

    public void Start()
    {
        smrenderer = gameObject.GetComponentInChildren<SkinnedMeshRenderer>();

        int i = rnd.Next(_materials.Length);
        smrenderer.material = _materials[i];
    }
}