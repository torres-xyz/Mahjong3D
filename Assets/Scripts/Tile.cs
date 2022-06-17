using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Tile : MonoBehaviour
{
    [HideInInspector] public TileType tileType;
    [HideInInspector] public Vector3Int boardPosition;

    [SerializeField] List<Material> tileTypeMaterial; //already in the same order as the enum
    [SerializeField] BoxCollider boxCollider;
    [SerializeField] AnimationCurve animationCurve;
    IEnumerator spinAndShrinkRoutine;
    Renderer thisRenderer;

    private void Start()
    {
        spinAndShrinkRoutine = SpinAndShrink();
    }

    public void Init(TileType type)
    {
        if (spinAndShrinkRoutine != null)
            StopCoroutine(spinAndShrinkRoutine);

        if (thisRenderer == null)
            thisRenderer = GetComponent<Renderer>();

        tileType = type;
        thisRenderer.material = tileTypeMaterial[(int)type];

        boxCollider.enabled = true;
        transform.localScale = Vector3.one;
        transform.rotation = Quaternion.identity;

        gameObject.SetActive(true);
    }

    public void SelfDisable()
    {
        boxCollider.enabled = false;

        StopCoroutine(spinAndShrinkRoutine);
        spinAndShrinkRoutine = SpinAndShrink();
        StartCoroutine(spinAndShrinkRoutine);
    }

    IEnumerator SpinAndShrink()
    {
        float t = 0.0f;
        float startRotation = transform.eulerAngles.y;
        float endRotation = startRotation + 180.0f;

        Vector3 initialScale = transform.localScale;
        float duration = 0.25f;
        while (t < duration)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.zero;
            float progress = animationCurve.Evaluate( t / duration);

            transform.localScale = new Vector3(
                Mathf.LerpUnclamped(initialScale.x, 0, progress),
                Mathf.LerpUnclamped(initialScale.y, 0, progress),
                Mathf.LerpUnclamped(initialScale.z, 0, progress));

            transform.eulerAngles = new Vector3(
                transform.eulerAngles.x,
                Mathf.LerpUnclamped(startRotation, endRotation, progress) % 360.0f,
                transform.eulerAngles.z);

            yield return null;
        }
        //Disable at the end of the anim
        gameObject.SetActive(false);
    }
}
