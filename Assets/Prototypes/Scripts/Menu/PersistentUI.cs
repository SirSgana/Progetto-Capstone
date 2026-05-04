using UnityEngine;

public class PersistentUI : MonoBehaviour
{
    public static PersistentUI instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            transform.SetParent(null); // Si sgancia dal GameManager
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}