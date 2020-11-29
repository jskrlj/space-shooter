using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SpaceShipTouch : MonoBehaviour
{
    private bool mobile;
    private Vector2 touchOffset;

    private void Start()
    {
        mobile = Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer;
    }

    void Update()
    {
        if (EventSystem.current != null)
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;
        }

        // Handle native touch events
        if (Input.touchCount > 0)
        {
            Touch touch = Input.touches[0];
            HandleTouch(touch.fingerId, touch.position, touch.phase);
        }


        // Simulate touch events from mouse events
        if (Input.touchCount == 0 && !mobile)
        {
            if (Input.GetMouseButtonDown(0))
            {
                HandleTouch(10, Input.mousePosition, TouchPhase.Began);
            }
            if (Input.GetMouseButton(0))
            {
                HandleTouch(10, Input.mousePosition, TouchPhase.Moved);
            }
            if (Input.GetMouseButtonUp(0))
            {
                HandleTouch(10, Input.mousePosition, TouchPhase.Ended);
            }
        }
    }

    private void HandleTouch(int touchFingerId, Vector3 touchPosition, TouchPhase touchPhase)
    {
        // touch logic
        switch (touchPhase)
        {
            case TouchPhase.Began:
                Vector2 worldBeganPosition = Camera.main.ScreenToWorldPoint(touchPosition);
                touchOffset = (Vector2)transform.position - worldBeganPosition;
                break;
            case TouchPhase.Moved:
                Vector2 worldMovePosition = Camera.main.ScreenToWorldPoint(touchPosition);
                Vector3 newPosition = worldMovePosition + touchOffset;
                transform.position = CheckBorders(newPosition);
                break;
            case TouchPhase.Ended:
                break;
        }
    }

    private Vector3 CheckBorders(Vector3 newPosition)
    {
        var topRight = Camera.main.ViewportToWorldPoint(Vector3.one);
        var bottomLeft = Camera.main.ViewportToWorldPoint(Vector3.zero);
        return new Vector3(Mathf.Clamp(newPosition.x, bottomLeft.x, topRight.x),
                           Mathf.Clamp(newPosition.y, bottomLeft.y, topRight.y),
                           0);
    }
}
