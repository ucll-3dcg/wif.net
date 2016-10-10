package viewer;

public class Countdown
{
    private final Chronometer chronometer;
    
    private double timeLeft;
    
    public Countdown(double seconds)
    {
        chronometer = new Chronometer();
        this.timeLeft = seconds;
    }
    
    public boolean tick()
    {
        timeLeft -= chronometer.getElapsedTime();
        
        return timeLeft <= 0;
    }
}
