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

public class VoxelScript : MonoBehaviour {
	
	public VoxelScript[,] neighbors;
	public float heat;
	int timer;
	
	// Use this for initialization
	void Start () {
		heat = 0;
		timer = 20;
	}
	
	// Update is called once per frame
	void Update () {
		if (heat > 15) {
			Renderer rend = GetComponent<Renderer> ();
			Material material = new Material(Shader.Find("Transparent/Diffuse"));
			rend.material = material;
			//rend.material.SetColor ("_SpecColor", Color.red);
			rend.material.color = new Color(0.04f * heat,0f,0f,0.03f * heat);
		} else if (heat > 5) {
			Renderer rend = GetComponent<Renderer> ();
			Material material = new Material(Shader.Find("Transparent/Diffuse"));
			rend.material = material;
			//rend.material.SetColor ("_SpecColor", Color.red);
			rend.material.color = new Color(0f,0.2f * heat,0f,0.01f * heat);
		}
		if (timer > 0) {
			timer--;
		} else {
			if (heat > 0) {
				heat--;
			}
			timer = 30;
		}
	}

	public void AddHeat(float h) {
		heat += h;
	}

	public void SpreadHeat() {
//		for (int i = 0; i < neighbors.GetLength(0); i++) {
//			for(int j = 0; j < neighbors.GetLength(1); j++) {
//				if(neighbors[i,j] != null) {
////					Debug.Log(".");
//					neighbors[i,j].AddHeat(1);
//				}
//			}
//		}
	}
}
