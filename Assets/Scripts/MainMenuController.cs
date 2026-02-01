using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenuController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject pressAnyKeyText;
    [SerializeField] private GameObject loreImage;

    [Header("Settings")]
    [SerializeField] private float loreDisplayTime = 2.5f;
    [SerializeField] private string tutorialSceneName = "Tutorial";

    private bool hasStarted;

    void Start()
    {
        if (loreImage != null)
        {
            loreImage.SetActive(false);
        }
    }

    void Update()
    {
        if (hasStarted)
        {
            return;
        }

        if (Input.anyKeyDown)
        {
            StartCoroutine(StartGameSequence());
        }
    }

    IEnumerator StartGameSequence()
    {
        hasStarted = true;

        if (pressAnyKeyText != null)
        {
            pressAnyKeyText.SetActive(false);
        }

        if (loreImage != null)
        {
            loreImage.SetActive(true);
        }

        yield return new WaitForSeconds(loreDisplayTime);

        SceneManager.LoadScene(tutorialSceneName);
    }
}