using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;


public class DetectedPlaneGeneratorGeo : MonoBehaviour
{
    /// <summary>
    /// A prefab for tracking and visualizing detected planes.
    /// </summary>
    public GameObject DetectedPlanePrefab;
    public ModelController modelControllerDetec;

    /// <summary>
    /// A list to hold new planes ARCore began tracking in the current frame. This object is
    /// used across the application to avoid per-frame allocations.
    /// </summary>
    private List<DetectedPlane> _newPlanes = new List<DetectedPlane>();

    private List<GameObject> _placeholderPlanes = new List<GameObject>();

    /// <summary>
    /// The Unity Update method.
    /// </summary>
    public void Update()
    {
        // Check that motion tracking is tracking.
        if (Session.Status != SessionStatus.Tracking)
        {
            return;
        }

        
    
        Session.GetTrackables<DetectedPlane>(_newPlanes, TrackableQueryFilter.New);
        for (int i = 0; i < _newPlanes.Count; i++)
        {
            // Instantiate a plane visualization prefab and set it to track the new plane. The
            // transform is set to the origin with an identity rotation since the mesh for our
            // prefab is updated in Unity World coordinates.
            GameObject planeObject =
                Instantiate(DetectedPlanePrefab, Vector3.zero, Quaternion.identity, transform);
            
            // _placeholderPlanes[i] = planeObject;
            planeObject.GetComponent<DetectedPlaneVisualizerGeo>().Initialize(_newPlanes[i]);
        }
    
    }
}
