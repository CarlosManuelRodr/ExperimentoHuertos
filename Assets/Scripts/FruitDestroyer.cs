using UnityEngine;

public class FruitDestroyer : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "ItemA" || other.tag == "ItemB")
        {
            Destroy(other.gameObject);
        }
    }
}
