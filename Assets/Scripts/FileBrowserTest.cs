using UnityEngine;
using System.Collections;
using System.IO;
using SimpleFileBrowser;
using UnityEditor;
using System;

public class FileBrowserTest : MonoBehaviour
{
    [SerializeField]
    private AudioAnalyzerManager audioAnalyzerManager;
    string[] extensions = { "*.mp3", "*.wav" };
    public void SelectFile()
    {
        // Set filters (optional)
        // It is sufficient to set the filters just once (instead of each time before showing the file browser dialog), 
        // if all the dialogs will be using the same filters
        FileBrowser.SetFilters(true, new FileBrowser.Filter("SoundFiles", ".wav", ".mp3"));

        // Set default filter that is selected when the dialog is shown (optional)
        // Returns true if the default filter is set successfully
        // In this case, set Images filter as the default filter
        FileBrowser.SetDefaultFilter(".wav");

        // Set excluded file extensions (optional) (by default, .lnk and .tmp extensions are excluded)
        // Note that when you use this function, .lnk and .tmp extensions will no longer be
        // excluded unless you explicitly add them as parameters to the function
        FileBrowser.SetExcludedExtensions(".lnk", ".tmp", ".zip", ".rar", ".exe");

        // Add a new quick link to the browser (optional) (returns true if quick link is added successfully)
        // It is sufficient to add a quick link just once
        // Name: Users
        // Path: C:\Users
        // Icon: default (folder icon)
        FileBrowser.AddQuickLink("Users", "C:\\Users", null);

        // Show a save file dialog 
        // onSuccess event: not registered (which means this dialog is pretty useless)
        // onCancel event: not registered
        // Save file/folder: file, Allow multiple selection: false
        // Initial path: "C:\", Initial filename: "Screenshot.png"
        // Title: "Save As", Submit button text: "Save"
        // FileBrowser.ShowSaveDialog( null, null, FileBrowser.PickMode.Files, false, "C:\\", "Screenshot.png", "Save As", "Save" );

        // Show a select folder dialog 
        // onSuccess event: print the selected folder's path
        // onCancel event: print "Canceled"
        // Load file/folder: folder, Allow multiple selection: false
        // Initial path: default (Documents), Initial filename: empty
        // Title: "Select Folder", Submit button text: "Select"
        // FileBrowser.ShowLoadDialog( ( paths ) => { Debug.Log( "Selected: " + paths[0] ); },
        //						   () => { Debug.Log( "Canceled" ); },
        //						   FileBrowser.PickMode.Folders, false, null, null, "Select Folder", "Select" );

        // Coroutine example
        StartCoroutine(ShowLoadDialogCoroutine());
    }

    IEnumerator ShowLoadDialogCoroutine()
    {
        // Show a load file dialog and wait for a response from user
        // Load file/folder: both, Allow multiple selection: true
        // Initial path: default (Documents), Initial filename: empty
        // Title: "Load File", Submit button text: "Load"
        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.FilesAndFolders, true, null, null, "Load Files and Folders", "Load");

        // Dialog is closed
        // Print whether the user has selected some files/folders or cancelled the operation (FileBrowser.Success)
        Debug.Log(FileBrowser.Success);
        
        if (FileBrowser.Success)
        {
            string destinationPath = "Beatmaps/Generated/";
            string sourcePath = FileBrowser.Result[0];
            string fileName ="song";
            string extension = Path.GetExtension(sourcePath);
            fileName = fileName + extension;
            string targetPath = Path.Combine(Application.dataPath, destinationPath, fileName);
            Debug.Log(targetPath);

            foreach (string ext in extensions)
            {
                foreach (string file in Directory.GetFiles(Path.Combine(Application.dataPath, destinationPath), ext, SearchOption.AllDirectories))
                {
                    File.Delete(file);
                    Console.WriteLine($"Usuni�to: {file}");
                }
            }

            File.Copy(sourcePath, targetPath, true);

            audioAnalyzerManager.Main(FileBrowser.Result[0], Path.Combine(Application.dataPath, destinationPath));
        }
        if (!FileBrowser.Success) 
        { 
            audioAnalyzerManager.FinishedGenerating();
        }
    }
}