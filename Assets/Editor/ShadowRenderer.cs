using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.IO;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEditorInternal;

public class ShadowRenderer : EditorWindow
{
    private List<Tilemap> selectedTilemaps = new List<Tilemap>();
    private ReorderableList reorderableList;
    private Material material;
    private int pixelsPerTile = 16;

    [MenuItem("Tools/Custom/Render Tilemap Shadows to PNG")]
    public static void ShowWindow()
    {
        GetWindow<ShadowRenderer>("Render Tilemap Shadows to PNG");
    }

    private void OnGUI()
    {
        reorderableList.DoLayoutList();
        material = (Material)EditorGUILayout.ObjectField("Shader Material", material, typeof(Material), false);
        pixelsPerTile = EditorGUILayout.IntSlider("Pixels Per Tile", pixelsPerTile, 1, 64);

        if (GUILayout.Button("Render"))
        {
            if (selectedTilemaps != null && selectedTilemaps.Count > 0)
            {
                if (material)
                {
                    RenderTilemapToPNG(selectedTilemaps, material, pixelsPerTile);
                }
                else
                {
                    EditorUtility.DisplayDialog("Error", "Please select a Material.", "OK");
                }
            }
            else
            {
                EditorUtility.DisplayDialog("Error", "Please select a Tilemap.", "OK");
            }
        }
    }

    private void OnEnable()
    {
        selectedTilemaps = new List<Tilemap>(Selection.GetFiltered<Tilemap>(SelectionMode.Editable | SelectionMode.Deep));
        reorderableList = new ReorderableList(selectedTilemaps, typeof(Tilemap), true, true, true, true);
        material = AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/Materials/SolidTileShadow.mat");

        reorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            rect.y += 2;
            rect.height = EditorGUIUtility.singleLineHeight;
            selectedTilemaps[index] = (Tilemap)EditorGUI.ObjectField(rect, $"Tilemap {index + 1}", selectedTilemaps[index], typeof(Tilemap), true);
        };

        reorderableList.drawHeaderCallback = (Rect rect) => EditorGUI.LabelField(rect, "Tilemaps");
        reorderableList.onAddCallback = (ReorderableList list) => selectedTilemaps.Add(null);
        reorderableList.onRemoveCallback = (ReorderableList list) => selectedTilemaps.RemoveAt(list.index);
    }

    private void RenderTilemapToPNG(List<Tilemap> tilemaps, Material material, int pixelsPerTile)
    {
        // Set up a RenderTexture to capture the Tilemap
        Bounds bounds = new Bounds(tilemaps[0].cellBounds.center, tilemaps[0].cellBounds.size);
        foreach (Tilemap tilemap in tilemaps)
        {
            bounds.Encapsulate(new Bounds(tilemap.cellBounds.center, tilemap.cellBounds.size));
        }
        BoundsInt tileBounds = new BoundsInt(Vector3Int.FloorToInt(bounds.min), Vector3Int.FloorToInt(bounds.size));
        int width = tileBounds.size.x;
        int height = tileBounds.size.y;
        int texWidth = width * pixelsPerTile;
        int texHeight = height * pixelsPerTile;

        RenderTexture renderTexture = new RenderTexture(texWidth, texHeight, 24);
        renderTexture.enableRandomWrite = true;
        renderTexture.Create();

        // Create a temporary camera to render the Tilemap
        Camera tempCamera = new GameObject("TempCamera").AddComponent<Camera>();
        tempCamera.orthographic = true;
        tempCamera.orthographicSize = height / 2f;
        tempCamera.transform.position = new Vector3(tileBounds.center.x, tileBounds.center.y, -10f);
        tempCamera.cullingMask = LayerMask.GetMask("SolidTiles", "FakeTiles");
        tempCamera.backgroundColor = new Color(1, 1, 1, 0);
        tempCamera.targetTexture = renderTexture;

        // Render the Tilemap into the RenderTexture using the temporary camera
        GL.Clear(true, true, Color.clear);
        tempCamera.Render();

        //Apply shader
        RenderTexture blurredRenderTexture = new RenderTexture(renderTexture);
        Graphics.Blit(renderTexture, blurredRenderTexture, material, 0);

        // Create a Texture2D from the RenderTexture
        Texture2D texture = new Texture2D(texWidth, texHeight, TextureFormat.RGB24, false);
        RenderTexture.active = blurredRenderTexture;
        texture.ReadPixels(new Rect(0, 0, texWidth, texHeight), 0, 0);
        texture.Apply();
        RenderTexture.active = null;

        // Save the texture to PNG
        string path = EditorUtility.SaveFilePanelInProject("Save Tilemap Shadows as PNG", SceneManager.GetActiveScene().name + "_Shadows", "png", "Save shadow texture", "Assets/Art/World/Shadows");
        if (!string.IsNullOrEmpty(path))
        {
            File.WriteAllBytes(path, texture.EncodeToPNG());

            AssetDatabase.Refresh(); // Import the new PNG

            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null)
            {
                importer.maxTextureSize = Mathf.NextPowerOfTwo(Mathf.Max(texWidth, texHeight));
                importer.spritePixelsPerUnit = pixelsPerTile;

                TextureImporterSettings settings = new TextureImporterSettings();
                importer.ReadTextureSettings(settings);
                settings.alphaSource = TextureImporterAlphaSource.FromGrayScale;
                settings.spriteAlignment = (int)SpriteAlignment.Custom;
                settings.spritePivot = new Vector2(-tileBounds.xMin * (1.0f / width), -tileBounds.yMin * (1.0f / height));
                importer.SetTextureSettings(settings);

                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            }
        }

        // Clean up resources
        DestroyImmediate(tempCamera.gameObject);
        DestroyImmediate(renderTexture);
        DestroyImmediate(blurredRenderTexture);
        DestroyImmediate(texture);
    }
}
