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

public class HeatReceiver : MonoBehaviour {
	
	public float heat;
	public LayerMask voxel;
	private int timer;
	
	// Use this for initialization
	void Start () {
		timer = 60;
	}
	
	// Update is called once per frame
	void Update () {
		if (timer < 0) {

            int numPeople = ModelCrowdMapManager.GetNumOfAgentsindistance(gameObject.transform.position, 0.45f);
            int numPeople2 = ModelCrowdMapManager.GetNumOfAgentsindistance(gameObject.transform.position, 1.2f);

            //int numPeople2 = voxels2.Length;
            //if (numPeople2 > 0)
            //{
            //    for (int i = 0; i < numPeople2; i++)
            //    {
            //        float distance = Vector3.Distance(gameObject.transform.position, voxels2[i].gameObject.transform.position);

            //        /////What's the relationship between agent's distance and the heat transfer or comfort changing between them? Should it be linear?
            //    }

            //}

            this.GetComponent<ComfortJudgment>().intimatenum = numPeople;
            this.GetComponent<ComfortJudgment>().personalnum = numPeople2;


            timer = 60;
		} else {
			timer--;
		}
	}

}
