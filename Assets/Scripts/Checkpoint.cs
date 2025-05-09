using Unity.VisualScripting;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
  private Animator _animator => GetComponent<Animator>();
  private bool _isActivated;

  private void OnTriggerEnter2D(Collider2D collision)
  {
    if (_isActivated)
      return;
    
    Player player = collision.GetComponent<Player>();
    if (player != null)
    {
      ActivateCheckpoint();
    }
  }

  private void ActivateCheckpoint()
  {
    _isActivated = true;
    _animator.SetTrigger("activated");
    GameManager.Instance.UpdateRespawnPosition(transform);
  }
}
