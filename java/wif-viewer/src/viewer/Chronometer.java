package viewer;

public class Chronometer
{
    private long lastTick;
    
    public Chronometer()
    {
        lastTick = System.currentTimeMillis();
    }
    
    public double getElapsedTime()
    {
        long now = System.currentTimeMillis();
        long delta = now-lastTick;
        lastTick = now;
        
        return delta / 1000.0;        
    }
}
