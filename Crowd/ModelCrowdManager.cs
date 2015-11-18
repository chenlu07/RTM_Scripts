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
using System.Collections.Generic;
using UnityEngine.UI;

public class ModelCrowdManager : MonoBehaviour {

	public GameObject wander;
	public GameObject FullCharacter;
    public GameObject[] _models;
	public GameObject ObjectLabel;
	public int crowdsize; //crowd size
    public System.Random PickModel = new System.Random();
    public static int NumberModels;
    public static int model;
    public static bool follow;
	public static GameObject[] _characters;
	public static GameObject[] _sidewalks;
	public static Stack<int> _numbers; //available numbers, based on crowdsize

	void Start () {
		follow = true;
		_numbers = new Stack<int>();
		for (int i = crowdsize; i >= 1; i--){
			_numbers.Push (i);
		}
		_sidewalks = GameObject.FindGameObjectsWithTag ("sidewalk");
		this.BuildCrowd (crowdsize);
	}
	
	// Update is called once per frame
	void Update () {
		//toggle camera follow
		if (Input.GetKeyDown (KeyCode.F))
		if (follow) {
			follow = false;
		} else {
			follow = true;
		}
		GameObject.Find ("Main Camera").GetComponent<SmoothFollow> ().enabled = follow;
		GameObject.Find ("Main Camera").GetComponent<FlyingCamera> ().enabled = !follow;
	}

	protected void BuildCrowd(int crowdsize){
		if (crowdsize <= 0) {
			throw new System.ArgumentException("Crowd must be bigger than 0");
		}
		_characters = new GameObject[crowdsize];
		for (int n = 0; n < crowdsize; n++)
		{
			if(n == 0)
			{
				//Instantiate first agent randomly on sidewalk
				_characters[n] = this.SpawnOutside ();
				_characters[n].tag = "Player";

				//Set camera follow to first agent
				GameObject.Find("CameraControl/Main Camera").GetComponent<SmoothFollow>().target = _characters[n].transform;
				
				//Camera control variable just a test, remove
				Body body = _characters[n].GetComponent<Body>();
				GameObject.Find ("CameraControl").GetComponent<DemoKeyCommands>().bodyInterface = body;
			}
			else
			{
				//If not first agent, instantiate randomly on NavMesh
				_characters[n] = this.SpawnRandom();	
			}
			//Get character name from stack
			int num = _numbers.Pop ();

			//Name in Hierarchy
			string name = "Character " + num;
			_characters[n].name = name;

			//Add character label
			GameObject canvas = Instantiate(ObjectLabel) as GameObject;
			canvas.transform.parent = _characters[n].transform;
			canvas.transform.localPosition = new Vector3(0.0f, 2.5f, 0.0f);
			Text editableText = canvas.transform.Find("Text").GetComponent<Text>();
			//editableText.text = "Character " + (n+1);
			editableText.text = _characters[n].name;

			canvas.AddComponent<LabelScript>();

			//Generate new wanderpoints
			GameObject newwander1 = generateWanderPoint();
			newwander1.name = "wanderpoint1";
			GameObject newwander2 = generateWanderPoint ();
			newwander2.name = "wanderpoint2";
			GameObject newwander3 = generateWanderPoint ();
			newwander3.name = "wanderpoint3";

			var empty = new GameObject();
			empty.transform.position = Vector3.zero;
			empty.name = "Wanderpoints " + num;

			//Wanderbehavior added in Spawn methods
			//Set wanderbehavior references to new points
			_characters[n].GetComponent<WanderBehavior>().wander1 = newwander1.transform;
			_characters[n].GetComponent<WanderBehavior>().wander2 = newwander2.transform;
			_characters[n].GetComponent<WanderBehavior>().wander3 = newwander3.transform;

			newwander1.transform.parent = empty.transform;
			newwander2.transform.parent = empty.transform;
			newwander3.transform.parent = empty.transform;
		}
	}

	/*
	 * Places character at new random exterior location on sidewalk
	 */
	public void Respawn(GameObject character){
		GameObject randomSidewalk = _sidewalks[Random.Range (0, _sidewalks.Length)]; //get random sidewalk square
		character.transform.position = randomSidewalk.transform.position;
		character.transform.rotation = randomSidewalk.transform.rotation;
	}

	/*
	 *Spawn character at designated location (Vector3). Returns instantiated character. 
	 */
	protected GameObject SpawnLocation(Vector3 location){
		NavMeshHit hit;
		if (!NavMesh.SamplePosition(location, out hit, 1.0f, NavMesh.AllAreas)){ //if input location not a (close) valid point on NavMesh, throw exception
			throw new System.ArgumentException("Target location not on NavMesh");
		}
		//instantiate with location
		GameObject character = (GameObject)Instantiate (FullCharacter, location, Quaternion.identity);

		character.tag = "character";

		character.AddComponent<Gestures>();
		character.AddComponent<WanderBehavior>();

		//Collider for automatic doors
		CapsuleCollider collider = character.AddComponent<CapsuleCollider> ();
		Rigidbody rb = character.GetComponent<Rigidbody> ();
		rb.isKinematic = true;
		collider.center = new Vector3 (0, 1, 0);
		collider.radius = 0.3f;
		collider.height = 3.0f;
		collider.direction = 1;

		return character;

	}
	
