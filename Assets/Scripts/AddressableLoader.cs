using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class AddressableLoader : MonoBehaviour
{
    public string Key;
    public string CatalogPath;

    private void OnInitCompleted(AsyncOperationHandle<IResourceLocator> obj)
    {
        StartCoroutine(DownloadObject());
    }

    public IEnumerator DownloadObject()
    {
        Debug.Log("Start downloading");
        var downloadHandle = Addressables.DownloadDependenciesAsync(Key);

        DownloadStatus status;
        while (!downloadHandle.IsDone)
        {
            status = downloadHandle.GetDownloadStatus();
            float progress = status.Percent;
            Debug.Log("progress:" + progress);
            yield return null;
        }
        status = downloadHandle.GetDownloadStatus();
        float percent = status.Percent;
        var downloadHandleStatus = downloadHandle.Status;
        Addressables.Release(downloadHandle);
        Addressables.InstantiateAsync(Key, -new Vector3(0, 0, 0), Quaternion.identity);
    }

    public IEnumerator StartInitialize()
    {
        {
            AsyncOperationHandle<IResourceLocator> handle = Addressables.LoadContentCatalogAsync(CatalogPath, true);
            yield return handle;
        }

        {
            var handle = Addressables.InitializeAsync();
            handle.Completed += OnInitCompleted;
            yield return handle;
        }
    }

    [ContextMenu("Load Addressable")]
    void LoadAddressable()
    {
        StartCoroutine(StartInitialize());
    }

    // Update is called once per frame
    void Start()
    {
        LoadAddressable();
    }
}
