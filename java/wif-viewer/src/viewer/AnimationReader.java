package viewer;

import java.io.IOException;
import java.nio.ByteBuffer;
import java.nio.ByteOrder;
import java.nio.file.FileSystems;
import java.nio.file.Files;
import java.nio.file.Path;
import java.util.ArrayList;
import java.util.List;

public class AnimationReader
{
    private final ByteBuffer bb;

    public AnimationReader( String path ) throws IOException
    {
        this( FileSystems.getDefault().getPath( path ) );
    }

    public AnimationReader( Path path ) throws IOException
    {
        this( Files.readAllBytes( path ) );
    }

    public AnimationReader( byte[] bytes )
    {
        bb = ByteBuffer.wrap( bytes );
        bb.order( ByteOrder.nativeOrder() );
    }

    public List<Image> loadAnimation()
    {
        List<Image> frames = new ArrayList<Image>();
        Image frame;

        while ( (frame = loadImage() ) != null )
        {
            frames.add(frame);
        }

        return frames;
    }

    private Image loadImage()
    {
        int width = readWidth();

        if ( width == 0 )
        {
            return null;
        }
        else
        {
            int height = readHeight();
            Image image = new Image(width, height);

            for (int y = 0; y != height; ++y) {
                for (int x = 0; x != width; ++x) {
                    image.pixels[x][y] = readColor();
                }
            }

            return image;
        }
    }

    private int readWidth()
    {
        return bb.getInt();
    }

    private int readHeight()
    {
        return bb.getInt();
    }

    private Color readColor()
    {
        int r = bb.get() & 0xFF;
        int g = bb.get() & 0xFF;
        int b = bb.get() & 0xFF;

        return new Color( r, g, b );
    }
}
