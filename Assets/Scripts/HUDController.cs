using UnityEngine;

/// <summary>
/// Controlador del HUD (Head Up Display).
/// </summary>
public class HUDController : MonoBehaviour
{
    public GameObject endExperimentButton;
    private EndExperimentButton endExperiment;

    void Awake()
    {
        endExperiment = endExperimentButton.GetComponent<EndExperimentButton>();
    }

    void OnDisable()
    {
        endExperiment.Restore(); // Por si acaso restaura el tamaño original del botón.
    }
}
