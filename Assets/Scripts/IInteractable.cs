using UnityEngine;

interface IInteractable
{
    public void Interact(Player player);
    public void LookAt(Player player);
    public void LookAway(Player player);
}
