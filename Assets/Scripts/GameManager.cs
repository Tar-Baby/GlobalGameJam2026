using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private GameOverFade gameOverFade;

    public enum PlayerForm
    {
        Jaguar,
        Condor,
        Serpiente,
        Mask
    }

    public PlayerForm CurrentForm { get; private set; } = PlayerForm.Mask;

    public event Action<PlayerForm> PlayerFormChanged;

    void Awake()
    {
        Time.timeScale = 1f;

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void SetPlayerForm(PlayerForm form)
    {
        if (CurrentForm == form)
        {
            return;
        }

        CurrentForm = form;
        PlayerFormChanged?.Invoke(CurrentForm);
    }

    public void CyclePlayerForm(int direction)
    {
        int count = Enum.GetValues(typeof(PlayerForm)).Length;
        int newIndex = ((int)CurrentForm + direction + count) % count;
        SetPlayerForm((PlayerForm)newIndex);
    }

    public void GameOver()
    {
        Time.timeScale = 0f;

        MusicManager musicManager = FindObjectOfType<MusicManager>();
        if (musicManager != null)
        {
            musicManager.PlayGameOverJingle();
        }

        gameOverFade.Show();
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}