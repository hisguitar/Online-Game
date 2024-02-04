using Unity.Netcode;
using UnityEngine;

public class PlayerAiming : NetworkBehaviour
{
    [SerializeField] private InputReader inputReader;
    [SerializeField] private Transform weaponPivot;

    private void LateUpdate()
    {
        if (!IsOwner) { return; }

        Vector2 aimScreenPosition = inputReader.AimPosition;
        Vector2 aimWorldPosition = Camera.main.ScreenToWorldPoint(aimScreenPosition);

        weaponPivot.up = new Vector2(
            aimWorldPosition.x - weaponPivot.position.x,
            aimWorldPosition.y - weaponPivot.position.y);
    }
}