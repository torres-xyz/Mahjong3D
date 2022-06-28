using UnityEngine;

public class CameraPivot : MonoBehaviour
{
    private Camera cam;

    private void OnEnable()
    {
        Board.BoardInitialized += OnBoardInitialized;
    }
    private void OnDisable()
    {
        Board.BoardInitialized -= OnBoardInitialized;
    }

    private void OnBoardInitialized(object sender, GameLevel level)
    {
        int levelSize = Mathf.Max(level.GetLevel().GetLength(0), level.GetLevel().GetLength(1), level.GetLevel().GetLength(2));

        if (cam == null)
            cam = GetComponentInChildren<Camera>();

        switch (levelSize)
        {
            case 2:
                cam.orthographicSize = 2;
                break;
            case 3:
                cam.orthographicSize = 3;
                break;
            default:
                cam.orthographicSize = levelSize - 0.5f;
                break;
        }
    }
}
