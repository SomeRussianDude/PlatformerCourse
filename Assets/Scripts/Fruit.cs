using UnityEngine;

public enum FruitType {Apple, Banana, Cherry, Kiwi, Melon, Orange, Pineapple, Strawberry}
public class Fruit : MonoBehaviour
{
  [SerializeField] private FruitType fruitType;
  [SerializeField] private GameObject _pickupVFX;
  
  private GameManager gm;
  private Animator anim;
  
  private void Awake()
  {
    anim = GetComponentInChildren<Animator>();
  }
  private void Start()
  {
    gm = GameManager.Instance;
    SetRandomFruitIfNeeded();
  }

  private void SetRandomFruitIfNeeded()
  {
    if (gm.FruitsHaveRandomLook == false)
    {
      UpdateFruitVisuals();
      return;}
    int randomFruit = Random.Range(0, 8);
    anim.SetFloat("fruitIndex", randomFruit);
  }

  private void UpdateFruitVisuals() => anim.SetFloat("fruitIndex", (int)fruitType);

  private void OnTriggerEnter2D(Collider2D collision)
  {
    Player player = collision.GetComponent<Player>();

    if (player != null)
    {
      gm.AddFruit();
      Destroy(gameObject);

      GameObject pickupVFX = Instantiate(_pickupVFX,transform.position,Quaternion.identity);
      
      //VFX object is destroyed via animation event
      //Destroy(pickupVFX, 0.5f);
    }
  }
}
