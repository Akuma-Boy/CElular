using UnityEngine;

public class MissilGuiado : MonoBehaviour
{
    private GameObject alvo;
    private float velocidade;
    private float velocidadeRot;
    private float tempoVida;

    public void Configurar(GameObject alvoSelecionado, float vel, float velRot, float vida)
    {
        alvo = alvoSelecionado;
        velocidade = vel;
        velocidadeRot = velRot;
        tempoVida = vida;

        Destroy(gameObject, vida);
    }

    private void Update()
    {
        if (alvo == null || !alvo.activeInHierarchy || alvo.transform.position.x < transform.position.x)
        {
            // Vai em linha reta se não houver alvo válido ou alvo foi para trás
            transform.Translate(Vector2.right * velocidade * Time.deltaTime);
            return;
        }

        Vector2 direcao = (Vector2)alvo.transform.position - (Vector2)transform.position;
        direcao.Normalize();

        float rotacaoZ = Mathf.Atan2(direcao.y, direcao.x) * Mathf.Rad2Deg;
        Quaternion rotacaoDesejada = Quaternion.Euler(0f, 0f, rotacaoZ);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotacaoDesejada, velocidadeRot * Time.deltaTime);

        transform.Translate(Vector2.right * velocidade * Time.deltaTime);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == alvo)
        {
            Destroy(gameObject);
        }
    }
}
