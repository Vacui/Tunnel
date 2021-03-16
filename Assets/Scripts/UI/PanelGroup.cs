/* based on tutorial by GameDevGuide
 * source: https://www.youtube.com/watch?v=CGsEJToeXmA
 * */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelGroup : MonoBehaviour
{
    [SerializeField] GameObject[] panels;
    private int panelIndex;

    private void Awake()
    {
        DeselectAllPanels();
        panelIndex = 0;
    }

    public void SetPanelIndex(int value)
    {
        if (panels != null)
        {
            value = Mathf.Clamp(value, 0, panels.Length);

            if (value != panelIndex)
                panels[panelIndex].SetActive(false);

            panelIndex = value;
            panels[panelIndex].SetActive(true);
        }
    }

    private void DeselectAllPanels()
    {
        if(panels != null)
        {
            for(int i= 0; i < panels.Length; i++)
            {
                panels[i].SetActive(false);
            }
        }
    }
}