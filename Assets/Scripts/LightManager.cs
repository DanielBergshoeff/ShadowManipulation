using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightManager : MonoBehaviour
{
    public GameObject PlayerSkeleton;
    public List<Transform> PlayerSkeletonParts;

    public GameObject Light;
    public LayerMask CubeLayer;

    public static LightManager Instance;
    public Vector3 AveragePos;
    private float ClosestPointShadow;
    private float FurthestPointShadow;
    public float SizeShadow;
    public bool ShadowFound;

    public bool ShowDebugLines = false;

    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null)
            Instance = this;
        PlayerSkeletonParts = new List<Transform>();
        AddChildrenToList(PlayerSkeletonParts, PlayerSkeleton.transform);
    }

    // Update is called once per frame
    void Update()
    {
        ShadowCube();
        ShadowAveragePosition();
    }

    /// <summary>
    /// Adds all children transforms of an object to a single list
    /// </summary>
    /// <param name="list"></param>
    /// <param name="ParentTransform"></param>
    private void AddChildrenToList(List<Transform> list, Transform ParentTransform) {
        list.Add(ParentTransform);
        for (int i = 0; i < ParentTransform.childCount; i++) {
            AddChildrenToList(list, ParentTransform.GetChild(i));
        }
    }


    /// <summary>
    /// Takes all cubes currently in the shadow of the player and applies an upwards force to them
    /// </summary>
    private void ShadowCube() {
        List<GameObject> cubesTouchedByShadow = new List<GameObject>(); //Create new list of cubes
        for (int i = 0; i < PlayerSkeletonParts.Count; i++) { //For each part of the skeleton
            var heading = PlayerSkeletonParts[i].position - Light.transform.position;
            var distance = heading.magnitude;
            var direction = heading / distance;
            RaycastHit hit;
            if (Physics.Raycast(PlayerSkeletonParts[i].position, direction, out hit, 100.0f, CubeLayer)) { //Raycast from the position, directly away from the light
                if (!cubesTouchedByShadow.Contains(hit.collider.gameObject)) //If it touches a cube that isn't in the list
                    cubesTouchedByShadow.Add(hit.collider.gameObject); //Add it to the list
            }
            //Debug.DrawRay(PlayerSkeletonParts[i].position, direction * 100.0f);
        }

        foreach (GameObject cube in cubesTouchedByShadow) { //For each cube that is in the shadow
            cube.GetComponent<Rigidbody>().AddForce(Vector3.up * 100.0f);  //Add an upward force with 10.0f strength
        }
    }

    /// <summary>
    /// Calculates the average position of the shadow
    /// </summary>
    private void ShadowAveragePosition() {
        int count = 0;
        Vector3 closest = Vector3.positiveInfinity;
        float closestPointDistance = float.PositiveInfinity;
        Vector3 furthest = Vector3.zero;
        float furthestPointDistance = float.NegativeInfinity;
        Vector3 average = Vector3.zero;
        for (int i = 0; i < PlayerSkeletonParts.Count; i++) { //For each part of the skeleton
            var heading = PlayerSkeletonParts[i].position - Light.transform.position;
            var distance = heading.magnitude;
            var direction = heading / distance;
            RaycastHit hit;
            if (Physics.Raycast(PlayerSkeletonParts[i].position, direction, out hit, 100.0f)) { //Raycast from the position, directly away from the light
                if (hit.collider.CompareTag("Wall")) {
                    Vector3 headingToPoint = hit.point - PlayerSkeleton.transform.position;
                    float distancePoint = headingToPoint.sqrMagnitude;

                    if (distancePoint < closestPointDistance) {
                        closestPointDistance = distancePoint;
                        closest = hit.point;
                    }
                    if (distancePoint > furthestPointDistance) {
                        furthestPointDistance = distancePoint;
                        furthest = hit.point;
                    }
                    average += hit.point;
                    count++;
                }
                if(ShowDebugLines)
                    Debug.DrawRay(PlayerSkeletonParts[i].position, direction * 100.0f);
            }
        }

        if (count > 0) {
            AveragePos = average / count;
            ClosestPointShadow = Mathf.Sqrt(closestPointDistance);
            FurthestPointShadow = Mathf.Sqrt(furthestPointDistance);
            SizeShadow = FurthestPointShadow - ClosestPointShadow;
            ShadowFound = true;
        }
        else
            ShadowFound = false;
    }

    private void OnDrawGizmos() {
        /*
        Gizmos.color = Color.blue;
        Gizmos.DrawCube(AveragePos, Vector3.one);
        */
    }
}
