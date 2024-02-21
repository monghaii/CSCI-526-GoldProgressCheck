using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn.Unity;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("DatingSim")] 
    public Canvas datingSimMode;
    
    [Header("FPS")] 
    public GameObject fpsMode;

    [Header("CameraManagement")] 
    public Camera datingSimCamera;
    public Camera fpsCamera;
    
    [Header("Characters")] 
    public Image characterImage;
    public Characters characterSO;
    
    
    // Start is called before the first frame update
    void Start()
    {
        // dating sim enabled at the beginning
        datingSimCamera.enabled = true;
        
        // fps mode disabled at the beginning
        fpsMode.SetActive(false);
        fpsCamera.enabled = false;
    }
    
    public void StartFPS()
    {
        fpsMode.SetActive(true);
        datingSimMode.enabled = false;
        ShowFPSCamera();
    }
    public void EndFPS()
    {
        fpsMode.SetActive(false);
        datingSimMode.enabled = true;
        ShowDatingSimCamera();
    }
    
    // boilerplate to expose a method to yarn runtime
    // https://docs.yarnspinner.dev/using-yarnspinner-with-unity/creating-commands-functions
    [YarnCommand("TestYarnUnityIntegration")]
    public static void TestYarnUnityIntegration() {
        Debug.Log($"I am called from yarn :)");
    }
    
    [YarnCommand("SetSprite")]
    public void SetSprite(string characterName) 
    {
        Debug.Log($"Switching to {characterName}");
        // Find the character in the CharacterList by name
        Character character = characterSO.CharacterList.Find(c => c.CharacterName == characterName);
        if (character != null)
        {
            characterImage.sprite = character.CharacterImage;
        }
        else
        {
            Debug.LogWarning($"Character '{characterName}' not found.");
        }
    }

    public void ReloadGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ShowDatingSimCamera()
    {
        datingSimCamera.enabled = true;
        fpsCamera.enabled = false;
    }

    public void ShowFPSCamera()
    {
        datingSimCamera.enabled = false;
        fpsCamera.enabled = true;
    }
}