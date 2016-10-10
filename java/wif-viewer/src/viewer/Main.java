package viewer;

import java.awt.image.BufferedImage;
import java.io.File;
import java.io.IOException;
import java.nio.file.FileSystems;
import java.nio.file.Path;

import javax.imageio.ImageIO;

public class Main
{
	private final static String DEFAULT_PATH = "e:\\temp\\output\\test.wif";
	
    public static void main(String[] args) throws IOException
    {
        if ( args.length == 0 )
        {
            System.err.println( "Missing command line arguments" );
            System.err.println( "Using default path " + DEFAULT_PATH );
            System.err.println( "Feel free to modify the source code so as to use a different default path" );

            view( DEFAULT_PATH );
        }
        else if ( args.length == 1 )
        {
            view( args[0] );
        }
        else
        {
            switch ( args[0].toLowerCase() )
            {
            case "view":
                view( args[1] );
                break;

            case "convert":
                convert( args[1], args[2] );
                break;
            }
        }
    }

    private static Path toPath(String filename)
    {
        return FileSystems.getDefault().getPath( filename );
    }

    private static void view(String filename) throws IOException
    {
        new Viewer( toPath( filename ).toAbsolutePath() );
    }

    private static void convert(String input, String outputPattern) throws IOException
    {
        Animation animation = Animation.loadFromFile( toPath( input ) );

        if ( animation.getFrameCount() == 1 )
        {
            convertFrame( animation.getFrame( 0 ), outputPattern );
        }
        else
        {
            if ( !outputPattern.contains( "%d" ) )
            {
                System.err.println( "%s contains multiple frames; please specify output file with %d to enable indexing" );
            }
            else
            {
                for ( int i = 0; i != animation.getFrameCount(); ++i )
                {
                    String filename = String.format( outputPattern, i );
                    BufferedImage frame = animation.getFrame( i );

                    convertFrame( frame, filename );
                }
            }
        }
    }

    private static void convertFrame(BufferedImage frame, String output) throws IOException
    {
        ImageIO.write( frame, "PNG", new File( output ) );
    }
}
