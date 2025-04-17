using UnityEngine;

public class GameManager : MonoBehaviour
{
   public static GameManager Instance;
   public Player player;
   public void Awake()
   {
      if (Instance == null)
         Instance = this;
      else
         Destroy(gameObject);
   }
}
