package viewer;

import java.awt.image.BufferedImage;
import java.io.IOException;
import java.nio.file.Path;
import java.util.ArrayList;
import java.util.List;

public class Animation
{
    private final ArrayList<BufferedImage> frames;

    private int currentFrame;

    public Animation( List<Image> frames )
    {
        this.frames = new ArrayList<>();

        for ( Image frame : frames )
        {
            this.frames.add( frame.convertToBufferedImage() );
        }

        currentFrame = -1;
    }

    public BufferedImage nextFrame()
    {
        currentFrame = (currentFrame + 1) % frames.size();

        return frames.get( currentFrame );
    }
    
    public int getFrameCount()
    {
        return frames.size();
    }
    
    public BufferedImage getFrame(int index)
    {
        return frames.get( index );
    }

    public static Animation loadFromFile(Path path) throws IOException
    {
        return new Animation( new AnimationReader(path).loadAnimation() );
    }
}
