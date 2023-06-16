using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultImageManager : MonoBehaviour
{


    public List<Sprite> defaultSpriteList = new List<Sprite>(); // The list to store the sprites.

    void Awake()
    {
        LoadSpritesFromResources("Sprites/DefaultSprite"); // Call the function at the start of the game.
    }

    void LoadSpritesFromResources(string folderName)
    {
        Sprite[] sprites = Resources.LoadAll<Sprite>(folderName); // Load all sprites from the specified folder.

        foreach (Sprite sprite in sprites)
        {
            defaultSpriteList.Add(sprite); // Add each sprite to the list.
        }
    }
}
