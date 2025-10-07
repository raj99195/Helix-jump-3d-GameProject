using UnityEngine;

public class JumpBoost : MonoBehaviour
{


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var player = other.GetComponent<Player>();
            if (!player.IsDied)
            {
                player.Boosted = true;
                Destroy(gameObject);
            }
        }
    }
}