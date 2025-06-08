using UnityEngine;

public class EnemyPoints : MonoBehaviour
{
    public int pointsWhenDestroyed = 100; // Pontos que este inimigo vale

    void OnDestroy()
    {
        // Verifica se o objeto está sendo destruído durante o encerramento do jogo
        // A condição 'this.gameObject.scene.isLoaded' é boa para evitar erros
        // quando a aplicação está saindo ou a cena está sendo descarregada.
        if (this.gameObject.scene.isLoaded)
        {
            // Adiciona pontos ao sistema de pontuação
            // Devemos usar o ScoreManager para adicionar pontos ao score do jogo.
            if (ScoreManager.Instance != null && GameManager.Instance != null && GameManager.Instance.IsGameActive)
            {
                // CORREÇÃO AQUI: Chamar AddPoints no ScoreManager, não no ScoreOverTime.
                ScoreManager.Instance.AddPoints(pointsWhenDestroyed);
                Debug.Log($"Inimigo destruído! Adicionado {pointsWhenDestroyed} pontos. Score atual: {ScoreManager.Instance.CurrentGameScore}");
            }
            else if (ScoreManager.Instance == null)
            {
                Debug.LogWarning("EnemyPoints: ScoreManager.Instance não encontrado. Não foi possível adicionar pontos.");
            }
            else if (GameManager.Instance == null)
            {
                Debug.LogWarning("EnemyPoints: GameManager.Instance não encontrado. Não foi possível verificar o estado do jogo.");
            }
            else if (!GameManager.Instance.IsGameActive)
            {
                Debug.Log($"EnemyPoints: Jogo não está ativo. Pontos não adicionados ao destruir {gameObject.name}.");
            }
        }
    }
}