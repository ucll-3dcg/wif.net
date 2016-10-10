package viewer;

import java.awt.Dimension;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.awt.image.BufferedImage;
import java.io.IOException;
import java.nio.file.FileSystem;
import java.nio.file.FileSystems;
import java.nio.file.Path;
import java.nio.file.StandardWatchEventKinds;
import java.nio.file.WatchEvent;
import java.nio.file.WatchKey;
import java.nio.file.WatchService;

import javax.swing.ImageIcon;
import javax.swing.JFrame;
import javax.swing.JLabel;
import javax.swing.SwingUtilities;
import javax.swing.Timer;

public class Viewer
{
    private JFrame frame;

    private JLabel label;

    private Delayer delayer;

    private Animator animator;

    public Viewer( final Path path ) throws IOException
    {
        this.delayer = new Delayer();

        final Path parentPath = path.getParent();
        final WatchService watcher = createWatcher( parentPath );

        SwingUtilities.invokeLater( new Runnable()
        {
            @Override
            public void run()
            {
                frame = new JFrame( "Viewer" );
                frame.setDefaultCloseOperation( JFrame.EXIT_ON_CLOSE );

                label = new JLabel( (ImageIcon) null );
                frame.add( label );

                frame.setVisible( true );

                loadAnimation( path );

                new Timer( 50, new ActionListener()
                {
                    @Override
                    public void actionPerformed(ActionEvent arg0)
                    {
                        showNextFrame();
                        delayer.tick();
                        
                        WatchKey key = watcher.poll();

                        if ( key != null )
                        {
                            for ( WatchEvent<?> event : key.pollEvents() )
                            {
                                @SuppressWarnings("unchecked")
                                WatchEvent<Path> pathEvent = (WatchEvent<Path>) event;

                                if ( pathEvent.context().getFileName().equals( path.getFileName() ) )
                                {
                                    System.out.println( "Update detected" );

                                    delayer.delay( new Action()
                                    {
                                        @Override
                                        public void perform()
                                        {
                                            loadAnimation( path );
                                        }
                                    }, 1.0 );
                                }
                            }

                            key.reset();
                        }
                    }
                } ).start();
            }
        } );
    }

    private void loadAnimation(Path path)
    {
        try
        {
            Animation animation = Animation.loadFromFile( path );            
            
            System.out.println(String.format("Loaded animation with %d frame(s)", animation.getFrameCount()));
            
            this.animator = new Animator(animation);

            showNextFrame();
        }
        catch ( IOException e )
        {
            System.out.println( e );
        }
    }
    
    private void showNextFrame()
    {
        showImage( animator.nextFrame() );
    }

    private void showImage(BufferedImage image)
    {
        label.setIcon( new ImageIcon( image ) );
        label.setPreferredSize( new Dimension( image.getWidth(), image.getHeight() ) );

        frame.invalidate();
        frame.pack();
    }

    private static WatchService createWatcher(Path path) throws IOException
    {
        FileSystem fileSystem = FileSystems.getDefault();
        WatchService watchService = fileSystem.newWatchService();

        path.register( watchService, StandardWatchEventKinds.ENTRY_MODIFY );

        return watchService;
    }
}
