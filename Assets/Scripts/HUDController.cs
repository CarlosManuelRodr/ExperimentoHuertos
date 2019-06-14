using UnityEngine;

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
        endExperiment.Restore(); // Just in case
    }
}
