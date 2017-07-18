using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

namespace DTS.IO
{
    /// <summary>
    /// Monobehaviour singleton that represents a file viewer
    /// </summary>
    public class UIFileExporerBehaviour : MonoBehaviour, IFileOpener, IFileWriter
    {
        
        /// <summary>
        /// Humble Object field
        /// </summary>
        public UIFileExplorer FileExplorer;

        /// <summary>
        /// Singleton Instance
        /// </summary>
        public static UIFileExporerBehaviour Instance;

        private void Awake()
        {
            // Implement singleton pattern
            if(Instance == null)
            {
                Instance = this;
            }
            else { Destroy(gameObject); return; }

            // implement humble object pattern
            FileExplorer.SetFileOpener(this);
            FileExplorer.SetFileWriter(this);
        }


    }

    [System.Serializable]
    public class UIFileExplorer
    {
        #region Humble Object Pattern

        private IFileOpener FileOpener;
        private IFileWriter FileWriter;

        public void SetFileOpener(IFileOpener FileOpener)
        {
            this.FileOpener = FileOpener;
        }

        public void SetFileWriter(IFileWriter FileWriter)
        {
            this.FileWriter = FileWriter;
        }

        #endregion

        #region Serialized Variables

        [Header("File Open Dialogue")]

        [SerializeField]
        private GameObject FileOpenDialogue;

        public ScrollRect ScrollView;

        [Header("File Save Dialogue")]

        [SerializeField]
        private GameObject FileSaveDialogue;

        #endregion

        private List<Button>



    }

    public interface IFileOpener
    {

    }

    public interface IFileWriter
    {

    }
}


