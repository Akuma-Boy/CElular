using UnityEngine;

public class EnemyPoints : MonoBehaviour
{
    public int pointsWhenDestroyed = 100; // Pontos que este inimigo vale

    void OnDestroy()
    {
        // Verifica se o objeto está sendo destruído durante o encerramento do jogo
        if (this.gameObject.scene.isLoaded)
        {
            // Adiciona pontos ao sistema de pontuação
            if (ScoreOverTime.Instance != null)
            {
                ScoreOverTime.Instance.AddScore(pointsWhenDestroyed);
            }
        }
    }
}
