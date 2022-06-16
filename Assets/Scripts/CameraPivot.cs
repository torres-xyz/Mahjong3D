using System;
using System.Collections;
using UnityEngine;

public class CameraPivot : MonoBehaviour
{
    [SerializeField] private float boardSwipeRotationDuration;
    [SerializeField] private AnimationCurve animationCurve;
    private bool cameraIsRotating;

    private void OnEnable()
    {
        PlayerControlls.SwippedRightToLeft += OnSwippedRightToLeft;
        PlayerControlls.SwippedLeftToRight += OnSwippedLeftToRight;
    }
    private void OnDisable()
    {
        PlayerControlls.SwippedRightToLeft -= OnSwippedRightToLeft;
        PlayerControlls.SwippedLeftToRight -= OnSwippedLeftToRight;
    }
    private void Start()
    {
        cameraIsRotating = false;
    }

    private void OnSwippedLeftToRight(object sender, EventArgs e)
    {
        if (!cameraIsRotating) StartCoroutine(TurnBoard(true));
    }

    private void OnSwippedRightToLeft(object sender, EventArgs e)
    {
        if (!cameraIsRotating) StartCoroutine(TurnBoard(false));
    }

    IEnumerator TurnBoard(bool direction)
    {
        cameraIsRotating = true;

        float startRotation = transform.eulerAngles.y;
        float endRotation = direction == true ? startRotation + 90f : startRotation - 90f;
        float t = 0.0f;
        while (t < boardSwipeRotationDuration)
        {
            t += Time.deltaTime;
            float yRotation = Mathf.Lerp(startRotation, endRotation, animationCurve.Evaluate(t / boardSwipeRotationDuration)) % 360.0f;
            transform.eulerAngles = new Vector3(
                transform.eulerAngles.x,
                yRotation,
                transform.eulerAngles.z);
            yield return null;
        }
        cameraIsRotating = false;
    }
}
