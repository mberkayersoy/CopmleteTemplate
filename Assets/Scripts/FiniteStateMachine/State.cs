public abstract class State
{
    protected ThirdPersonCharacterController playerController;

    public State(ThirdPersonCharacterController sm)
    {
        playerController = sm;
    }

    public void OnStart(ThirdPersonCharacterController playerController)
    {
        // Code placed here will always run
        this.playerController = playerController;
        OnStart();
    }
    public virtual void OnStart() 
    {
        // Code placed here can be overridden
    }

    public void OnStateUpdate()
    {
        // Code placed here will always run
        OnUpdate();
    }

    public virtual void OnUpdate() 
    {

    }

    public void OnStateExit()
    {
        // Code placed here will always run
        OnExit();
    }

    public virtual void OnExit() 
    {

    }
}
