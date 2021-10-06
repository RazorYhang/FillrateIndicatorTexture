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

    //public bool create = false;
    public bool createFillrateIndicator = false;

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
        //if(create)
        //{
        //    CreateResolutionTexture();
        //    create = false;
        //}
        if (createFillrateIndicator)
        {
            Create();
            createFillrateIndicator = false;
        }
    }

    void Create()
    {
#if UNITY_EDITOR

        int levelCount = maxMipLevel - minMipLevel + 1;
        int resolution = 2 << (maxMipLevel-1);
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

            Color[] texCol = sourcePatternTexture.GetPixels(0, 0, sourcePatternTextureWidth, sourcePatternTextureHeight);

            for(int p = 0; p < texCol.Length; ++p)
            {
                var col = texCol[p];
                col *= fillColor;
                texCol[p] = col;
            }

            int copyStepX = width / sourcePatternTextureWidth;
            int copyStepY = height / sourcePatternTextureHeight;

            for (int x = 0; x < copyStepX; ++x)
            {
                for (int y = 0; y < copyStepY; ++y)
                {
                    texture.SetPixels(x * sourcePatternTextureWidth, y * sourcePatternTextureHeight, sourcePatternTextureWidth, sourcePatternTextureHeight, texCol, i);
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
