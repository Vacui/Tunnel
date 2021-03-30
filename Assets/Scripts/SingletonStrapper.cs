/* based on tutorial made by Thousand Ant
 * link: https://www.youtube.com/watch?v=tcatvGLvCDc
 * */

using UnityEngine;

public class SingletonStrapper : MonoBehaviour
{
    [SerializeField] private GameObject singletonPrefab;

    private void Awake()
    {
        if (Singletons.main == null)
            if (singletonPrefab != null)
                Instantiate(singletonPrefab);
            else
                Debug.LogWarning("Singleton Prefab is null", gameObject);

        Destroy(this.gameObject);
    }
}