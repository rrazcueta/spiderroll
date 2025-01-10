using Rewired;
using UnityEngine;

public class PlayerUse : MonoBehaviour
{
    private IInteractableItem _item;
    private int playerId = 0;
    private Player player;

    void Awake()
    {
        player = ReInput.players.GetPlayer(playerId);
    }

    // Update is called once per frame
    void Update()
    {
        if (_item != null && player.GetButtonDown("Use"))
        {
            _item.Use();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        _item = other.GetComponent<IInteractableItem>();
        if (_item != null)
        {
            _item.Enter();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (_item != null)
        {
            _item.Exit();
            _item = null;
        }
    }
}
