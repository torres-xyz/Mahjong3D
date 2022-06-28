using System;
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
    IEnumerator shakeRoutine;
    Renderer thisRenderer;
    private bool isDisabling;

    //Cubes.shader variable that controls shadow intensity
    //This will be changed to a higher value, making it brighter on mouse hover.
    private float initialAmbientLightValue;
    [Range(0, 1)]
    [SerializeField] private float ambientLightValueOnHover;

    private void OnEnable()
    {
        PlayerControlls.HoveredTile += OnHoveredTile;
        Board.ClickedOnAStuckTile += OnClickedOnAStuckTile;
    }
    private void OnDisable()
    {
        PlayerControlls.HoveredTile -= OnHoveredTile;
        Board.ClickedOnAStuckTile -= OnClickedOnAStuckTile;
        
    }
    private void Start()
    {
        spinAndShrinkRoutine = SpinAndShrink();
        shakeRoutine = ShakeCoroutine(new Vector3(.15f, .15f, .15f), .05f, .5f);
    }

    private void OnClickedOnAStuckTile(object sender, Tile tile)
    {
        if (isDisabling == true)
            return;

        if (tile == this)
        {
            StopCoroutine(shakeRoutine);
            shakeRoutine = ShakeCoroutine(new Vector3(.15f, .15f, .15f), .05f, .5f);
            StartCoroutine(shakeRoutine);
        }
    }

    private void OnHoveredTile(object sender, Transform trans)
    {
        if (isDisabling == true)
            return;

        if (trans == transform)
        {
            thisRenderer.material.SetFloat("_AmbientLight", ambientLightValueOnHover);
        }
        else
        {
            thisRenderer.material.SetFloat("_AmbientLight", initialAmbientLightValue);
        }
    }

    public void Init(TileType type)
    {
        if (spinAndShrinkRoutine != null)
            StopCoroutine(spinAndShrinkRoutine);
        if (shakeRoutine != null)
            StopCoroutine(shakeRoutine);

        if (thisRenderer == null)
            thisRenderer = GetComponent<Renderer>();


        tileType = type;
        thisRenderer.material = tileTypeMaterial[(int)type];

        //For the shader that highlights the tile on mouse hover
        initialAmbientLightValue = thisRenderer.material.GetFloat("_AmbientLight");

        boxCollider.enabled = true;
        transform.localScale = Vector3.one;
        transform.rotation = Quaternion.identity;

        isDisabling = false;

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

        isDisabling = true;

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
        isDisabling = false;
        //Disable at the end of the anim
        gameObject.SetActive(false);
    }


    private IEnumerator ShakeCoroutine(Vector3 magnitude, float duration, float wavelength)
    {
        Vector3 startPos = transform.localPosition;
        float endTime = Time.time + duration;
        float currentX = 0;

        while (Time.time < endTime)
        {
            Vector3 shakeAmount = new Vector3(
                Mathf.PerlinNoise(currentX, 0) - .5f,
                Mathf.PerlinNoise(currentX, 7) - .5f,
                Mathf.PerlinNoise(currentX, 19) - .5f
            );

            transform.localPosition = Vector3.Scale(magnitude, shakeAmount) + startPos;
            currentX += wavelength;
            yield return null;
        }

        transform.localPosition = startPos;
    }
}
