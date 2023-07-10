using UnityEngine;

public class TextureReader : MonoBehaviour
{
    public static TextureReader instance;

    private void Awake()
    {
        instance = this;
    }

    public PixelData GetTexturePixels(Texture2D texture)
    {
        int width = texture.width;
        int height = texture.height;

        PixelData pixelData = new PixelData( width, height,texture);

        return pixelData;
    }
}