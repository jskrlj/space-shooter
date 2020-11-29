using UnityEngine;
using System.Collections;

public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    public static T Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this as T;
        }
        else
        {
            Destroy(gameObject);
            Debug.Log("Warning: multiple " + this + " in scene!");
            return;
        }
        OnSingletonAwake();
    }

    protected virtual void OnSingletonAwake() { }
}
