///<author> Graham Alexander MacDonald 20/02/2015 </author>

using UnityEngine;
using System.Collections;

public class TurnPointScript : MonoBehaviour
{
    public enum eDirection { north, east, south, west }
    public int iDirection;
	public bool bIsCheckPoint;

	// Use this for initialization
	void Start ()
	{
		if (iDirection == 3 && this.gameObject.name == "BigTurnPoint(Clone)")
			this.transform.rotation = Quaternion.Euler(0,0,0);
	}
	
	// Update is called once per frame
	void Update ()
	{

	}
}
