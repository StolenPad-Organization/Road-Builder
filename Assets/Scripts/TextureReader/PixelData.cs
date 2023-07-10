using UnityEngine;
using System;
public struct PixelData
{
    public Color32[] pixels;
    public int width;
    public int height;
    public Texture2D texture;

    public PixelData( int width, int height, Texture2D texture)
    {
        this.width = width;
        this.height = height;      
        this.texture = texture;
        pixels = null;
    }


    public Color32[] GetPixels(Texture2D texture)
    {
        if (width == texture.width && height == texture.height)
        {
            Color32[] result = new Color32[width * height];
            byte[] rawTextureData = texture.GetRawTextureData();

            for (int i = 0; i < result.Length; i++)
            {
                int offset = i * 4;
                result[i] = new Color32(rawTextureData[offset], rawTextureData[offset + 1], rawTextureData[offset + 2], rawTextureData[offset + 3]);
            }

            return result;
        }
        else
        {
            return null;
        }
    }
    public Color32[] GetPixels()
    {
        if (width == texture.width && height == texture.height)
        {
            Color32[] result = new Color32[width * height];
            byte[] rawTextureData = texture.GetRawTextureData();

            for (int i = 0; i < result.Length; i++)
            {
                int offset = i * 4;
                result[i] = new Color32(rawTextureData[offset], rawTextureData[offset + 1], rawTextureData[offset + 2], rawTextureData[offset + 3]);
            }

            return result;
        }
        else
        {
            return null;
        }
    }
    public Color32[] GetPixels(int x, int y, int blockWidth, int blockHeight, int miplevel = 0)
    {
        int mipWidth = width >> miplevel;
        int mipHeight = height >> miplevel;
        int startX = x >> miplevel;
        int startY = y >> miplevel;

        Color32[] result = new Color32[blockWidth * blockHeight];

        for (int row = 0; row < blockHeight; row++)
        {
            int textureY = startY + row;

            for (int col = 0; col < blockWidth; col++)
            {
                int textureX = startX + col;

                try
                {
                    int pixelIndex = (textureY * mipWidth) + textureX;
                    Color brushPixel = pixels[pixelIndex];

                    // Check if the pixel in the brush is black
                    if (brushPixel.r == 0f && brushPixel.g == 0f && brushPixel.b == 0f)
                    {
                        result[row * blockWidth + col] = brushPixel;
                    }
                    else
                    {
                        // Set the result pixel to transparent if the brush pixel is not black
                        result[row * blockWidth + col] = Color.clear;
                    }
                }
                catch (SystemException)
                {
                    // Set the result pixel to transparent if the index is out of range
                    result[row * blockWidth + col] = Color.clear;
                }
            }
        }

        return result;
    }

    public void SetPixels()
    {
        if (pixels != null && pixels.Length == width * height)
        {
            texture.SetPixels32(pixels);
            texture.Apply();
        }
    }


}



