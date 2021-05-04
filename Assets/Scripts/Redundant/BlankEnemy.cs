public class BlankEnemy : Enemy
{
    public override void Update()
    {
        // Update gun looking
        gun.transform.LookAt(leftHand);
    }
}
