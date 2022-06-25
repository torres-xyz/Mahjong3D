using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControlls : MonoBehaviour
{
    //Events
    public static EventHandler<Transform> ClickedOnTile;
    public static EventHandler<Transform> HoveredTile;
    public static EventHandler SwippedRightToLeft;
    public static EventHandler SwippedLeftToRight;

    [Tooltip("Drag Threshold is in percentage of resolution traveled.")]
    [Range(0, 1)]
    [SerializeField] private float dragScreenPercentageTraveledThreshold;
    private Vector2 mousePosition;
    private Vector2 mousePositionAtStartOfSwipe;
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
        mousePosition = Vector2.zero;
    }

    void OnClick(InputValue value)
    {
        if (value.isPressed)
        {
            CalculateSwipe(clickState: true);
            return;
        }

        bool isPlayerSwipping = CalculateSwipe(clickState: false);
        if (isPlayerSwipping) //Only register a click if player is not currently Swipping the screen
            return;

        Transform objHit = RaycastToObject(mousePosition);
        if (objHit != null)
        {
            //Debug.Log(objHit.name);
            //Debug.Log(objHit.GetComponent<Tile>().tileType.ToString());
            ClickedOnTile?.Invoke(this, objHit);
        }
    }
    void OnPoint(InputValue value)
    {
        mousePosition = value.Get<Vector2>();
        Transform objHit = RaycastToObject(mousePosition);
        if (objHit != null)
        {
            //Debug.Log(objHit.name);
            //Debug.Log(objHit.GetComponent<Tile>().tileType.ToString());
            HoveredTile?.Invoke(this, objHit);
        }

    }

    bool CalculateSwipe(bool clickState)
    {
        if (clickState == true) //swipe not started, mouse is down
        {
            mousePositionAtStartOfSwipe = mousePosition;
            return false;
        }

        //Mouse is now up
        if (Math.Abs(mousePositionAtStartOfSwipe.x - mousePosition.x) < (dragScreenPercentageTraveledThreshold * Screen.width))
            return false; //Swipe distance was too small. Swipe not started

        //swipe started
        if (mousePositionAtStartOfSwipe.x > mousePosition.x)
            SwippedRightToLeft?.Invoke(this, EventArgs.Empty);
        else
            SwippedLeftToRight?.Invoke(this, EventArgs.Empty);

        return true;
    }


    Transform RaycastToObject(Vector2 mousePos)
    {
        if (Physics.Raycast(cam.ScreenPointToRay(mousePos), out RaycastHit hit))
            return hit.transform;

        return null;
    }
}