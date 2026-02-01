using UnityEngine;

public class PlayerFormSwitcher : MonoBehaviour
{
    [Header("Form Roots (children)")]
    [SerializeField] private GameObject jaguarRoot;
    [SerializeField] private GameObject condorRoot;
    [SerializeField] private GameObject serpienteRoot;
    [SerializeField] private GameObject maskRoot;

    public Animator ActiveAnimator { get; private set; }
    public JaguarCombat ActiveJaguarCombat { get; private set; }
    public Condor ActiveCondor { get; private set; }
    public Serpiente ActiveSerpiente { get; private set; }

    void Start()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager no encontrado");
            return;
        }

        GameManager.Instance.PlayerFormChanged += ApplyForm;
        ApplyForm(GameManager.Instance.CurrentForm);
    }

    void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.PlayerFormChanged -= ApplyForm;
        }
    }

    void ApplyForm(GameManager.PlayerForm form)
    {
        SetActiveOnly(form);

        ActiveAnimator = GetRoot(form).GetComponent<Animator>();

        ActiveJaguarCombat = GetRoot(form).GetComponent<JaguarCombat>();
        ActiveCondor = GetRoot(form).GetComponent<Condor>();
        ActiveSerpiente = GetRoot(form).GetComponent<Serpiente>();

        // Hooks de activación/desactivación de habilidades pasivas
        if (form == GameManager.PlayerForm.Serpiente)
        {
            ActiveSerpiente?.EnablePhase();
        }
        else
        {
            ActiveSerpiente?.DisablePhase();
        }

        if (form != GameManager.PlayerForm.Condor)
        {
            ActiveCondor?.ResetGlide();
        }
    }

    void SetActiveOnly(GameManager.PlayerForm form)
    {
        jaguarRoot.SetActive(form == GameManager.PlayerForm.Jaguar);
        condorRoot.SetActive(form == GameManager.PlayerForm.Condor);
        serpienteRoot.SetActive(form == GameManager.PlayerForm.Serpiente);
        maskRoot.SetActive(form == GameManager.PlayerForm.Mask);
    }

    GameObject GetRoot(GameManager.PlayerForm form)
    {
        return form switch
        {
            GameManager.PlayerForm.Jaguar => jaguarRoot,
            GameManager.PlayerForm.Condor => condorRoot,
            GameManager.PlayerForm.Serpiente => serpienteRoot,
            GameManager.PlayerForm.Mask => maskRoot,
            _ => jaguarRoot,
        };
    }
}