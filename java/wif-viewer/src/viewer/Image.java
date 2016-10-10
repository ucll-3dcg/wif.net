package viewer;

import java.awt.image.BufferedImage;
import java.io.IOException;
import java.nio.file.Path;


public class Image
{
    public final Color[][] pixels;
    
    public final int width;
    
    public final int height;
    
    public Image(int width, int height)
    {
        this.width = width;
        this.height = height;
        this.pixels = new Color[width][height];        
    }
    
    public BufferedImage convertToBufferedImage()
    {
        BufferedImage image = new BufferedImage( width, height, BufferedImage.TYPE_INT_ARGB );

        for ( int y = 0; y != height; ++y )
        {
            for ( int x = 0; x != width; ++x )
            {
                image.setRGB( x,  y, pixels[x][y].clamp().asInt() );
            }
        }
        
        return image;
    }
}
