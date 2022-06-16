using UnityEngine;

public class SpinCubeIntroScene : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private Vector3 angle;
    void Update()
    {
        transform.Rotate(angle, speed * Time.deltaTime);
    }
}
