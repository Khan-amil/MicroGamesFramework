using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace DefaultNamespace
{
    [System.Serializable]
    public class SceneAssetReference:AssetReference
    {
        public SceneAssetReference(string guid) : base(guid)
        {

        }

        public override bool ValidateAsset(string path)
        {
            return path.EndsWith(".unity");
        }


        /// <summary>
        /// Loads the reference as a scene.
        /// </summary>
        /// <returns>The operation handle for the scene load.</returns>
        public AsyncOperationHandle<SceneInstance> PreLoadSceneAsync()
        {

            Resources.UnloadUnusedAssets();
            var result = Addressables.LoadSceneAsync(RuntimeKey,LoadSceneMode.Additive,activateOnLoad:true);
            return result;
        }
    }
}