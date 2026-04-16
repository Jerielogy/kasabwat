using UnityEngine;
using TMPro;
using System.Collections;

public class CreepyTextPopUp : MonoBehaviour
{
    public GameObject textPrefab; // Drag your prefab here
    public RectTransform canvasRect; // Drag your Canvas here

    private string[] messages = {
        "Being watched",
        "Help!",
        "I know where you are!",
        "Shut your mouth",
        "You are not safe!",
        "Only do your task!",
        "ACCOMPLICE!",
        "Murderer!"
    };

    void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            // Wait for a random amount of time between pop-ups
            yield return new WaitForSeconds(Random.Range(1f, 5f));

            SpawnText();
        }
    }

    void SpawnText()
    {
        // 1. Create the text object
        GameObject go = Instantiate(textPrefab, canvasRect);

        // 2. Set random message
        go.GetComponent<TextMeshProUGUI>().text = messages[Random.Range(0, messages.Length)];

        // 3. Set random position within screen bounds
        RectTransform rt = go.GetComponent<RectTransform>();
        float width = canvasRect.rect.width / 2;
        float height = canvasRect.rect.height / 2;
        rt.anchoredPosition = new Vector2(Random.Range(-width, width), Random.Range(-height, height));

        // 4. Make it disappear after a split second (flicker effect)
        Destroy(go, Random.Range(0.5f, 2.0f));
    }
}