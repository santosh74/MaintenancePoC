using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DefaultGltfLoader", menuName = "VERTX/GltfLoader", order = 1)]
public class DefaultGltfLoader : GltfLoader {

    public UnityGLTF.GLTFSceneImporter.ColliderType ColliderType = UnityGLTF.GLTFSceneImporter.ColliderType.Box;

    public override void Load(GameObject target, string url)
    {
        var gltfLoader = target.AddComponent<UnityGLTF.GLTFComponent>();
        gltfLoader.GLTFUri = url;
        gltfLoader.Collider = ColliderType;
    }
}
