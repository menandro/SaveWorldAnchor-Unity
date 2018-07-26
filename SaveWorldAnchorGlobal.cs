using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.XR.WSA;
using UnityEngine.XR.WSA.Sharing;
using System;
using System.Runtime.InteropServices.WindowsRuntime;
#if !UNITY_EDITOR
using Windows.Storage;
#endif

public class SaveWorldAnchorGlobal
{
    // Exporting
    private GameObject gameObjectToGet;
    private byte[] exportedData;
    private string exportAnchorName;
    private WorldAnchor exportedAnchor;
    private string exportFilename;
    private MemoryStream ms;

    // Importing
    public WorldAnchor importedAnchor;
    private byte[] importedData = null;
    private int retryCount = 3;
    private string importAnchorName;
    private GameObject gameObjectToSet;
    private string importFilename;
    public bool isImportFileLoaded = false;
    private bool isLoadingFile = false;

    // General Request
    public bool isRequestActive = false;
    public bool isRequestFinished = false;

#if UNITY_EDITOR
    private void Save(string filename, string anchorName, WorldAnchor anchor)
    {
        Debug.Log("Saving world anchor using transfer batch won't work in Editor.");
    }
    public void Save(string filename, string anchorName, GameObject gameObject)
    {
        Debug.Log("Saving world anchor using transfer batch won't work in Editor.");
    }
#endif
#if !UNITY_EDITOR
    private void Save(string filename, string anchorName, WorldAnchor anchor)
    {
        exportFilename = filename;
        exportAnchorName = anchorName;
        exportedAnchor = anchor;
        ms = new MemoryStream();

        WorldAnchorTransferBatch transferBatch = new WorldAnchorTransferBatch();
        transferBatch.AddWorldAnchor(exportAnchorName, exportedAnchor);
        WorldAnchorTransferBatch.ExportAsync(transferBatch, OnExportDataAvailable, OnExportComplete);
    }

    public void Save(string filename, string anchorName, GameObject gameObject)
    {
        gameObjectToGet = gameObject;
        WorldAnchor anchor = gameObjectToGet.GetComponent<WorldAnchor>();
        if (anchor == null)
        {
            anchor = gameObjectToGet.AddComponent<WorldAnchor>();
        }
        Save(filename, anchorName, anchor);
    }

    private void OnExportDataAvailable(byte[] data)
    {
        //Save data to file
        ms.Write(data, 0, data.Length);
    }

    private void OnExportComplete(SerializationCompletionReason completionReason)
    {
        if (completionReason != SerializationCompletionReason.Succeeded)
        {
            //Failed
        }
        else
        {
            //Success
            exportedData = ms.ToArray();
            Task.Factory.StartNew(() => SaveToFile());
        }
    }

    private async void SaveToFile()
    {
        StorageFolder storageFolder = KnownFolders.CameraRoll;
        StorageFile storageFile = await storageFolder.CreateFileAsync(exportFilename, CreationCollisionOption.ReplaceExisting);
        await FileIO.WriteBytesAsync(storageFile, exportedData);
    }


    public async void SetWorldAnchor(string filename, string anchorName, GameObject gameObject)
    {
        isRequestActive = true;
        isRequestFinished = false;
        // Import Data (open from file)
        importFilename = filename;
        importAnchorName = anchorName;
        gameObjectToSet = gameObject;

        // Load the data if it not yet loaded
        if (importedData == null)
        {
            StorageFolder storageFolder = KnownFolders.CameraRoll;
            StorageFile storageFile = await storageFolder.GetFileAsync(importFilename);
            var buffer = await FileIO.ReadBufferAsync(storageFile);
            importedData = buffer.ToArray();
        }
        WorldAnchorTransferBatch.ImportAsync(importedData, OnImportComplete);
    }
    
    private void OnImportComplete(SerializationCompletionReason completionReason, WorldAnchorTransferBatch deserializedTransferBatch)
    {
        if (completionReason != SerializationCompletionReason.Succeeded)
        {
            //Import failed
            if (retryCount > 0)
            {
                retryCount--;
                WorldAnchorTransferBatch.ImportAsync(importedData, OnImportComplete);
            }
            return;
        }
        importedAnchor = deserializedTransferBatch.LockObject(importAnchorName, gameObjectToSet);
        isRequestActive = false;
        isRequestFinished = true;
    }
#endif
#if UNITY_EDITOR
    public void SetWorldAnchor(string filename, string anchorName, GameObject gameObject)
    {
        gameObject.AddComponent<WorldAnchor>();
    }
#endif
}
