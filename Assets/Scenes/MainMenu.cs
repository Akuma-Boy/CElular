using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button quitButton;
    
    [Header("Scene Settings")]
    [SerializeField] private string gameSceneName = "GameScene"; // Nome da cena do jogo
    
    [Header("Audio")]
    [SerializeField] private AudioClip buttonClickSound;
    private AudioSource audioSource;

    private void Awake()
    {
        // Configura os listeners dos bot�es
        startButton.onClick.AddListener(StartGame);
        quitButton.onClick.AddListener(QuitGame);
        
        // Se tiver bot�o de op��es
        if(optionsButton != null)
        {
            optionsButton.onClick.AddListener(ShowOptions);
        }
        
        // Configura o audio source
        audioSource = GetComponent<AudioSource>();
        if(audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void StartGame()
    {
        PlayButtonSound();
        SceneManager.LoadScene(gameSceneName);
    }

    private void ShowOptions()
    {
        PlayButtonSound();
        // Implemente a l�gica para mostrar op��es aqui
        Debug.Log("Options button clicked");
    }

    private void QuitGame()
    {
        PlayButtonSound();
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    private void PlayButtonSound()
    {
        if(buttonClickSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(buttonClickSound);
        }
    }
}