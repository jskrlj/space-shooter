using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundManager : MonoSingleton<BackgroundManager>
{
    private const float Speed = 1f;
    private float height;
    private float startPositionY;
    private bool moving;

    protected override void OnSingletonAwake()
    {
        base.OnSingletonAwake();
        PrepareBackgrounds();
        startPositionY = 10;
    }

    public void EnableMovement(bool b) 
    {
        moving = b;
    }

    private void PrepareBackgrounds() 
    {
        height = transform.GetChild(0).GetComponent<SpriteRenderer>().bounds.size.y * 0.999f;

        for(int i = 0; i < transform.childCount; i++)
        {
            Transform background = transform.GetChild(i);

            if (i == 0)
            {
                background.position = new Vector3(0, startPositionY, background.position.z);
                //startPositionY = background.position.y;
            }
            else 
            {
                background.position = new Vector3(0, startPositionY + height * i, background.position.z);
            }
        }
    }

    private void Update()
    {
        if (moving) 
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform background = transform.GetChild(i);
                if (background.position.y < startPositionY - height)
                {
                    int highestBackgroundIndex = (i + (transform.childCount - 1)) % transform.childCount;
                    Transform highestBackground = transform.GetChild(highestBackgroundIndex);
                    background.position = new Vector3(0, highestBackground.position.y + height, background.position.z);
                }
                background.position -= new Vector3(0, Time.deltaTime * Speed, 0);
            }
        }
    }
}
