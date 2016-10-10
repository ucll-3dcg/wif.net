package viewer;

public class Color
{
    public int r, g, b;

    public Color()
    {
        this( 0, 0, 0 );
    }

    public Color( int r, int g, int b )
    {
        this.r = r;
        this.g = g;
        this.b = b;
    }

    public int asInt()
    {
        return 0xFF000000 | (r << 16) | (g << 8) | b;
    }

    public Color clamp()
    {
        int r = Math.min( 255, this.r );
        int g = Math.min( 255, this.g );
        int b = Math.min( 255, this.b );

        return new Color( r, g, b );
    }
}
