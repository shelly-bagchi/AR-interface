using UnityEngine;

public class CopyTarget : MonoBehaviour {
    public GameObject inputTarget;
    public float delay = 0.2f;

    private float timer = 0.0f;

    // Use this for initialization
    void Start ()
    { // Copy loc from target
        gameObject.transform.SetPositionAndRotation(inputTarget.transform.position, inputTarget.transform.rotation);
    }
	
	// Update is called once per frame
	void Update () { }

    // Right before camera renders
    void LateUpdate()
    {
        // Count up until delay is reached
        timer += Time.deltaTime;
        if (timer >= delay) {
            gameObject.transform.SetPositionAndRotation(inputTarget.transform.position, inputTarget.transform.rotation);
            timer = 0.0f;
        }
    }
    


}
