using UnityEngine;

public class TexturePainter : MonoBehaviour
{
    public Material paintMaterial;
    public float brushSize = 1f;
    public Color paintColor = Color.red;

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
            {
                Renderer renderer = hit.collider.GetComponent<Renderer>();
                if (renderer != null)
                {
                    Texture2D paintTexture = renderer.material.GetTexture("_PaintTexture") as Texture2D;
                    if (paintTexture != null)
                    {
                        Vector2 pixelUV = hit.textureCoord;
                        pixelUV.x *= paintTexture.width;
                        pixelUV.y *= paintTexture.height;

                        int brushSizePixels = Mathf.RoundToInt(brushSize);
                        int halfBrushSizePixels = brushSizePixels / 2;

                        for (int x = -halfBrushSizePixels; x < halfBrushSizePixels; x++)
                        {
                            for (int y = -halfBrushSizePixels; y < halfBrushSizePixels; y++)
                            {
                                int posX = Mathf.Clamp((int)pixelUV.x + x, 0, paintTexture.width - 1);
                                int posY = Mathf.Clamp((int)pixelUV.y + y, 0, paintTexture.height - 1);
                                paintTexture.SetPixel(posX, posY, paintColor);
                            }
                        }

                        paintTexture.Apply();
                    }
                }
            }
        }
    }
}
