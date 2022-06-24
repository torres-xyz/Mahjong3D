using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinBoard : MonoBehaviour
{
    [SerializeField] private float boardSwipeRotationDuration = 0.5f;
    [SerializeField] private AnimationCurve animationCurve;
    private bool boardIsRotating;

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
        boardIsRotating = false;
    }

    private void OnSwippedLeftToRight(object sender, EventArgs e)
    {
        if (!boardIsRotating) StartCoroutine(TurnBoard(true));
    }

    private void OnSwippedRightToLeft(object sender, EventArgs e)
    {
        if (!boardIsRotating) StartCoroutine(TurnBoard(false));
    }

    IEnumerator TurnBoard(bool direction)
    {
        boardIsRotating = true;

        float startRotation = transform.eulerAngles.y;
        float endRotation = direction == true ? startRotation - 90f : startRotation + 90f;
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
        boardIsRotating = false;
    }
}
