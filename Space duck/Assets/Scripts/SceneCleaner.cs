using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneCleaner : MonoBehaviour
{
    [Header("Takarítás")]
    public string[] namesToDestroy = { "Managers", "Player" };

    void Start()
    {
        CleanDontDestroyOnLoad();

        UnlockMouse();
    }

    void Update()
    {
        if (Time.timeSinceLevelLoad < 2f)
        {
            UnlockMouse();
        }
    }

    public void UnlockMouse()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    void CleanDontDestroyOnLoad()
    {
        GameObject[] ddolObjects = GetDontDestroyOnLoadObjects();
        foreach (GameObject obj in ddolObjects)
        {
            foreach (string nameToKill in namesToDestroy)
            {
                if (obj.name == nameToKill)
                {
                    Destroy(obj);
                    Debug.Log($"Cleaner: {obj.name} törölve.");
                }
            }
        }
    }

    GameObject[] GetDontDestroyOnLoadObjects()
    {
        GameObject temp = new GameObject();
        DontDestroyOnLoad(temp);
        Scene ddolScene = temp.scene;
        Destroy(temp);
        return ddolScene.GetRootGameObjects();
    }
}