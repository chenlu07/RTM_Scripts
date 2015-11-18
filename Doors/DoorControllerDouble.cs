using UnityEngine;
using System.Collections;

public class DoorControllerDouble : MonoBehaviour {

	//References should be set to empty parent container "ParentLeft" and "ParentRight", for relative rotation not absolute
	//Animations will play on pivotleft and pivotright
	public GameObject pivotleft;
	public GameObject pivotright;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}

	void OnTriggerEnter(Collider other){
		pivotleft.GetComponent<Animation>().Play ("DoorOpenLeft");
		pivotright.GetComponent<Animation>().Play ("DoorOpenRight");
	}

	void OnTriggerExit(Collider other){
		pivotleft.GetComponent<Animation>().Play ("DoorCloseLeft");
		pivotright.GetComponent<Animation>().Play ("DoorCloseRight");

	}
}
