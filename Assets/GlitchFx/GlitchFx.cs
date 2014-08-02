//
// Glitch Fx
//
// Copyright (C) 2014 Keijiro Takahashi
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class GlitchFx : MonoBehaviour
{
    [Range(0, 1)]
    public float intensity = 1.0f;

    // Temporary objects.
    Material material;
    Texture2D texture;
    RenderTexture buffer1;
    RenderTexture buffer2;
    int frameCount;

    static Color RandomColor()
    {
        return new Color(Random.value, Random.value, Random.value, Random.value);
    }

    void SetUpObjects()
    {
        if (material != null && texture != null) return;

        material = new Material(Shader.Find("Hidden/GlitchFx"));
        material.hideFlags = HideFlags.DontSave;

        texture = new Texture2D(64, 32, TextureFormat.ARGB32, false);
        texture.hideFlags = HideFlags.DontSave;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.filterMode = FilterMode.Point;

        buffer1 = new RenderTexture(Screen.width, Screen.height, 0);
        buffer2 = new RenderTexture(Screen.width, Screen.height, 0);

        UpdateParameters();
    }

    void UpdateParameters()
    {
        var color = RandomColor();

        for (var y = 0; y < texture.height; y++)
        {
            for (var x = 0; x < texture.width; x++)
            {
                if (Random.value > 0.85f) color = RandomColor();
                texture.SetPixel(x, y, color);
            }
        }

        texture.Apply();
    }

    void Start()
    {
        SetUpObjects();
    }

    void Update()
    {
        if (Random.value > 0.85f) UpdateParameters();
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        SetUpObjects();

        if ((frameCount % 13) == 0) Graphics.Blit(source, buffer1);
        if ((frameCount % 23) == 0) Graphics.Blit(source, buffer2);

        material.SetFloat("_Intensity", intensity);
        material.SetTexture("_GlitchTex", texture);
        material.SetTexture("_BufferTex", (frameCount % 3) == 0 ? buffer1 : buffer2);

        Graphics.Blit(source, destination, material);

        frameCount++;
    }
}
