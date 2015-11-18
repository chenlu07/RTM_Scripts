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

public class VoxelManager : MonoBehaviour {
	
	public GameObject voxel;
	public int X;
	public int Y;
	public int Z;
	GameObject[][][] vox_arr;

	//changed to awake so HideLayer can find voxels as tagged with "heatmap"
	// Use this for initialization
	void Awake () {
		vox_arr = new GameObject[(X * 2) + 1] [] [];
		int i = 0;
		int j = 0;
		int k = 0;
		for (int x = -1*X; x < X+1; x++) {
			j = 0;
			GameObject[][] tempy = new GameObject[Y][];
			vox_arr[i] = tempy;
			for (int y = 0; y < Y; y++) {
				k = 0;
				GameObject[] tempz = new GameObject[(Z*2) + 1];
				tempy[j] = tempz;
				for(int z = -1*Z; z < Z+1; z++) {
					Vector3 pos = new Vector3(x*4,y*4,z*4);
					//Debug.Log(i + " " + j + " " + k);
					//vox_arr[i][j][k] = (GameObject)Instantiate(voxel,pos,Quaternion.identity);
					tempz[k] = (GameObject)Instantiate(voxel,pos,Quaternion.identity);
					tempz[k].tag = "heatmap";
					VoxelScript temp = tempz[k].GetComponent<VoxelScript>();
					temp.neighbors = new VoxelScript[2,9];
					tempz[k].transform.parent = gameObject.transform;
					Renderer rend = tempz[k].GetComponent<Renderer>();
					Color transparent = new Color(0,0,0,1);
					//rend.material.shader = Shader.Find("Transparent");
					rend.material.SetColor("_SpecColor", transparent);
					k++;
				}
				j++;
			}
			i++;
		}
		for (int x = 0; x < i; x++) {
			for (int y = 0; y < j; y++) {
				for (int z = 0; z < k; z++) {
					VoxelScript curr = vox_arr[x][y][z].GetComponent<VoxelScript>();
					if (y != j-1) {
						VoxelScript vy = vox_arr[x][y+1][z].GetComponent<VoxelScript>();
						curr.neighbors[0,4] = vy;
						if (z != 0) {
							VoxelScript vz = vox_arr[x][y+1][z-1].GetComponent<VoxelScript>();
							curr.neighbors[0,1] = vz;
						}
						if (z != k-1) {
							VoxelScript vz = vox_arr[x][y+1][z+1].GetComponent<VoxelScript>();
							curr.neighbors[0,7] = vz;
						}
						if (x != i-1) {
							VoxelScript vx = vox_arr[x+1][y+1][z].GetComponent<VoxelScript>();
							curr.neighbors[0,3] = vx;
							if (z != 0) {
								VoxelScript vxz = vox_arr[x+1][y+1][z-1].GetComponent<VoxelScript>();
								curr.neighbors[0,0] = vxz;
							}
							if (z != k-1) {
								VoxelScript vxz = vox_arr[x+1][y+1][z+1].GetComponent<VoxelScript>();
								curr.neighbors[0,6] = vxz;
							}
						} 
						if (x != 0) {
							VoxelScript vx = vox_arr[x-1][y+1][z].GetComponent<VoxelScript>();
							curr.neighbors[0,5] = vx;
							if (z != 0) {
								VoxelScript vxz = vox_arr[x-1][y+1][z-1].GetComponent<VoxelScript>();
								curr.neighbors[0,2] = vxz;
							}
							if (z != k-1) {
								VoxelScript vxz = vox_arr[x-1][y+1][z+1].GetComponent<VoxelScript>();
								curr.neighbors[0,8] = vxz;
							}
						}
					}
					if (z != 0) {
						VoxelScript vz = vox_arr[x][y][z-1].GetComponent<VoxelScript>();
						curr.neighbors[1,1] = vz;
					}
					if (z != k-1) {
						VoxelScript vz = vox_arr[x][y][z+1].GetComponent<VoxelScript>();
						curr.neighbors[1,7] = vz;
					}
					if (x != i-1) {
						VoxelScript vx = vox_arr[x+1][y][z].GetComponent<VoxelScript>();
						curr.neighbors[1,3] = vx;

						if (z != 0) {
							VoxelScript vxz = vox_arr[x+1][y][z-1].GetComponent<VoxelScript>();
							curr.neighbors[1,0] = vxz;
						}
						if (z != k-1) {
							VoxelScript vxz = vox_arr[x+1][y][z+1].GetComponent<VoxelScript>();
							curr.neighbors[1,6] = vxz;
						}
					} 
					if (x != 0) {
						VoxelScript vx = vox_arr[x-1][y][z].GetComponent<VoxelScript>();
						curr.neighbors[1,5] = vx;
						if (z != 0) {
							VoxelScript vxz = vox_arr[x-1][y][z-1].GetComponent<VoxelScript>();
							curr.neighbors[1,2] = vxz;
						}
						if (z != k-1) {
							VoxelScript vxz = vox_arr[x-1][y][z+1].GetComponent<VoxelScript>();
							curr.neighbors[1,8] = vxz;
						}
					}
					/*
					if(x != 0) {
						VoxelScript xu = vox_arr[x-1][y][z].GetComponent<VoxelScript>();
						curr.xu = xu;
						xu.xd = curr;
					} if (y != 0) {
						VoxelScript yu = vox_arr[x][y-1][z].GetComponent<VoxelScript>();
						curr.yu = yu;
						yu.yd = curr;
					} if (z != 0) {
						VoxelScript zu = vox_arr[x][y][z-1].GetComponent<VoxelScript>();
						curr.zu = zu;
						zu.zd = curr;
					}*/
				}
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
