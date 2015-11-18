#region License
/*
* ADAPT: Reading Terminal Market
*
* This file is part of the ADAPT for Reading Terminal Market.
* 
* cg.cis.upenn.edu/hms/research/ADAPT/Reading
* 
* ADAPT is free software: you can redistribute it and/or modify
* it under the terms of the GNU Lesser General Public License as published
* by the Free Software Foundation, either version 3 of the License, or
* (at your option) any later version.
* 
* ADAPT is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
* GNU Lesser General Public License for more details.
* 
* You should have received a copy of the GNU Lesser General Public License
* along with ADAPT.  If not, see <http://www.gnu.org/licenses/>.
*/
#endregion

using UnityEngine;
using System.Collections;

public class HeatEmitter : MonoBehaviour {
	
	public LayerMask voxel;
	private int timer;
	public float heatIntensity;
	
	// Use this for initialization  
	void Start () {
		timer = 20;
        if (gameObject != null)
            HeatManager.AddHeatSourceObject(gameObject.transform.position, gameObject.name, heatIntensity);
    }
	
	// Update is called once per frame
	void Update () {
		if (timer < 0) {
            //Collider[] voxels = Physics.OverlapSphere (gameObject.transform.position,
            //                                           2.0f, voxel.value);
            //         int numPeople = voxels.Length;
            //int count = 0;
            //for (int i = 0; i < numPeople; i++) {
            //	float distance = Vector3.Distance (gameObject.transform.position,
            //	                                   voxels [i].gameObject.transform.position);
            //	if (distance <= 2.0f) {
            //		VoxelScript vox = voxels[i].GetComponent<VoxelScript>();
            //		vox.AddHeat(heatIntensity);
            //		vox.SpreadHeat();
            //		//Renderer rend = voxels [i].GetComponent<Renderer> ();
            //		//rend.material.SetColor ("_SpecColor", Color.red);
            //	}
            //}

            timer = 20;
		} else {
			timer--;
		}
	}
}

