using Core;
using Enemies;
using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
/// Displays a ui element displaying the amount of money the player has earned from killing an enemy.
/// </summary>
public class KilledEnemyMoneyDisplay : MonoBehaviour
{
    EventBus eventBus;

    [SerializeField] private GameObject moneyDisplayPrefab;
    [SerializeField] private float displayTime = 2f;
    [SerializeField] private float riseSpeed = 1f;

    private void Start()
    {
        eventBus = EventBus.Instance;
        eventBus.Subscribe("EnemyKilled", OnEnemyKilled);
    }

    private void OnEnemyKilled(object obj)
    {
        Enemy enemy = (Enemy)obj;

        if (enemy == null)
        {
            Debug.LogError($"Error in {this.name} Enemy is null!");
            return;
        }

        GameObject moneyDisplay = Instantiate(moneyDisplayPrefab, transform.position, Quaternion.identity);

        moneyDisplay.transform.position = (enemy.transform.position);

        moneyDisplay.GetComponentInChildren<TextMeshProUGUI>().text = "+" + enemy.carriedMoney.ToString();

        StartCoroutine(DestoryMoneyDisplay(moneyDisplay));
    }

    private IEnumerator DestoryMoneyDisplay(GameObject display)
    {
        float currentDisplayTime = displayTime;

        while (true)
        {
            display.transform.position += new Vector3(0, riseSpeed * Time.deltaTime, 0);

            if (currentDisplayTime <= 0)
            {
                Destroy(display);
                yield break;
            }

            currentDisplayTime -= Time.deltaTime;

            yield return null;
        }
    }
}
