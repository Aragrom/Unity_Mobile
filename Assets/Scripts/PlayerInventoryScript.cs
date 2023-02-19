///<author> Graham Alexander MacDonald 05/02/2015 </author>

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerInventoryScript : MonoBehaviour
{
    public List<GameObject> LgoItems;

    private PlayerScript ps;

    /// <summary> Use this for initialization </summary>
	void Start ()
    {
        LinkComponents();
	}

    /// <summary> Update is called once per frame </summary>
	void Update ()
    {

	}

    /// <summary> Links components </summary>
    void LinkComponents()
    {
        ps = this.gameObject.GetComponent<PlayerScript>();
    }

    /// <summary> </summary>
    /// <param name="c"></param>
    void OnTriggerEnter(Collider c)
    {
		if(c.gameObject.tag == "Item")
		{
			LgoItems.Add(c.gameObject);
			c.gameObject.SetActive(false);
		}

		if (c.gameObject.name == "Health"
		    || c.gameObject.name == "Health(Clone)")
		{
			ps.pcs.iCurrentHealth++;
			Debug.Log ("Health Picked up");
		}

		if (c.gameObject.name == "InstantCharge"
		    || c.gameObject.name == "InstantCharge(Clone)")
		{
			ps.pcs.iCharges++;
			ps.pcs.StartCharge(10.0f); //10.0f is not used for possible increase in charge related to swipe vector
			Debug.Log ("Instant-Charge picked up");
		}

		if (c.gameObject.name == "Charge"
		    || c.gameObject.name == "Charge(Clone)")
		{
			ps.pcs.iCharges++;
			Debug.Log ("Charge picked up");
		}
    }
}
