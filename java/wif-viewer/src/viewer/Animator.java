package viewer;

import java.awt.image.BufferedImage;

public class Animator
{
    private final Animation animation;

    private int currentFrameIndex;
    
    public Animator(Animation animation)
    {
        this.animation = animation;
        this.currentFrameIndex = -1;
    }
    
    public Animation getAnimation()
    {
        return animation;
    }
    
    public BufferedImage nextFrame()
    {
        currentFrameIndex = (currentFrameIndex + 1) % animation.getFrameCount();
        
        return animation.getFrame( currentFrameIndex );
    }
}
