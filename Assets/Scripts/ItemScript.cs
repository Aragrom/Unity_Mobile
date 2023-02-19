///<author> Graham Alexander MacDonald 05/02/2015 </author>

using UnityEngine;
using System.Collections;

public class ItemScript : MonoBehaviour
{
    private float fDropVelocity = 10.0f;

    public Color colorStart = Color.white;
    public Color colorEnd = Color.black;
    public float fDuration = 1.0f;
	public float fRotateSpeed = 100.0f;

    private PlayerInventoryScript pis;

    /// <summary> Use this for initialization </summary>
    void Start()
    {
        if (this.gameObject.name == "Health"
			|| this.gameObject.name == "Health(Clone)")
		{
			colorStart = Color.green;
			colorEnd = Color.white;
		}

		if (this.gameObject.name == "Charge"
		    || this.gameObject.name == "Charge(Clone)")
		{
			colorStart = Color.blue;
			colorEnd = Color.white;
		}

		if (this.gameObject.name == "InstantCharge"
		    || this.gameObject.name == "InstantCharge(Clone)")
		{
			colorStart = Color.magenta;
			colorEnd = Color.white;
		}
    }

    /// <summary> Update is called once per frame </summary>
    void Update()
    {
        UpdateMaterialColour();
		RotateItem ();
    }

	void RotateItem()
	{
		this.gameObject.transform.Rotate(0, Time.deltaTime * fRotateSpeed, 0);
	}

    /// <summary> Use to instantiate an object in the world. Velocity is added to the'item' on 'dropping' </summary>
    public void DropItem(Vector3 v3)
    {
        GameObject go = (GameObject)Instantiate(gameObject, v3 + Vector3.up * 2, Quaternion.identity);
        go.GetComponent<Rigidbody>().velocity = new Vector3(0, fDropVelocity, 0);
    }

    /// <summary> Creates the flashing effect on the texture </summary>
    void UpdateMaterialColour()
    {
        float lerp = Mathf.PingPong(Time.time, fDuration) / fDuration;
        GetComponent<Renderer>().material.color = Color.Lerp(colorStart, colorEnd, lerp);
    }
}
