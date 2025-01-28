using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Enemies
{
    /// <summary>
    /// Generates a path from one point to another, with some deviation
    /// </summary>
    public class RoadPathCreator : MonoBehaviour
    {
        public static RoadPathCreator Instance;

        private NavMeshPath path;
        private List<Vector3> subdividedPath;
        private List<Vector3> modifiedPath;

        public float maxWayPointDistance = 1;
        public float maxDeviation = 0.2f;
        public float maxDelta = 0.2f;

        public bool showSubdivided = false;
        public bool showModified = false;
        public float maxDistance;

        void Awake()
        {
            path = new NavMeshPath();
        }

        void Start()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Debug.LogError("Multiple RoadPathCreator instances found!");
                Destroy(this);
            }
        }


        /// <summary>
        /// Creates a path from the start point to the end point, with some deviation
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="endPoint"></param>
        /// <returns></returns>
        public List<Vector3> CreatePath(Vector3 startPoint, Vector3 endPoint)
        {
            // Debug.Log($"Creating path from {startPoint} to {endPoint}");

            path ??= new NavMeshPath();

            List<Vector3> result = new List<Vector3>();

            if (NavMesh.CalculatePath(startPoint, endPoint, NavMesh.AllAreas, path) && path.status == NavMeshPathStatus.PathComplete)
            {
                subdividedPath = SubdividePath(path.corners, maxWayPointDistance);
                modifiedPath = ModifyPath(subdividedPath, maxDeviation);
                result = modifiedPath;
            }
            else
            {
                Debug.Log("No path found");

            }

            return result;
        }

        /// <summary>
        /// Subdivides the path into smaller segments
        /// </summary>
        /// <param name="corners"></param>
        /// <param name="maxDistance"></param>
        /// <returns></returns>
        private List<Vector3> SubdividePath(Vector3[] corners, float maxDistance)
        {
            List<Vector3> result = new List<Vector3>();

            for (int i = 0; i < corners.Length - 1; i++)
            {
                result.Add(corners[i]);

                Vector3 delta = (corners[i + 1] - corners[i]);
                float distance = delta.magnitude;

                if (distance <= maxDistance) continue;

                int stepCount = (int)(distance / maxDistance);
                Vector3 stepSize = delta.normalized * maxDistance;

                for (int j = 1; j < stepCount; j++)
                {
                    result.Add(corners[i] + stepSize * j);
                }
            }

            result.Add(corners[corners.Length - 1]);

            return result;
        }

        /// <summary>
        /// Modifies the path by adding some deviation to each point
        /// </summary>
        /// <param name="subdividedPath"></param>
        /// <param name="maxDeviation"></param>
        /// <returns></returns>
        private List<Vector3> ModifyPath(List<Vector3> subdividedPath, float maxDeviation)
        {
            //first copy the original path
            List<Vector3> result = new List<Vector3>(subdividedPath.Count);

            result.Add(subdividedPath[0]);

            float lastOffset = 0;

            //now modify every point except the first and last point
            for (int i = 1; i < subdividedPath.Count - 1; i++)
            {
                Vector3 previous = subdividedPath[i - 1];
                Vector3 next = subdividedPath[i + 1];

                Vector3 sideAxis = Vector3.Cross(Vector3.forward, next - previous).normalized;

                float targetOffset = Random.Range(-maxDeviation, maxDeviation);
                lastOffset = Mathf.MoveTowards(lastOffset, targetOffset, maxDelta);

                Vector3 newPoint = subdividedPath[i] + sideAxis * lastOffset;

                if (NavMesh.SamplePosition(newPoint, out _, maxDistance, 1))
                {
                    result.Add(newPoint);
                }
                else
                {
                    result.Add(subdividedPath[i]);
                }
            }

            result.Add(subdividedPath[subdividedPath.Count - 1]);

            return result;
        }

        private void OnDrawGizmos()
        {
            if (path != null)
            {
                Vector3[] corners = path.corners;

                for (int i = 0; i < corners.Length; i++)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawSphere(path.corners[i] + Vector3.up * 0.1f, 0.1f);
                }
            }

            if (showSubdivided && subdividedPath != null)
            {
                foreach (Vector3 location in subdividedPath)
                {
                    Gizmos.color = Color.blue;
                    Gizmos.DrawSphere(location, 0.1f);
                }
            }

            if (showModified && modifiedPath != null)
            {
                foreach (Vector3 location in modifiedPath)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawSphere(location, 0.1f);
                }
            }

            if (showSubdivided && showModified && subdividedPath != null && modifiedPath != null && subdividedPath.Count == modifiedPath.Count)
            {
                for (int i = 0; i < subdividedPath.Count; i++)
                {
                    Debug.DrawLine(subdividedPath[i], modifiedPath[i]);
                }
            }
        }
    }
}