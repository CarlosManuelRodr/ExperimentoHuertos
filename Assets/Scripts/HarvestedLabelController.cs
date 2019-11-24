using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HarvestedLabelController : MonoBehaviour
{
    public GameObject experiment;
    public Player owner = Player.PlayerA;
    private ExperimentManager experimentManager;
    private TextMeshProUGUI valueLabel;
    private uint score = 0;

    // Start is called before the first frame update
    void Start()
    {
        valueLabel = GetComponent<TextMeshProUGUI>();
        if (experiment != null)
            experimentManager = experiment.GetComponent<ExperimentManager>();
        else
            Debug.LogWarning("HarvestedLabelController unconnected to experiment");
    }

    // Update is called once per frame
    void Update()
    {
        if (owner == Player.PlayerA)
        {
            if (experimentManager.harvestedA != score)
            {
                score = experimentManager.harvestedA;
                valueLabel.text = score.ToString();
            }
        }
        else
        {
            if (experimentManager.harvestedB != score)
            {
                score = experimentManager.harvestedB;
                valueLabel.text = score.ToString();
            }
        }
    }
}
