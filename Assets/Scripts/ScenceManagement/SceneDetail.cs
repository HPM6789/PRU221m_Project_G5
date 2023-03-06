using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneDetail : MonoBehaviour
{
    [SerializeField] List<SceneDetail> connectedScenes;
    [SerializeField] AudioClip senceMusic;

    public AudioClip SenceMusic => senceMusic;
    public bool IsLoaded { get; private set; }

    List<SavableEntity> savableEntities;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        var test = collision.gameObject.name;
        var text = this.gameObject;
        if (collision.tag == "Player")
        {
            LoadScene();
            GameController.Instance.SetCurrentScence(this);
            if(senceMusic != null)
                AudioManager.Instance.PlayerMusic(senceMusic, true, true);

            foreach (var scene in connectedScenes)
            {
                scene.LoadScene();
            }

            var prevScene = GameController.Instance.PreviousScence;
            if(prevScene != null)
            {
                var previousLoadedScenes = prevScene.connectedScenes;
                foreach (var scene in previousLoadedScenes)
                {
                    if(!connectedScenes.Contains(scene) && scene != this)
                        scene.UnLoadScene();
                }

                if (!connectedScenes.Contains(prevScene))
                    prevScene.UnLoadScene();

            }

            
        }

    }

    public void LoadScene() {
        if (!IsLoaded)
        {
            var operation = SceneManager.LoadSceneAsync(gameObject.name, LoadSceneMode.Additive);
            IsLoaded = true;

            operation.completed += (AsyncOperation op) =>
            {
                savableEntities = GetSavableEntitesInScene();
                SavingSystem.i.RestoreEntityStates(savableEntities);
            };

            
        }
    }

    public void UnLoadScene()
    {
        if (IsLoaded)
        {

            SavingSystem.i.CaptureEntityStates(savableEntities);

            SceneManager.UnloadSceneAsync(gameObject.name);
            IsLoaded = false;
        }
    }

    public List<SavableEntity> GetSavableEntitesInScene()
    {
        var currentScene = SceneManager.GetSceneByName(gameObject.name);
        var savableEntities = FindObjectsOfType<SavableEntity>().Where(x => x.gameObject.scene == currentScene).ToList();

        return savableEntities;
    }
}
