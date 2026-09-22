public class Rigidbody_velocity : RigidbodyBase
{
    /// <inheritdoc />
    public override void Update()
    {
        base.Update();

        rb.linearVelocity = moveDirection;
    }
}
