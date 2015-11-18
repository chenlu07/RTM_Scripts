using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HideLayer : MonoBehaviour {

	protected bool active;
	public List<GameObject> heatitems;

	// Use this for initialization
	void Start () {
		active = true;
		var array = GameObject.FindGameObjectsWithTag ("heatmap");
		heatitems = new List<GameObject> (array);
	}
		// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.M))
		{
			if (active){
				active = false;
				foreach (GameObject heatitem in heatitems){
					heatitem.GetComponent<Renderer>().enabled = false;
				}

			}else{
				active = true;
				foreach (GameObject heatitem in heatitems){
					heatitem.GetComponent<Renderer>().enabled = true;
				}
			}
		}
	}
}
