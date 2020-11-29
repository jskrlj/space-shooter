using UnityEngine;
using System.Collections;

public class BasicEnemy : Enemy
{
    public bool IsAggressive { get; set; }
    [SerializeField]
    private Sprite[] EnemySkins;
    [SerializeField]
    private GameObject particlesPrefab;
    private bool aggressiveManeuverInProgress;
    private GameObject childObject;
    private int enemySkinId;
    private static int[] randomSkins = new int[] { 0, 1, 2 };

    private void OnEnable()
    {
        childObject = transform.GetChild(0).gameObject;

        base.EnemyDestroyed += (int dmg, Vector3 pos) => 
        {
            GameObject particles = Instantiate(particlesPrefab);
            particles.transform.position = new Vector3(transform.position.x, transform.position.y + 0.02f, -1);
        };
    }

    public void RandomizeSkins() 
    {
        randomSkins = new int[] { Random.Range(0, EnemySkins.Length), Random.Range(0, EnemySkins.Length), Random.Range(0, EnemySkins.Length) };
    }

    public void SetEnemySkin(char skinId) 
    {

        switch (skinId) 
        {
            case 'a':
                enemySkinId = randomSkins[0];
                break;
            case 'b':
                enemySkinId = randomSkins[1];
                break;
            case 'c':
                enemySkinId = randomSkins[2];
                break;
        }

        Debug.Log("Enemy skin id " + enemySkinId.ToString());
        if (EnemySkins.Length > 0) 
        {
            childObject.GetComponent<SpriteRenderer>().sprite = EnemySkins[enemySkinId];
        }
    }

    protected override void HitByBullet(GameObject other)
    {
        base.HitByBullet(other);

        if (childObject.transform.localPosition == Vector3.zero)
            LeanTween.moveLocalY(childObject, 0.1f, .1f).setEaseOutSine();
            //childObject.transform.localPosition = new Vector3(0, .1f, 0);

        if (IsAggressive && !aggressiveManeuverInProgress && transform.localPosition == StartLocalPosition)
        {
            aggressiveManeuverInProgress = true;
            StartAggressiveManeuver();
        }
    }

    private void StartAggressiveManeuver()
    {
        Vector3 spaceShipLocalPosition = transform.parent.InverseTransformVector(SpaceShipManager.Instance.transform.position);

        //GameObject go = new GameObject();
        //go.transform.parent = transform.parent;
        //go.transform.localPosition = spaceShipLocalPosition;

        // higher score - more aggressive enemies
        float curveHeight = GameManager.Instance.CurrentScore * 0.001f; 
        float curveWidth = Random.Range(1f, 6f);

        Vector3 startPoint = transform.localPosition;
        Vector3 endPoint = StartLocalPosition;
        Vector3 startControl = spaceShipLocalPosition + new Vector3(curveWidth, -curveHeight, 0);
        Vector3 endControl = spaceShipLocalPosition + new Vector3(-curveWidth, -curveHeight, 0);

        int randomDirection = Random.Range(0, 2);
        Vector3[] bezierDirection1 = new Vector3[] { startPoint, endControl, startControl, endPoint };
        Vector3[] bezierDirection2 = new Vector3[] { startPoint, startControl, endControl, endPoint };
        Vector3[] randomCurve = (Random.Range(0, 2) == 0) ? bezierDirection1 : bezierDirection2;

        LTBezierPath ltPath = new LTBezierPath(randomCurve);

        float attackTime = 4f - (GameManager.Instance.CurrentScore * 0.001f);
        Mathf.Clamp(attackTime, 1.5f, 10f);
        LeanTween.cancel(gameObject);
        LeanTween.moveLocal(gameObject, ltPath, attackTime)
            .setEaseInOutSine()
            .setOnComplete(() => { aggressiveManeuverInProgress = false; });
    }

    private void Update()
    {
        if (!LeanTween.isTweening(childObject)) 
        {
            if (childObject.transform.localPosition.y > 0)
            {
                childObject.transform.localPosition -= new Vector3(0, .2f * Time.deltaTime, 0);
            }
            else
            {
                childObject.transform.localPosition = Vector3.zero;
            }
        }
        
    }
}

