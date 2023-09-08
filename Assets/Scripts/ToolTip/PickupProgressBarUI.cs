using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickupProgressBarUI : MonoBehaviour
{
    [SerializeField] private Image progressbarImage;
    private float remainingtime;
    private float duration;

    private void Update()
    {
        if (remainingtime <= duration)
        {
            progressbarImage.fillAmount += remainingtime / duration;
            remainingtime -= Time.deltaTime;
            if (remainingtime <= 0)
            {
                remainingtime = Mathf.Infinity;
            }
        }
    }
    public void UpdateProgressBar(double duration)
    {
        this.duration = (float)duration;
        remainingtime = this.duration;
        progressbarImage.fillAmount += Time.deltaTime; 
    }
}
