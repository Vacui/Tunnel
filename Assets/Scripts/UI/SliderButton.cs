using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class SliderButton : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] float valueToAdd;

    private void OnEnable()
    {
        GetComponent<Button>().onClick.AddListener(AddValue);
    }

    private void OnDisable()
    {
        GetComponent<Button>().onClick.RemoveListener(AddValue);
    }

    private void AddValue()
    {
        if (slider)
            slider.value += valueToAdd;
        else
            Debug.LogWarning("Slider is null", gameObject);
    }
}