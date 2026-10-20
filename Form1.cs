namespace WinFormsDraw
{

public partial class Form1 : Form
{
 private Bitmap _drawingBitmap;
        private Graphics _bitmapGraphics;
        private Point _lastPoint;
        private bool _isDrawing = false;

    public Form1()
    {
        //window setup
        this.Text = "My Drawing App";
            this.ClientSize = new Size(800, 600);
            this.DoubleBuffered = true;

         // Initialize canvas - draw on canvas
            _drawingBitmap = new Bitmap(this.ClientSize.Width, this.ClientSize.Height);
            _bitmapGraphics = Graphics.FromImage(_drawingBitmap);
            _bitmapGraphics.Clear(Color.White); // Start with a white background
            _bitmapGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Wire up the mouse events
            this.MouseDown += Form1_MouseDown;
            this.MouseMove += Form1_MouseMove;
            this.MouseUp += Form1_MouseUp;
            this.Paint += Form1_Paint;    

        InitializeComponent();
    }

     private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _isDrawing = true;
                _lastPoint = e.Location;
            }
        }

     private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDrawing)
            {
                // Draw a line from the last point to the current point
                using (Pen pen = new Pen(Color.Black, 3))
                {
                    _bitmapGraphics.DrawLine(pen, _lastPoint, e.Location);
                }
                _lastPoint = e.Location;
                this.Invalidate(); // form to repaint itself
            }
        }

    private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            _isDrawing = false;
        }

     private void Form1_Paint(object sender, PaintEventArgs e)
        {
            // Copy in-memory bitmap onto the form's surface
            e.Graphics.DrawImageUnscaled(_drawingBitmap, Point.Empty);
        }

     // Clean up resources when the form closes
    protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _bitmapGraphics.Dispose();
            _drawingBitmap.Dispose();
            base.OnFormClosed(e);
        }
}
    
}
