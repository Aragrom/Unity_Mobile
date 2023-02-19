using UnityEngine;
using System.Collections;

public class DestroyMessageScript : MonoBehaviour {

	public int iDestroyAfter;
	// Use this for initialization
	void Start () 
	{
		Invoke ("Destroy", iDestroyAfter);
	}
	
	void Destroy()
	{
		Destroy(this.gameObject);
	}
}
