using System;
using System.Collections;
using System.IO;
using UnityEngine;

public static class ResourceManager
{
    private static Hashtable m_resourceGroups = new Hashtable();

    private static Hashtable m_resources = new Hashtable();

    public static void ListResources(string _groupIdentifier)
    {
        IResourceGroup resourceGroup = m_resourceGroups[_groupIdentifier] as IResourceGroup;
        string text = string.Empty;
        foreach (IResource resource in resourceGroup.resources)
        {
            text = text + resource.identifier + ",";
        }
    }

    public static int GetGroupCount()
    {
        return m_resourceGroups.Count;
    }

    public static int GetResourceCount()
    {
        return m_resources.Count;
    }

    public static void GenerateResourceGroupFromFolder(string _resourcesPath)
    {
        // CHANGED: split on both '/' and '\' so the group is always named after the last folder.
        string[] array = _resourcesPath.Split('/', '\\');
        string text = array[array.Length - 1];
        if (m_resourceGroups.ContainsKey(text))
        {
            Debug.LogError("Resource group with the name " + text + " already exists!");
            return;
        }
        AddResourceGroup(text);
        UnityEngine.Object[] array2 = Resources.LoadAll(_resourcesPath, typeof(UnityEngine.Object));
        for (int i = 0; i < array2.Length; i++)
        {
            if (array2[i] != null)
            {
                UnityResource resource = new UnityResource(array2[i], text);
                AddResourceToGroup(text, resource);
            }
        }
    }

    public static void LoadAndAddResourceToGroup(string _resourcePath, string _group)
    {
        if (!m_resourceGroups.ContainsKey(_group))
        {
            AddResourceGroup(_group);
        }
        UnityEngine.Object obj = Resources.Load(_resourcePath);
        // CHANGED: only if that load handed back a Mesh, check whether a prefab shares the same name
        // (Foo.prefab + Foo mesh) and take the prefab. Every other kind of asset (fonts, materials, ...)
        // is loaded exactly as before.
        if (obj is Mesh)
        {
            UnityEngine.Object prefab = Resources.Load(_resourcePath, typeof(GameObject));
            if (prefab != null)
            {
                obj = prefab;
            }
        }
        UnityResource resource = new UnityResource(obj, _group);
        AddResourceToGroup(_group, resource);
    }

    public static IResourceGroup AddResourceGroup(string _identifier)
    {
        BasicResourceGroup basicResourceGroup = new BasicResourceGroup(_identifier);
        m_resourceGroups.Add(_identifier, basicResourceGroup);
        return basicResourceGroup;
    }

    public static IResource AddResourceToGroup(IResourceGroup _group, IResource _resource)
    {
        // CHANGED: handle several objects sharing one identifier instead of throwing on Hashtable.Add.
        if (_resource == null || string.IsNullOrEmpty(_resource.identifier))
        {
            return null;
        }
        IResource existing = m_resources[_resource.identifier] as IResource;
        if (existing != null)
        {
            UnityEngine.Object existingObj = existing.resourceObject as UnityEngine.Object;
            UnityEngine.Object newObj = _resource.resourceObject as UnityEngine.Object;

            // The very same object seen twice: nothing to add.
            if (existingObj != null && newObj != null && existingObj.GetInstanceID() == newObj.GetInstanceID())
            {
                return existing;
            }

            // The same object seen again after it lost the identifier to a prefab: also nothing to add.
            if (newObj != null)
            {
                int newId = newObj.GetInstanceID();
                for (int i = 0; i < _group.resources.Count; i++)
                {
                    UnityEngine.Object other = _group.resources[i].resourceObject as UnityEngine.Object;
                    if (other != null && other.GetInstanceID() == newId)
                    {
                        return _group.resources[i];
                    }
                }
            }

            // Two different objects share one identifier. Both stay loaded and tracked in the group
            // (kept alive, unloaded with the group), but the identifier can only point at one:
            // the first one keeps it, except that a prefab takes it over from a Mesh with the same name
            // (Terrain.prefab + Terrain mesh). Identifiers are not renamed.
            if (newObj is GameObject && existingObj is Mesh)
            {
                m_resources[_resource.identifier] = _resource;
            }
            _group.resources.Add(_resource);
            return _resource;
        }
        m_resources.Add(_resource.identifier, _resource);
        _group.resources.Add(_resource);
        return _resource;
    }

    public static IResource AddResourceToGroup(string _groupIdentifier, IResource _resource)
    {
        if (m_resourceGroups.Contains(_groupIdentifier))
        {
            return AddResourceToGroup(m_resourceGroups[_groupIdentifier] as IResourceGroup, _resource);
        }
        return null;
    }

