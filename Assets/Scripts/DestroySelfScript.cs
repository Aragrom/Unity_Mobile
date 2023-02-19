///<author> Graham Alexander MacDonald 03/03/2015 </author>

using UnityEngine;
using System.Collections;

public class DestroySelfScript : MonoBehaviour
{
	GameObject goGreatGrandParent;
	GameObject goGrandParent;
	GameObject goParent;

	// Use this for initialization
	void Start ()
	{
		//Set up links with parent objects
		if(this.gameObject.transform.parent != null) 
		{
			goParent = this.gameObject.transform.parent.gameObject;
			if(goParent.transform.parent != null)
			{
				goGrandParent = goParent.transform.parent.gameObject;
				if(goGrandParent.transform.parent != null) goGreatGrandParent = goGrandParent.transform.parent.gameObject;
			}
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	void OnTriggerEnter(Collider c)
	{
		//On collision destroy the "oldest" parent (top level/deleting all children)
		if (c.tag == "Ground" || c.tag == "House")
		{
			if(goGreatGrandParent != null)
			{
				Debug.Log ("Great Grand Parent = Destroyed :" + goGreatGrandParent.name + " at " + goGreatGrandParent.transform.position.ToString());
				Destroy(goGreatGrandParent.gameObject);
			}
			else if(goGrandParent != null)
			{
				Debug.Log ("Grand Parent = Destroyed :" + goGrandParent.name + " at " + goGrandParent.transform.position.ToString());
				Destroy(goGrandParent.gameObject);
			}
			else if(goParent != null)
			{
				Debug.Log ("Parent = Destroyed :" + goParent.name + " at " + goParent.transform.position.ToString());
				Destroy(goParent.gameObject);
			}
		}
	}
}
