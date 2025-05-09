using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
   public static GameManager Instance;
   [Header("Player")] 
   [SerializeField] private GameObject _playerPrefab;
   [SerializeField] private Transform _respawnPoint;
   [SerializeField] private float _respawnDelay;
   
   public Player player;

   [Header("Fruit GameManager")] 
   public bool _fruitsHaveRandomLook;
   public int _fruitsCollected = 0;
   public void Awake()
   {
      if (Instance == null)
         Instance = this;
      else
         Destroy(gameObject);
   }

   public void UpdateRespawnPosition(Transform newRespawnPoint) => _respawnPoint = newRespawnPoint;
   
   public void RespawnPlayer() => StartCoroutine(RespawnCoroutine());

   private IEnumerator RespawnCoroutine()
   {
      yield return new WaitForSeconds(_respawnDelay);
      GameObject newPlayer = Instantiate(_playerPrefab, _respawnPoint.position, Quaternion.identity);
      player = newPlayer.GetComponent<Player>();
   }

   public void AddFruit() => _fruitsCollected++;
   
   public bool FruitsHaveRandomLook => _fruitsHaveRandomLook;
}
