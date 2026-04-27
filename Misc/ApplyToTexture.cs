using System.Collections.Generic;

using UnityEngine;

namespace Heaj.Sam.MapUtilities
{
    public static partial class MapUtility<T>
    {
        private static void EnsureTextureSize(int width, int height, Texture2D texture)
        {
            if (texture == null)
                throw new System.Exception("The ApplyToTexture function cannot be used with a null texture. Please create the texture before calling the function.");

            if (texture.width != width | texture.height != height)
                texture.Reinitialize(width, height);
        }

        private static void EnsureTextureSize(T[,] map, Texture2D texture, out int width, out int height)
        {
            if (texture == null)
                throw new System.Exception("The ApplyToTexture function cannot be used with a null texture. Please create the texture before calling the function.");

            width = map.GetLength(0);
            height = map.GetLength(1);

            if (texture.width != width | texture.height != height)
                texture.Reinitialize(width, height);
        }

        private static Color ColorFromIndex(int index)
        {
            if (index < 0)
                return Color.black;

            float hue = (index * 0.13f + 0.5f) % 1.0f;
            float saturation = 0.8f - ((index / 8) * 0.1f % 0.4f);
            float value = 0.8f - ((index / 64) * 0.1f % 0.4f);

            return Color.HSVToRGB(hue, saturation, value);
        }

        private static Color ColorFromType(T type)
        {
            return ColorFromIndex((int)(object)type);
        }

        /// <summary>
        /// Applies a random color by type to the texture.<br/>
        /// The texture cannot be null.
        /// </summary>
        public static void ApplyToTexture(T[,] map, Texture2D texture)
        {
            EnsureTextureSize(map, texture, out int width, out int height);

            //Fill texture
            Color[] c = new Color[width * height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    c[x + y * width] = ColorFromType(map[x, y]);
                }
            }
            texture.SetPixels(c);
            texture.Apply();
        }

        /// <summary>
        /// Applies a random color by type to the texture.<br/>
        /// The texture cannot be null.
        /// </summary>
        public static void ApplyToTexture(FloodFillData floodFill, Texture2D texture)
        {
            int width = floodFill.ids.GetLength(0);
            int height = floodFill.ids.GetLength(1);
            EnsureTextureSize(width, height, texture);

            //Fill texture
            Color[] c = new Color[width * height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    c[x + y * width] = ColorFromIndex(floodFill[x, y]);
                }
            }
            texture.SetPixels(c);
            texture.Apply();
        }

        /// <summary>
        /// Applies a given color by type to the texture.<br/>
        /// The texture cannot be null.
        /// </summary>
        /// <param name="colors">An array containing the colors to use. The order matches the values in your enum. If the enum values exceed the array's bounds, the pixels at those positions will remain clear.</param>
        public static void ApplyToTexture(T[,] map, Texture2D texture, params Color[] colors)
        {
            EnsureTextureSize(map, texture, out int width, out int height);

            //Fill texture
            Color[] c = new Color[width * height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int i = (int)(object)map[x, y];
                    if (i < 0 || i >= colors.Length)
                        continue;
                    c[x + y * width] = colors[i];
                }
            }
            texture.SetPixels(c);
            texture.Apply();
        }

        /// <summary>
        /// Applies colors to the texture according to a specific remap.<br/>
        /// The texture cannot be null.<br/>
        /// If a color exists in the map but not in the remap, the “clear” color will be applied to those pixels.
        /// </summary>
        public static void ApplyToTexture(T[,] map, Texture2D texture, params (T, Color)[] colors)
        {
            EnsureTextureSize(map, texture, out int width, out int height);

            //Fill dictionary
            Dictionary<T, Color> dictionary = new Dictionary<T, Color>();
            for (int i = 0; i < colors.Length; i++)
            {
                if (dictionary.ContainsKey(colors[i].Item1))
                    dictionary[colors[i].Item1] = colors[i].Item2;
                else
                    dictionary.Add(colors[i].Item1, colors[i].Item2);
            }

            //Fill texture
            Color[] c = new Color[width * height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (!dictionary.ContainsKey(map[x, y]))
                        continue;
                    c[x + y * width] = dictionary[map[x, y]];
                }
            }
            texture.SetPixels(c);
            texture.Apply();
        }

        /// <summary>
        /// Applies a given color by id to the texture.<br/>
        /// The texture cannot be null.
        /// </summary>
        public static void ApplyToTexture(int[,] floodFill, Texture2D texture)
        {
            int width = floodFill.GetLength(0);
            int height = floodFill.GetLength(1);
            EnsureTextureSize(width, height, texture);

            //Fill texture
            Color[] c = new Color[width * height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    c[x + y * width] = ColorFromIndex(floodFill[x, y]);
                }
            }
            texture.SetPixels(c);
            texture.Apply();
        }

        /// <summary>
        /// Draw the gradient in black and white in the texture.<br/>
        /// The texture cannot be null.
        /// </summary>
        public static void ApplyToTexture(float[,] gradient, Texture2D texture)
        {
            int width = gradient.GetLength(0);
            int height = gradient.GetLength(1);
            EnsureTextureSize(width, height, texture);

            //Fill texture
            Color[] c = new Color[width * height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float v = gradient[x, y];
                    c[x + y * width] = new Color(v, v, v, 1.0f);
                }
            }
            texture.SetPixels(c);
            texture.Apply();
        }

        /// <summary>
        /// Draw the gradient in a specific channel of the texture; the other channels will not be modified if the provided the texture is already the correct size.<br/>
        /// The texture cannot be null.
        /// </summary>
        public static void ApplyToTexture(float[,] gradient, int channel, Texture2D texture)
        {
            int width = gradient.GetLength(0);
            int height = gradient.GetLength(1);
            EnsureTextureSize(width, height, texture);

            //Fill texture
            Color[] c = texture.GetPixels();
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color color = c[x + y * width];
                    color[channel] = gradient[x, y];
                    c[x + y * width] = color;
                }
            }
            texture.SetPixels(c);
            texture.Apply();
        }

        /// <summary>
        /// Draw an uniform value in a specific channel of the texture; the other channels will not be modified if the provided the texture is already the correct size.<br/>
        /// The texture cannot be null.
        /// </summary>
        public static void ApplyToTexture(float uniformValue, int channel, Texture2D texture)
        {
            int width = texture.width;
            int height = texture.height;
            EnsureTextureSize(width, height, texture);

            //Fill texture
            Color[] c = texture.GetPixels();
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color color = c[x + y * width];
                    color[channel] = uniformValue;
                    c[x + y * width] = color;
                }
            }
            texture.SetPixels(c);
            texture.Apply();
        }
    }
}
