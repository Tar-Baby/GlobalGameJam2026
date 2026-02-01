using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour
{
    [Header("Music By Form")]
    [SerializeField] private AudioClip humaMusic;
    [SerializeField] private AudioClip jaguarMusic;
    [SerializeField] private AudioClip condorMusic;
    [SerializeField] private AudioClip serpienteMusic;

    [Header("Jingles")]
    [SerializeField] private AudioClip gameOverJingle;

    [Header("Fade Settings")]
    [SerializeField] private float fadeDuration = 1.5f;

    private AudioSource sourceA;
    private AudioSource sourceB;
    private AudioSource jingleSource;

    private AudioSource activeSource;
    private Coroutine fadeCoroutine;

    void Awake()
    {
        AudioSource[] sources = GetComponents<AudioSource>();

        if (sources.Length < 3)
        {
            int toAdd = 3 - sources.Length;
            for (int i = 0; i < toAdd; i++)
            {
                gameObject.AddComponent<AudioSource>();
            }

            sources = GetComponents<AudioSource>();
        }

        sourceA = sources[0];
        sourceB = sources[1];
        jingleSource = sources[2];

        sourceA.loop = true;
        sourceB.loop = true;

        sourceA.playOnAwake = false;
        sourceB.playOnAwake = false;
        jingleSource.playOnAwake = false;

        activeSource = sourceA;
    }

    void OnEnable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.PlayerFormChanged += OnPlayerFormChanged;
        }
    }

    void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.PlayerFormChanged -= OnPlayerFormChanged;
        }
    }

    // =========================
    // MUSIC BY FORM (FADE)
    // =========================

    void OnPlayerFormChanged(GameManager.PlayerForm form)
    {
        ChangeMusic(form);
    }

    void ChangeMusic(GameManager.PlayerForm form)
    {
        AudioClip newClip = GetMusicClip(form);

        if (newClip == null || activeSource.clip == newClip)
        {
            return;
        }

        AudioSource newSource = activeSource == sourceA ? sourceB : sourceA;

        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        fadeCoroutine = StartCoroutine(CrossFade(newSource, newClip));
    }

    IEnumerator CrossFade(AudioSource newSource, AudioClip newClip)
    {
        newSource.clip = newClip;
        newSource.volume = 0f;
        newSource.Play();

        float time = 0f;
        AudioSource oldSource = activeSource;

        while (time < fadeDuration)
        {
            time += Time.unscaledDeltaTime;
            float t = time / fadeDuration;

            newSource.volume = Mathf.Lerp(0f, 1f, t);
            oldSource.volume = Mathf.Lerp(1f, 0f, t);

            yield return null;
        }

        oldSource.Stop();
        oldSource.volume = 1f;

        activeSource = newSource;
    }

    // =========================
    // GAME OVER
    // =========================

    public void PlayGameOverJingle()
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        sourceA.Stop();
        sourceB.Stop();

        jingleSource.clip = gameOverJingle;
        jingleSource.Play();
    }

    // =========================
    // CLIP RESOLUTION
    // =========================

    AudioClip GetMusicClip(GameManager.PlayerForm form)
    {
        switch (form)
        {
            case GameManager.PlayerForm.Mask:
                return humaMusic;

            case GameManager.PlayerForm.Jaguar:
                return jaguarMusic;

            case GameManager.PlayerForm.Condor:
                return condorMusic;

            case GameManager.PlayerForm.Serpiente:
                return serpienteMusic;

            default:
                return null;
        }
    }
}