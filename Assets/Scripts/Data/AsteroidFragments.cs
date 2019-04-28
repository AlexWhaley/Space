public class AsteroidFragments : PoolObject
{
    public AsteroidFragmentsController Controller;
    
    protected override void PostInitialise()
    {
        Controller = Object.GetComponent<AsteroidFragmentsController>();
    }

    public override void ReturnedToPool()
    {
        base.ReturnedToPool();
        Controller.ResetFragments();
    }
}
