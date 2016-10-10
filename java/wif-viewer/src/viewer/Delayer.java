package viewer;

public class Delayer
{
    private Action action;
    
    private Countdown countdown; 
    
    public Delayer()
    {
        action = null;
        countdown = null;
    }
    
    public void tick()
    {
        if ( countdown != null && countdown.tick() )
        {            
            action.perform();
            
            countdown = null;
            action = null;
        }
    }
    
    public void delay(Action action, double seconds)
    {
        this.action = action;
        this.countdown = new Countdown( seconds );
    }
}
