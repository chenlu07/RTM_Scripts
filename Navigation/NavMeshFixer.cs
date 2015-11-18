using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Unity doesnt bake colliders without an active renderer in the navmesh. So enable the renderer, bake, then disable again.
/// author @TJHeuvel via http://forum.unity3d.com/threads/script-baking-navmeshes-for-hidden-objects.128345/
/// </summary>
public class NavMeshFixer : ScriptableObject{
	[MenuItem("ADAPT/Fix NavMesh")]

	public static void FixNavMesh(){        
		Undo.RegisterSceneUndo("BakeNavMesh");
		
		List<Renderer> disabledObjects = new List<Renderer>(); //Keep a list of old objects
		
		foreach (Renderer item in Object.FindObjectsOfType(typeof(Renderer))) //Loop over all renderers
		{
			//Check if its marked as NavigationStatic, and it has a disabled renderer
			if (GameObjectUtility.AreStaticEditorFlagsSet(item.gameObject, StaticEditorFlags.NavigationStatic) && !item.enabled)
			{
				disabledObjects.Add(item);
				item.GetComponent<Renderer>().enabled = true; //Temporary enable it.
			}            
		}
		
		NavMeshBuilder.BuildNavMesh(); //Trigger the navmesh to build.
		
		disabledObjects.ForEach( obj => obj.enabled = false ); //Disable the objects again.
		
	}
}