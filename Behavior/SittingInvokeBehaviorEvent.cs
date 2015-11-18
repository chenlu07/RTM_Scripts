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
using TreeSharpPlus;
using System.Collections;
using System.Collections.Generic;

public class SittingInvokeBehaviorEvent : MonoBehaviour {
	
	public List<GameObject> _chairs;
	public Behavior Character;
	public Behavior Chair;
	public GameObject targetChair;
	public GameObject wander;

	protected Node ApproachAndOrient(Val<Vector3> ChairPos, Val<Vector3> OrientPos)
	{
		
		return new Sequence(
			// Approach at distance .015f
			Character.Node_GoTo(ChairPos, 0.15f),
			new LeafWait(1000),
			Character.Node_OrientTowards(OrientPos),
			
			new LeafWait(1000));
	}
	
	public GameObject FindClosestChair(){
		GameObject closest = null;
		var distance = Mathf.Infinity;
		//Changed transform.psoition to Character.transform.position
		var position = transform.position;

		//iterate through charis to find closest one
		foreach (GameObject chair in _chairs){
			var difference = (chair.transform.position - position);
			var currentDistance = difference.sqrMagnitude;
			if (currentDistance < distance){
				closest = chair;
				distance = currentDistance;
			}
		}
		//Change renderer color, for debugging
		var rend = closest.GetComponent<Renderer>();
		
		// Set specular shader
		rend.material.shader = Shader.Find ("Specular");
		
		// Set red specular highlights
		rend.material.SetColor ("_SpecColor", Color.blue);
		
		return closest;
		
	}
	
	void Awake(){
	}
	
	void Start()
	{	
		//store tagged objects as array --> list
		var array = GameObject.FindGameObjectsWithTag ("chair");
		_chairs = new List<GameObject> (array);
	}
	
	protected Node Converse()
	{
		return new Sequence (
			Character.Node_Gesture ("acknowledging"),
			Character.Node_Gesture ("being_cocky"));
	}
	
	public Node SittingTree(){
		Vector3 realCenter = targetChair.GetComponent<Renderer>().bounds.center; //get center of renderer, to account for offset objects transform
		Vector3 chairDirection = targetChair.transform.forward;
		Quaternion chairRotation = targetChair.transform.rotation;
		float distance = 0.6f;
		
		Vector3 waitPos = realCenter + chairDirection * distance;
		Val<Vector3> ChairPos = Val.Value (() => waitPos);

		GameObject prefabwander = (GameObject)Instantiate (wander, waitPos, Quaternion.identity);
		Val<Vector3> OrientPos = Val.Value (() => waitPos + (waitPos - realCenter) * 100);
		return this.ApproachAndOrient (ChairPos, OrientPos);
	}
	
	
	void Update() 
	{
		if (Input.GetKeyDown (KeyCode.Z) == true) {
			BehaviorEvent.Run (this.SittingTree (), Character);
		}
	}
	
}