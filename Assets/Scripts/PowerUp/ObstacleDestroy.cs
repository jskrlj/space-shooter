using UnityEngine;

public class ObstacleDestroy : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (EnemiesManager.Instance.transform.childCount < 2)
        {
            GameManager.Instance.WaveCompleted();
        }
        Destroy(collision.gameObject);
    }
}
