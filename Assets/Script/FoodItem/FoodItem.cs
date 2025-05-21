using UnityEngine;

public class FoodItem : MonoBehaviour
{
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    void Start()
    {
        // »ìÂ¦ À§·Î Æ¢¾î³ª¿Àµµ·Ï
        rb.AddForce(Vector2.up * 3f, ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}
