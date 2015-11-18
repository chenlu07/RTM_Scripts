using UnityEngine;
using System.Collections;

public class BirdsEye : MonoBehaviour {

	protected bool on;
	protected GameObject roof;
	public Camera cam1;
	public Camera cam2;

	// Use this for initialization
	void Start () {
		on = false;
		roof = GameObject.Find ("Building/Roof");
		cam1.enabled = true;
		cam2.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.B))
		{
			//turn off birds eye
			if (on){
				on = false;
				roof.SetActive (true);
				//turn on regular camera, turn off bird camera
				cam1.enabled = true;
				cam2.enabled = false;
				
			}else{//turn on birds eye
				on = true;
				roof.SetActive (false);
				//turn on bird camera, turn off regular
				cam1.enabled = false;
				cam2.enabled = true;
			}
		}
	}
}