    public static void UnloadResourceGroup(IResourceGroup _group)
    {
        for (int i = 0; i < _group.resources.Count; i++)
        {
            UnloadResource(_group.resources[i], false);
        }
        m_resourceGroups.Remove(_group.identifier);
        Resources.UnloadUnusedAssets();
        GC.Collect();
    }

    public static void UnloadResourceGroup(string _identifier)
    {
        if (m_resourceGroups.Contains(_identifier))
        {
            UnloadResourceGroup((IResourceGroup)m_resourceGroups[_identifier]);
        }
    }

    public static void UnloadResource(IResource _resource)
    {
        UnloadResource(_resource, true);
    }

    public static void UnloadResource(IResource _resource, bool _unloadAssets)
    {
        // CHANGED: don't try to unload something that was already unloaded (would throw).
        if (_resource.resourceObject != null)
        {
            _resource.Unload();
        }
        if (_resource.resourceGroup != null)
        {
            _resource.resourceGroup.resources.Remove(_resource);
        }
        // CHANGED: only drop the identifier if it actually points at this resource
        // (a same-named object that lost the identifier must not remove the winner's entry).
        if (object.ReferenceEquals(m_resources[_resource.identifier], _resource))
        {
            m_resources.Remove(_resource.identifier);
        }
        if (_unloadAssets)
        {
            Resources.UnloadUnusedAssets();
            GC.Collect();
        }
    }

    public static void UnloadResource(string _identifier)
    {
        if (m_resources.Contains(_identifier))
        {
            UnloadResource((IResource)m_resources[_identifier]);
        }
        else
        {
            Debug.LogError("Trying to unload unknown resource!");
        }
    }

    public static IResource GetResourceClass(string _resourceIdentifier)
    {
        // CHANGED: one lookup instead of two (the table hands back null when the name isn't there).
        return m_resources[_resourceIdentifier] as IResource;
    }

    public static AudioClip GetAudioClip(string _resourceIdentifier)
    {
        IResource resourceClass = GetResourceClass(_resourceIdentifier);
        if (resourceClass != null && resourceClass.resourceObject != null)
        {
            return resourceClass.resourceObject as AudioClip;
        }
        return null;
    }

    public static Texture GetTexture(string _resourceIdentifier)
    {
        IResource resourceClass = GetResourceClass(_resourceIdentifier);
        if (resourceClass != null && resourceClass.resourceObject != null)
        {
            return resourceClass.resourceObject as Texture;
        }
        return null;
    }

    public static Shader GetShader(string _resourceIdentifier)
    {
        IResource resourceClass = GetResourceClass(_resourceIdentifier);
        if (resourceClass != null && resourceClass.resourceObject != null)
        {
            return resourceClass.resourceObject as Shader;
        }
        return null;
    }

    public static Material GetMaterial(string _resourceIdentifier)
    {
        IResource resourceClass = GetResourceClass(_resourceIdentifier);
        if (resourceClass != null && resourceClass.resourceObject != null)
        {
            return resourceClass.resourceObject as Material;
        }
        return null;
    }

    public static GameObject GetGameObject(string _resourceIdentifier)
    {
        IResource resourceClass = GetResourceClass(_resourceIdentifier);
        if (resourceClass != null && resourceClass.resourceObject != null)
        {
            return resourceClass.resourceObject as GameObject;
        }
        return null;
    }

    public static Level GetLevel(string _resourceIdentifier)
    {
        IResource resourceClass = GetResourceClass(_resourceIdentifier);
        if (resourceClass != null && resourceClass.resourceObject != null)
        {
            return resourceClass.resourceObject as Level;
        }
        return null;
    }

    public static TextAsset GetTextAsset(string _resourceIdentifier)
    {
        IResource resourceClass = GetResourceClass(_resourceIdentifier);
        if (resourceClass != null && resourceClass.resourceObject != null)
        {
            return resourceClass.resourceObject as TextAsset;
        }
        return null;
    }

    public static UnityEngine.Font GetFont(string _resourceIdentifier)
    {
        IResource resourceClass = GetResourceClass(_resourceIdentifier);
        if (resourceClass != null && resourceClass.resourceObject != null)
        {
            return resourceClass.resourceObject as UnityEngine.Font;
        }
        return null;
    }

    public static AnimationClip GetAnimationClip(string _resourceIdentifier)
    {
        IResource resourceClass = GetResourceClass(_resourceIdentifier);
        if (resourceClass != null && resourceClass.resourceObject != null)
        {
            return resourceClass.resourceObject as AnimationClip;
        }
        return null;
    }
}