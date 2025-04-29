using UnityEngine;

public class GameManager : MonoBehaviour
{
   public static GameManager Instance;
   public Player player;
   public int _fruitsCollected = 0;
   public void Awake()
   {
      if (Instance == null)
         Instance = this;
      else
         Destroy(gameObject);
   }

   public void AddFruit()
   {
      _fruitsCollected++;
   }
}
