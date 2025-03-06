using UnityEngine;
using UnityEngine.AI;
using Sirenix.OdinInspector;

namespace Core
{
    /// <summary>
    /// This class is used to check if a point is on the NavMesh and to find a sample position on the NavMesh from a given position.
    /// </summary>
    public class Check2DNavMesh : MonoBehaviour
    {
        [BoxGroup("Settings"), SerializeField] private int areaMask;
        [BoxGroup("Settings"), SerializeField] private float radius;
        [BoxGroup("Settings"), SerializeField] private float minDistance;
        [BoxGroup("Settings"), SerializeField] private float maxDistance;
        [BoxGroup("Settings"), SerializeField] private float initialStepSize;
        [BoxGroup("Settings"), SerializeField] private int maxIterations;

        [BoxGroup("Debugging"), SerializeField] private GameObject displayPoint;
        [BoxGroup("Debugging"), SerializeField] private bool drawLine;

        private Vector3[] travelDirections = { Vector3.right, Vector3.left, Vector3.up, Vector3.down };
        
        /// <summary>
        /// Returns true if the specified position is on the NavMesh
        /// </summary>
        /// <param name="position"></param>
        /// <param name="sampleRadius"></param>
        /// <returns></returns>
        public bool PointOnNavMesh(Vector3 position, float sampleRadius = 0.05f)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(position, out hit, sampleRadius, areaMask))
            {
                // Check if the hit area is not in the river area
                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns the nearest point on the NavMesh from the specified position
        /// </summary>
        /// <param name="position"></param>
        /// <param name="maxDistance"></param>
        /// <returns></returns>

        public Vector3 GetNearestPointOnNavMesh(Vector3 position, float maxDistance)
        {
            int iterations = 0;
            float radius = 0.05f;

            NavMeshHit hit;
            while (iterations < maxIterations)
            {
                if (NavMesh.SamplePosition(position, out hit, radius, areaMask))
                {
                    if (Vector3.Distance(position, hit.position) < maxDistance)
                    {
                        return hit.position;
                    }
                }

                radius += 0.05f;
                iterations++;
            }

            Debug.LogError("Failed to find a sample position on the road after " + iterations + " iterations");
            return Vector3.zero;
        }

        /// <summary>
        /// Returns a sample position on the road from the specified position
        /// </summary>
        public Vector3 GetRoadSampleFromPosition(Vector3 position)
        {
            NavMeshHit hit;

            Vector3 basePosition = position;
            float stepSize = initialStepSize;
            int iterations = 0;

            bool found = false;

            while (!found && iterations < maxIterations)
            {
                for (int travelDirection = 1; travelDirection <= 4; travelDirection++)
                {
                    Vector3 direction = travelDirections[travelDirection - 1];

                    for (float distance = initialStepSize; distance <= maxDistance; distance += stepSize)
                    {
                        Vector3 nextPosition = position + direction * distance;

                        if (drawLine)
                            Instantiate(displayPoint, nextPosition, Quaternion.identity);

                        if (NavMesh.SamplePosition(nextPosition, out hit, radius, areaMask) &&
                            Vector3.Distance(position, hit.position) > minDistance && Vector3.Distance(position, hit.position) < maxDistance)
                        {
                            found = true;
                            return hit.position;
                        }
                    }

                }

                // If none of the directions worked, increase step size and try again
                stepSize *= 2f;

                // Reset position to base position
                position = basePosition;

                iterations++;
            }

            Debug.LogError("Failed to find a sample position on the road after " + iterations + " iterations");
            return position;
        }
    }

}