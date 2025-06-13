using UnityEngine;

public class ExitGameButton : MonoBehaviour
{
    public void QuitGame()
    {
        Debug.Log("<color=red>QuitGame: Tentando sair do aplicativo...</color>");

        // Se estiver rodando no Editor Unity, para a execução.
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            // Se estiver rodando em uma build (ex: APK no Android), fecha o aplicativo.
            Application.Quit();
        #endif

        // Note: Em algumas plataformas (como WebGL), Application.Quit() não faz nada.
        // Em outras (como Android/iOS), ele fecha o aplicativo.
    }
}