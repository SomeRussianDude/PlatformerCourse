using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{
    private Player _player;
    void Awake()
    {
        _player = GetComponentInParent<Player>();
    }

    public void FinishRespawn() => _player.RespawnFinished(true);
}
