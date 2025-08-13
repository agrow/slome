using UnityEngine;
using UnityEditor;
using System.IO;

public class SpriteToTextureUtility : EditorWindow
{
    Sprite selectedSprite;
    string outputName = "ExtractedTexture";

    [MenuItem("Tools/Sprite to Texture2D")]
    public static void ShowWindow()
    {
        GetWindow<SpriteToTextureUtility>("Sprite To Texture2D");
    }

    void OnGUI()
    {
        GUILayout.Label("Convert Sprite to Texture2D", EditorStyles.boldLabel);
        selectedSprite = (Sprite)EditorGUILayout.ObjectField("Sprite", selectedSprite, typeof(Sprite), false);
        outputName = EditorGUILayout.TextField("Output Name", outputName);

        if (GUILayout.Button("Convert and Save Texture"))
        {
            if (selectedSprite != null)
            {
                SaveSpriteAsTexture(selectedSprite, outputName);
            }
            else
            {
                Debug.LogWarning("No sprite selected.");
            }
        }
    }

    void SaveSpriteAsTexture(Sprite sprite, string name)
    {
        Texture2D newTex = ExtractTextureFromSprite(sprite);

        byte[] pngData = newTex.EncodeToPNG();
        string path = EditorUtility.SaveFilePanel("Save Texture As PNG", "Assets/", name, "png");

        if (!string.IsNullOrEmpty(path))
        {
            File.WriteAllBytes(path, pngData);
            Debug.Log($"Texture saved to {path}");

            // Refresh the AssetDatabase to make Unity recognize the new file
            AssetDatabase.Refresh();
        }
    }

    Texture2D ExtractTextureFromSprite(Sprite sprite)
    {
        Rect r = sprite.rect;
        Texture2D sourceTex = sprite.texture;

        // Make readable copy of sub-region
        Texture2D newTex = new Texture2D((int)r.width, (int)r.height, TextureFormat.RGBA32, false);
        newTex.SetPixels(sourceTex.GetPixels(
            (int)r.x,
            (int)r.y,
            (int)r.width,
            (int)r.height));
        newTex.Apply();
        return newTex;
    }
}
