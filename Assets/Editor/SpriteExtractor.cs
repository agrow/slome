using UnityEngine;
using UnityEditor;
using System.IO;

public class SpriteExtractor
{
    [MenuItem("Assets/Extract Sprite to Texture2D", true)]
    private static bool ValidateExtract()
    {
        return Selection.activeObject is Sprite;
    }

    [MenuItem("Assets/Extract Sprite to Texture2D")]
    private static void ExtractSelectedSprite()
    {
        Sprite sprite = Selection.activeObject as Sprite;

        if (sprite == null)
        {
            Debug.LogError("Selected object is not a sprite.");
            return;
        }

        // Extract the sub-region from the sprite sheet
        Texture2D sourceTex = sprite.texture;
        Rect rect = sprite.rect;

        Texture2D newTex = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGBA32, false);
        Color[] pixels = sourceTex.GetPixels((int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height);
        newTex.SetPixels(pixels);
        newTex.Apply();

        // Save the texture as a PNG file
        string sourcePath = AssetDatabase.GetAssetPath(sprite);
        string folder = Path.GetDirectoryName(sourcePath);
        string spriteName = sprite.name;
        string newPath = EditorUtility.SaveFilePanel("Save Extracted Texture", folder, spriteName + "_extracted", "png");

        if (string.IsNullOrEmpty(newPath)) return;

        byte[] pngData = newTex.EncodeToPNG();
        File.WriteAllBytes(newPath, pngData);
        Debug.Log("Extracted texture saved to: " + newPath);

        AssetDatabase.Refresh();
    }
}
