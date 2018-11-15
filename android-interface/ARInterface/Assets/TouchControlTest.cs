using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchControlTest : MonoBehaviour {

    private Vector3 mousePosition;
    public float moveSpeed = 0.1f;


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButton(0))
        {
            mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = transform.position.z;
            Debug.Log(mousePosition);

            //transform.position = Vector3.Lerp(new Vector3(transform.position.x, transform.position.y, transform.position.z), mousePosition, moveSpeed);
            transform.position = Vector3.MoveTowards(transform.position, mousePosition, moveSpeed);
        }
	}
}
