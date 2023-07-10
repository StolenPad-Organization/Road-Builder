using UnityEngine;
using UnityEngine.Profiling;

public class Clean : MonoBehaviour
{
    [SerializeField] private Camera _camera;

    [SerializeField] private Texture2D _dirtMaskBase;
    public Texture2D brush;

    [SerializeField] private Material _material;

    [SerializeField] private Texture2D _templateDirtMask;


    [SerializeField] private Color blockColor;
    public float percentageTXT;

    [SerializeField] private Transform Player;

    private Texture2D textureToCheck;
    private int totalPixels;

    int layerToIgnore;
    int layerMask;


    public Color[] brushPixels;
    public PixelData pixelData;
    Color brushPixel;
    Color texturePixel;
    Color modifiedPixel;
    private void Start()
    {
        layerToIgnore = LayerMask.NameToLayer("Parte");
        layerMask = ~(1 << layerToIgnore);
        CreateTexture();
        //CreateBrushTexture();
        textureToCheck = (Texture2D)_material.GetTexture("_DirtMask");
        totalPixels = textureToCheck.width * textureToCheck.height;
        brush.Reinitialize(120, 120);
        brushPixels = brush.GetPixels();
    }



    private void Update()
    {
        Ray mouseRay = new Ray(Player.position + Vector3.up * (Player.localScale.y * 0.5f), Vector3.down);
        if (Physics.Raycast(mouseRay, out RaycastHit hit, float.MaxValue, layerMask))
        {
            Debug.DrawRay(mouseRay.origin, mouseRay.direction * hit.distance * 10, Color.red);

            Vector2 textureCoord = hit.textureCoord;
            int pixelX = (int)(textureCoord.x * pixelData.width);
            int pixelY = (int)(textureCoord.y * pixelData.height);


            Profiler.BeginSample("Pixels Test");


            for (int y = 0; y < brush.height; y++)
            {
                for (int x = 0; x < brush.width; x++)
                {

                    int brushIndex = y * brush.width + x;
                    int textureX = pixelX + x;
                    int textureY = pixelY + y;


                    int textureIndex = textureY * pixelData.width + textureX;


                    brushPixel = brushPixels[brushIndex];
                    texturePixel = pixelData.pixels[textureIndex];
                    modifiedPixel = new Color(texturePixel.r, brushPixels[brushIndex].g * texturePixel.g, texturePixel.b, brushPixel.a);
                    pixelData.pixels[textureIndex] = modifiedPixel;




                }
            }


            pixelData.SetPixels();
            Profiler.EndSample();
        }
    }

    private void CreateTexture()
    {
        _templateDirtMask = new Texture2D(_dirtMaskBase.width, _dirtMaskBase.height);
        _templateDirtMask.SetPixels(_dirtMaskBase.GetPixels());
        _templateDirtMask.Apply();
        _material.SetTexture("_DirtMask", _templateDirtMask);

        pixelData = TextureReader.instance.GetTexturePixels(_templateDirtMask);
        pixelData.pixels = pixelData.GetPixels();

    }


}
