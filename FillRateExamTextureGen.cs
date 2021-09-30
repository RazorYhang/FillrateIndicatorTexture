using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class FillRateExamTextureGen : MonoBehaviour
{
    public Color mip0Color = Color.blue;
    public Color mip1Color = Color.green;
    public Color mip2Color = Color.red;
    public int genSize = 1024;
    public Texture2D result;

    public bool create = false;
    public bool createFillrate = false;

    public int maxMipLevel = 11;  //2048
    public int minMipLevel = 6;   //64
    public Texture2D[] fillrateSourceTextures;
    public Color[] fillrateSourceColors;
    // Start is called before the first frame update
    void Start()
    { 
        
    }

    // Update is called once per frame
    void Update()
    {
        if(create)
        {
            CreateResolutionTexture();
            create = false;
        }
        if (createFillrate)
        {
            Create();
            createFillrate = false;
        }
    }

    void Create()
    {
#if UNITY_EDITOR

        int levelCount = maxMipLevel - minMipLevel + 1;
        int resolution = 1 << maxMipLevel;
        Texture2D texture = new Texture2D(resolution, resolution);

        for(int i = 0; i < maxMipLevel; ++i)
        {
            int width = resolution >> i;
            int height = resolution >> i;

            int pixelCount = width * height;

            int idx = levelCount - i - 1;

            idx = Mathf.Max(idx, 0);

            Texture2D sourcePatternTexture = fillrateSourceTextures[idx];
            int sourcePatternTextureWidth = sourcePatternTexture.width;
            int sourcePatternTextureHeight = sourcePatternTexture.height;
            Color fillColor = fillrateSourceColors[idx];
            int srcWidth = sourcePatternTexture.width;
            int srcHeight = sourcePatternTexture.height;
            Color[] srcTexture = sourcePatternTexture.GetPixels();
            Color[] cols = new Color[srcWidth * srcHeight];
            for (int s = 0; s < srcTexture.Length; ++s)
            {
                cols[s] = srcTexture[ s] * fillColor;
            }

            int stepX = width / srcWidth;
            int stepY = height / srcHeight;

            for(int sx = 0; sx < stepX;++sx )
            {
                for (int sy = 0; sy < stepY; ++sy)
                {
                    texture.SetPixels(sx* srcWidth, sy * srcHeight, srcWidth, srcHeight, cols, i);
                }
            }
        }

        texture.Apply(false);

        result = texture;
        AssetDatabase.CreateAsset(texture, "Assets/fillrate.asset");
#endif
    }

    void CreateResolutionTexture()
    {
#if UNITY_EDITOR
        int sizeWidth = genSize;
        int sizeHeight = genSize;

        Texture2D texture = new Texture2D(genSize, genSize);

        int mipLevel = texture.mipmapCount;

        for(int i = 0; i < mipLevel; ++i)
        {
            Color fillColor = (i == 0) ? mip0Color : ((i == 1) ? mip1Color : mip2Color);

            int width = sizeWidth >> i;
            int height = sizeHeight >> i;
            int pixelCount = width * height;
            Color[] cols = new Color[pixelCount];
            for (int p = 0; p < pixelCount; ++p)
                cols[p] = fillColor;

            texture.SetPixels(cols,i);
        }

        texture.Apply(false);

        result = texture;
        AssetDatabase.CreateAsset(texture, "Assets/fillrate_texture.asset");
#endif
    }
}
