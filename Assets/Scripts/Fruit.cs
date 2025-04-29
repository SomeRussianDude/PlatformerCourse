using UnityEngine;

public class Fruit : MonoBehaviour
{
  private GameManager gm;
  private Animator anim;

  private void Awake()
  {
    anim = GetComponentInChildren<Animator>();
  }
  private void Start()
  {
    gm = GameManager.Instance;
  }

  private void OnTriggerEnter2D(Collider2D collision)
  {
    Player player = collision.GetComponent<Player>();

    if (player != null)
    {
      gm.AddFruit();
      Destroy(gameObject);
    }
  }
}
