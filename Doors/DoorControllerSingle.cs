using UnityEngine;
using System.Collections;

public class DoorControllerSingle : MonoBehaviour {

	//References should be set to empty parent container "Parent", for relative rotation not absolute
	//Animations will play on pivot
	public GameObject pivot;
	
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}
	
	void OnTriggerEnter(Collider other){
		if (pivot.tag == "left") {
			pivot.GetComponent<Animation> ().Play ("DoorOpenLeft");
		} else if (pivot.tag == "right"){
			pivot.GetComponent<Animation> ().Play ("DoorOpenRight");
		}
	}
	
	void OnTriggerExit(Collider other){
		if (pivot.tag == "left") {
			pivot.GetComponent<Animation> ().Play ("DoorCloseLeft");
		} else if (pivot.tag == "right") {
			pivot.GetComponent<Animation> ().Play ("DoorCloseRight");
		}
	}
}
