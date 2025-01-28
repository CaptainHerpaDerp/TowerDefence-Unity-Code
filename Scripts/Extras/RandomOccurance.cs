using UnityEngine;

namespace Interactables
{
    [CreateAssetMenu(fileName = "RandomOccurance", menuName = "RandomOccurance", order = 0)]
    public class RandomOccurance : ScriptableObject
    {
        [Header("A roll is made every second from 0 to 1000, if the rolled number is greater than this value, the occurance will happen")]
        [SerializeField] private float occuranceChance = 0.1f;

        [Header("The prefab that will be instantiated when the occurance happens")]
        [SerializeField] private GameObject occurancePrefab;

        [Header("The duration of the occurance in seconds, prefab destroyed after given time")]
        [SerializeField] private float occuranceDuration = 5f;

        public float OccuranceChance => occuranceChance;
        public GameObject OccurancePrefab => occurancePrefab;
        public float OccuranceDuration => occuranceDuration;
    }
}
