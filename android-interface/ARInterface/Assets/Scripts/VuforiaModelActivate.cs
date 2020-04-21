using UnityEngine;
using Vuforia;


public class VuforiaModelActivate : MonoBehaviour, ITrackableEventHandler
{
    private TrackableBehaviour myTrackableBehaviour;

    public GameObject RobotModel;
    public GameObject DummyParent;

    private bool tracked = false;


    void Start()
    {
        myTrackableBehaviour = GetComponent<TrackableBehaviour>();
        if (myTrackableBehaviour)
        {
            myTrackableBehaviour.RegisterTrackableEventHandler(this);
        }
    }

    void Update()
    {
    }

    public void OnTrackableStateChanged(TrackableBehaviour.Status previousStatus,
      TrackableBehaviour.Status newStatus)
    {
        if (newStatus == TrackableBehaviour.Status.DETECTED ||
            newStatus == TrackableBehaviour.Status.TRACKED ||
            newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
        {
            if (!tracked)
            {
                OnTrackerFound();
                tracked = true;
            }
        }
        else
        {
            if (tracked)
            {
                TrackerNotFound();
                tracked = false;
            }
        }
    }

    // Tracker found:  change model parent to IT and re-scale
    private void OnTrackerFound()
    {
        //if (myModelPrefab != null) {
        Transform myModelTrf = RobotModel.transform;
        myModelTrf.parent = myTrackableBehaviour.transform;
        myModelTrf.localPosition = new Vector3(0f, 0f, 0f);
        myModelTrf.localRotation = Quaternion.identity;
        myModelTrf.localScale = new Vector3(1f, 1f, 1f);
        myModelTrf.gameObject.SetActive(true);

    }

    // Tracker not found:  Change parent to dummy and re-scale
    private void TrackerNotFound()
    {
        Transform myModelTrf = RobotModel.transform;
        myModelTrf.parent = DummyParent.transform;
        myModelTrf.localPosition = new Vector3(0f, -0.25f, 2.5f);
        myModelTrf.localRotation = Quaternion.identity;
        myModelTrf.localScale = new Vector3(1f, 1f, 1f);
        myModelTrf.gameObject.SetActive(true);

    }
}