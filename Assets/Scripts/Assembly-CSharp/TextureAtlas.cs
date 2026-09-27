using System.Collections;
using System.Collections.Generic;
using MiniJSON;
using UnityEngine;

public class TextureAtlas
{
    private struct AtlasFrame
    {
        public string name;

        public bool rotated;

        public bool trimmed;

        public Rect rect;
    }

    private Hashtable m_frames = new Hashtable();

    // CHANGED: size of the original atlas image, read from the JSON "meta" block.
    public int m_sheetWidth;

    public int m_sheetHeight;

    public TextureAtlas(TextAsset _JSON)
    {
        string text = _JSON.text;
        Dictionary<string, object> dictionary = Json.Deserialize(text) as Dictionary<string, object>;
        List<object> list = dictionary["frames"] as List<object>;
        for (int i = 0; i < list.Count; i++)
        {
            AtlasFrame atlasFrame = default(AtlasFrame);
            Dictionary<string, object> dictionary2 = list[i] as Dictionary<string, object>;
            string text2 = dictionary2["filename"] as string;
            text2 = (atlasFrame.name = text2.Substring(0, text2.Length - 4));
            Dictionary<string, object> dictionary3 = dictionary2["frame"] as Dictionary<string, object>;
            atlasFrame.rect = new Rect((long)dictionary3["x"], (long)dictionary3["y"], (long)dictionary3["w"], (long)dictionary3["h"]);
            atlasFrame.rotated = (bool)dictionary2["rotated"];
            atlasFrame.trimmed = (bool)dictionary2["trimmed"];
            m_frames.Add(text2, atlasFrame);
        }
        // CHANGED: remember the original atlas size so UVs don't depend on how Unity imported the texture.
        if (dictionary.ContainsKey("meta"))
        {
            Dictionary<string, object> meta = dictionary["meta"] as Dictionary<string, object>;
            if (meta != null && meta.ContainsKey("size"))
            {
                Dictionary<string, object> size = meta["size"] as Dictionary<string, object>;
                if (size != null && size.ContainsKey("w") && size.ContainsKey("h"))
                {
                    m_sheetWidth = (int)(long)size["w"];
                    m_sheetHeight = (int)(long)size["h"];
                }
            }
        }
    }

    public Frame GetFrame(string frameName)
    {
        if (m_frames.ContainsKey(frameName))
        {
            AtlasFrame atlasFrame = (AtlasFrame)m_frames[frameName];
            return new Frame(atlasFrame.rect.x, atlasFrame.rect.y, atlasFrame.rect.width, atlasFrame.rect.height);
        }
        Debug.LogError("Requested frame (" + frameName + ") missing!");
        return null;
    }
}