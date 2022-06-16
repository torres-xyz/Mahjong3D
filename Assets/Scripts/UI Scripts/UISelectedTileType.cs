using CustomHelperFunctions;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class UISelectedTileType : MonoBehaviour
{
    [SerializeField] private List<Texture> tileTypesImages;
    [SerializeField] private RawImage image;
    private Texture2D oneTransparentPixel;

    private void OnEnable()
    {
        Board.ClickedOnAFreeTile += OnPlayerClickedOnAFreeTile;
        GameManager.PlayerFoundTilePair += OnPlayerFoundTilePair;
    }
    private void OnDisable()
    {
        Board.ClickedOnAFreeTile -= OnPlayerClickedOnAFreeTile;
        GameManager.PlayerFoundTilePair -= OnPlayerFoundTilePair;
    }
    private void Start()
    {
        oneTransparentPixel = UnityHelperFunctions.OneTransparentPixel();
        ResetImage();
    }

    void OnPlayerFoundTilePair(object sender, (Tile, Tile) e) => ResetImage();
    void OnPlayerClickedOnAFreeTile(object sender, Tile tileClicked) => UpdateSelectedTileTypeUI(tileClicked.tileType);
    void ResetImage() => image.texture = oneTransparentPixel; //Ideally, with more time and polish, something else would go here
    void UpdateSelectedTileTypeUI(TileType type) => image.texture = tileTypesImages[(int)type]; //The list is already in the same order as the TileType enum
}
