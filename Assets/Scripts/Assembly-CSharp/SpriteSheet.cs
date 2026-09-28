using UnityEngine;

public class SpriteSheet
{
    public int m_index;

    public DynamicArray<SpriteC> m_components;

    public Camera m_camera;

    public GameObject m_gameObject;

    public Mesh m_mesh;

    public MeshFilter m_meshFilter;

    public MeshRenderer m_meshRenderer;

    public Material m_material;

    public int m_textureWidth;

    public int m_textureHeight;

    public float m_globalSpriteScale;

    public int m_currentMeshCapacity;

    public int[] m_freeVertexIndices;

    public int m_freeVertexIndicesCount;

    public Vector3[] m_vertices;

    public Vector3[] m_normals;

    public Vector2[] m_UVs;

    public Color[] m_colors;

    public int[] m_triIndices;

    public bool m_meshCapacityChanged;

    public bool m_vertsChanged;

    public bool m_uvsChanged;

    public bool m_colorsChanged;

    public bool m_sortMesh;

    public bool m_sortingEnabled = true;

    public TextureAtlas m_atlas;

    public static Color m_defaultColor = new Color(0.5f, 0.5f, 0.5f, 1f);

    public SpriteSheet(Camera _camera, Texture _texture, Shader _shader, float _globalSpriteScale)
    {
        CreateSpriteSheet(_camera, new Material(_shader)
        {
            mainTexture = _texture
        }, null, _globalSpriteScale);
    }

    public SpriteSheet(Camera _camera, Material _material, float _globalSpriteScale)
    {
        CreateSpriteSheet(_camera, _material, null, _globalSpriteScale);
    }

    public SpriteSheet(Camera _camera, Material _material, TextAsset _atlas, float _globalSpriteScale)
    {
        CreateSpriteSheet(_camera, _material, _atlas, _globalSpriteScale);
    }

    private void CreateSpriteSheet(Camera _camera, Material _material, TextAsset _atlas, float _globalSpriteScale)
    {
        if (_atlas != null)
        {
            m_atlas = new TextureAtlas(_atlas);
        }
        m_camera = _camera;
        m_gameObject = new GameObject("SpriteSheet");
        m_gameObject.layer = m_camera.gameObject.layer;
        m_meshFilter = m_gameObject.AddComponent<MeshFilter>() as MeshFilter;
        m_meshRenderer = m_gameObject.AddComponent<MeshRenderer>() as MeshRenderer;
        m_material = _material;
        m_textureWidth = 0;
        m_textureHeight = 0;
        if (_material.mainTexture != null)
        {
            m_textureWidth = _material.mainTexture.width;
            m_textureHeight = _material.mainTexture.height;
        }
        // CHANGED: the frame positions in the atlas JSON are pixels of the ORIGINAL atlas image. Unity may
        // have resized the texture on import (non power of two / max size), so divide by the original
        // size from the JSON instead of the imported texture's size.
        if (m_atlas != null && m_atlas.m_sheetWidth > 0 && m_atlas.m_sheetHeight > 0)
        {
            m_textureWidth = m_atlas.m_sheetWidth;
            m_textureHeight = m_atlas.m_sheetHeight;
        }
        m_mesh = m_meshFilter.mesh;
        m_mesh.MarkDynamic();
        m_meshRenderer.bounds.SetMinMax(new Vector3(float.MinValue, float.MinValue, float.MinValue), new Vector3(float.MaxValue, float.MaxValue, float.MaxValue));
        m_meshRenderer.GetComponent<Renderer>().material = m_material;
        m_globalSpriteScale = _globalSpriteScale;
        m_gameObject.transform.position = Vector3.zero;
        m_components = new DynamicArray<SpriteC>();
        m_currentMeshCapacity = 0;
        setMeshData(0);
    }

    public void setMeshData(int _capacity)
    {
        m_vertices = new Vector3[_capacity * 4];
        for (int i = 0; i < m_vertices.Length; i++)
        {
            m_vertices[i] = Vector3.zero;
        }
        m_normals = new Vector3[_capacity * 4];
        m_UVs = new Vector2[_capacity * 4];
        m_colors = new Color[_capacity * 4];
        m_triIndices = new int[_capacity * 6];
        m_freeVertexIndices = new int[_capacity];
        m_freeVertexIndicesCount = _capacity;
        for (int j = 0; j < _capacity; j++)
        {
            m_triIndices[j * 6 + 5] = j * 4;
            m_triIndices[j * 6 + 4] = j * 4 + 1;
            m_triIndices[j * 6 + 3] = j * 4 + 3;
            m_triIndices[j * 6 + 2] = j * 4 + 3;
            m_triIndices[j * 6 + 1] = j * 4 + 1;
            m_triIndices[j * 6] = j * 4 + 2;
            m_freeVertexIndices[j] = j;
        }
        m_mesh.triangles = null;
        m_mesh.vertices = null;
        m_mesh.vertices = m_vertices;
        m_mesh.triangles = m_triIndices;
        m_mesh.uv = m_UVs;
        m_mesh.colors = m_colors;
        m_mesh.normals = m_normals;
        m_mesh.bounds = new Bounds(Vector3.zero, new Vector3(99999f, 99999f, 99999f));
        m_meshCapacityChanged = true;
        m_currentMeshCapacity = _capacity;
    }

    public void resetVertexIndices()
    {
        m_freeVertexIndicesCount = m_currentMeshCapacity;
        for (int i = 0; i < m_freeVertexIndicesCount; i++)
        {
            m_freeVertexIndices[i] = i;
        }
    }

    public void assignVertexDataIndices()
    {
        for (int i = 0; i < m_components.m_aliveCount; i++)
        {
            int num = m_components.m_aliveIndices[i];
            SpriteC spriteC = m_components.m_array[num];
            spriteC.vertDataIndex = m_freeVertexIndices[m_freeVertexIndicesCount - 1];
            m_freeVertexIndicesCount--;
        }
    }
}