	/*
	 *Spawn character on random sidewalk square outside market. Returns instantiated character. 
	 */
	protected GameObject SpawnOutside(){
		GameObject randomSidewalk = _sidewalks[Random.Range (0, _sidewalks.Length)]; //get random sidewalk square

		//Outside, initial main character intantiated as a FullCharacter, to change, switch to InitializeModel();
		//GameObject character = (GameObject)Instantiate (InitializeModel(), randomSidewalk.transform.position, randomSidewalk.transform.rotation);
		GameObject character = (GameObject)Instantiate (FullCharacter, randomSidewalk.transform.position, randomSidewalk.transform.rotation);

		character.tag = "character";

		character.AddComponent<Gestures>();
		character.AddComponent<WanderBehavior>();

		//Collider for automatic doors
		CapsuleCollider collider = character.AddComponent <CapsuleCollider>();
		Rigidbody rb = character.GetComponent<Rigidbody> ();
		rb.isKinematic = true;
		collider.center = new Vector3 (0, 1, 0);
		collider.radius = 0.3f;
		collider.height = 3.0f;
		collider.direction = 1;

		return character;
	}
	
	/*
	 *Spawn character randomly on interior navmesh. Returns instantiated character. 
	 */
	protected GameObject SpawnRandom(){
		GameObject character = (GameObject)Instantiate(InitializeModel(), RandomPoint(new Vector3(0, 0, 0), 50.0f), Quaternion.identity);

		character.tag = "character";

		//Vincent and Dana not with Gestures capabilities
		//character.AddComponent<Gestures>();
		character.AddComponent<WanderBehavior>();

		//Collider for automatic doors
		CapsuleCollider collider = character.AddComponent <CapsuleCollider>();
		Rigidbody rb = character.GetComponent<Rigidbody> ();
		rb.isKinematic = true;
		collider.center = new Vector3 (0, 1, 0);
		collider.radius = 0.3f;
		collider.height = 3.0f;
		collider.direction = 1;

		return character;
	}

    //Pick random character model to spawn.
    protected GameObject InitializeModel(){
        if (_models.Length == 0){
            throw new System.ArgumentException("Must be at least one character model");
        }
        else{
            model = PickModel.Next(_models.Length);
            GameObject Model = _models[model];

            return Model;
        }
    }
    public void Enter(){
		if (_numbers.Count == 0) {
			Debug.Log ("crowd is full");	//crowd is 'full'
		}
		int num = _numbers.Pop ();
		_characters [num] = this.SpawnOutside ();

		string name = "FullCharacter" + num;
		_characters[num].name = name;
		
		//Add character label
		GameObject canvas = Instantiate(ObjectLabel) as GameObject;
		canvas.transform.parent = _characters[num].transform;
		canvas.transform.localPosition = new Vector3(0.0f, 2.5f, 0.0f);
		Text editableText = canvas.transform.Find("Text").GetComponent<Text>();
		editableText.text = "Character " + num;
		
		//Generate new wanderpoints
		GameObject newwander1 = generateWanderPoint();
		newwander1.name = "wanderpoint1";
		GameObject newwander2 = generateWanderPoint ();
		newwander2.name = "wanderpoint2";
		GameObject newwander3 = generateWanderPoint ();
		newwander3.name = "wanderpoint3";
		
		var empty = new GameObject();
		empty.transform.position = Vector3.zero;
		empty.name = "WanderPoints" + num;
		
		//Set wanderbehavior references to new points
		_characters[num].GetComponent<WanderBehavior>().wander1 = newwander1.transform;
		_characters[num].GetComponent<WanderBehavior>().wander2 = newwander2.transform;
		_characters[num].GetComponent<WanderBehavior>().wander3 = newwander3.transform;
		
		newwander1.transform.parent = empty.transform;
		newwander2.transform.parent = empty.transform;
		newwander3.transform.parent = empty.transform;
	}

	public void Exit(){
	}
	
	/* Generate a random wander point on walkable NavMesh
	 * Can adjust RandomPoint range as desired (RandomPoint(center, maxRange))
	 */
	protected GameObject generateWanderPoint(){
		GameObject prefabwander = (GameObject)Instantiate (wander, RandomPoint (new Vector3(0,0,0), 50.0f), Quaternion.identity);
		return prefabwander;
	}

	public Vector3 RandomPoint(Vector3 center, float maxRadius){
		Vector3 randomDirection = Random.insideUnitSphere * maxRadius;
		randomDirection += center;
		NavMeshHit hit;
		NavMesh.SamplePosition(randomDirection, out hit, maxRadius, 1);
		Vector3 finalPosition = hit.position;
		return finalPosition;
	}
}