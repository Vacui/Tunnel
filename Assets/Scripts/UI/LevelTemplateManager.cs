using UnityEngine;

public class LevelTemplateManager : MonoBehaviour
{
    public void LoadTemplate(TabButton button)
    {
        if (button)
        {
            LevelTemplateElement lvlTemplateElement = button.GetComponent<LevelTemplateElement>();
            if (lvlTemplateElement)
            {
                lvlTemplateElement.GetSize(out int width, out int height);
                MapManager.Instance.InitializeMap(width, height);
            }
        }
    }
}
