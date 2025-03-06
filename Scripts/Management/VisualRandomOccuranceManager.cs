using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameManagement
{
    using Interactables;

    public class VisualRandomOccuranceManager : MonoBehaviour
    {
        [SerializeField] private List<RandomOccurance> randomOccurances = new();

        private void Start()
        {
            if (randomOccurances.Count == 0)
            {
                Debug.LogWarning("No random occurances set up for this level");
                return;
            }

            StartCoroutine(RollRandomOccurances());
        }

        private IEnumerator RollRandomOccurances()
        {
            while (true)
            {
                foreach (var occurance in randomOccurances)
                {
                    if (Random.Range(0, 1000) <= occurance.OccuranceChance)
                    {
                        var occuranceInstance = Instantiate(occurance.OccurancePrefab);   
                        StartCoroutine(DestroyAfterTime(occuranceInstance, occurance.OccuranceDuration));
                    }
                }

                yield return new WaitForSeconds(1f);
            }
        }

        private IEnumerator DestroyAfterTime(GameObject gameObject, float time)
        {
            yield return new WaitForSeconds(time);
            Destroy(gameObject);
        }
    }
}
