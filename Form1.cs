using System.Drawing.Imaging;
namespace WinFormsDraw
{

public partial class Form1 : Form
{
    // Canvas variables
        private Bitmap _drawingBitmap;
        private Graphics _bitmapGraphics;

    // Drawing state variables
        private Point _lastPoint;
        private bool _isDrawing = false;

    // Tool variables
        private Color _currentColor = Color.Black;
        private int _currentThickness = 3;
        private bool _isEraserMode = false;

    // UI elements
        private Panel _toolbar;
        private Button _colorButton;
        private Button _eraserButton;
        private Button _clearButton;
        private Button _saveButton;
        private Button _loadButton;
        private NumericUpDown _sizeSelector;
        private Label _statusLabel;




    public Form1()
    {
        //window setup
        this.Text = "Let's Draw!";
            this.ClientSize = new Size(800, 600);
            this.DoubleBuffered = true;
            this.BackColor = Color.WhiteSmoke;

        // Create toolbar
        BuildToolbar();

        // Create a canvas below the toolbar
         int canvasWidth = this.ClientSize.Width;
         int canvasHeight = this.ClientSize.Height - _toolbar.Height;

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

        UpdateStatus();
    }




    // Toolbar setup
    private void BuildToolbar()
    {
        _toolbar = new Panel
        {
            Dock = DockStyle.Top,
            Height = 50,
            BackColor = Color.LightGray,
            Padding = new Padding(5)
        };
        this.Controls.Add(_toolbar);


         // Colour picker
        _colorButton = new Button
        {
            Text = "Colour",
            Location = new Point(10, 10),
            Size = new Size(70, 30),
            BackColor = Color.White
        };
        _colorButton.Click += ColorButton_Click;
        _toolbar.Controls.Add(_colorButton);



         // Eraser button
        _eraserButton = new Button
        {
            Text = "Eraser",
            Location = new Point(90, 10),
            Size = new Size(70, 30),
            BackColor = Color.White
        };
        _eraserButton.Click += EraserButton_Click;
        _toolbar.Controls.Add(_eraserButton);


        // Brush size label
        var sizeLabel = new Label
        {
            Text = "Size:",
            Location = new Point(410, 10),
            Size = new Size(40, 30),
        };
        _toolbar.Controls.Add(sizeLabel);


        // Brush size selector
        _sizeSelector = new NumericUpDown
        {
            Location = new Point(450, 10),
            Size = new Size(50, 30),
            Minimum = 1,
            Maximum = 20,
            Value = 3
        };
        _sizeSelector.ValueChanged += (s, e) => _currentThickness = (int)_sizeSelector.Value;
        _toolbar.Controls.Add(_sizeSelector);



        // Clear button
        _clearButton = new Button
        {
            Text = "Clear",
            Location = new Point(170, 10),
            Size = new Size(70, 30),
            BackColor = Color.White
        };
        _clearButton.Click += ClearButton_Click;
        _toolbar.Controls.Add(_clearButton);


        // Save button
        _saveButton = new Button
        {
            Text = "Save",
            Location = new Point(250, 10),
            Size = new Size(70, 30),
            BackColor = Color.White
        };
        _saveButton.Click += SaveButton_Click;
        _toolbar.Controls.Add(_saveButton);



        // Load button
        _loadButton = new Button
        {
            Text = "Load",
            Location = new Point(330, 10),
            Size = new Size(70, 30),
            BackColor = Color.White
        };
        _loadButton.Click += LoadButton_Click;
        _toolbar.Controls.Add(_loadButton);



       // Status label (shows current tool/color)
            _statusLabel = new Label
            {
                Location = new Point(600, 15),
                Size = new Size(280, 20),
                Text = "Tool: Pen"
            };
            _toolbar.Controls.Add(_statusLabel);

     }



     // Toolbar button event handlers
      private void ColorButton_Click(object sender, EventArgs e)
        {
            using (var dialog = new ColorDialog())
            {
                dialog.Color = _currentColor;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    _currentColor = dialog.Color;
                    _isEraserMode = false; // picking a color exits eraser mode
                    _colorButton.BackColor = _currentColor;
                    UpdateStatus();
                }
            }
        }

        private void EraserButton_Click(object sender, EventArgs e)
        {
            _isEraserMode = !_isEraserMode;
            _eraserButton.BackColor = _isEraserMode ? Color.LightCoral : Color.White;
            UpdateStatus();
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            var confirm = MessageBox.Show(
                "Clear the entire canvas?", "Confirm",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes)
            {
                _bitmapGraphics.Clear(Color.White);
                this.Invalidate();
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            using (var dialog = new SaveFileDialog())
            {
                dialog.Filter = "PNG Image|*.png|JPEG Image|*.jpg|Bitmap Image|*.bmp";
                dialog.Title = "Save Drawing";
                dialog.FileName = "drawing.png";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    // Determine format from extension
                    ImageFormat format = ImageFormat.Png;
                    if (dialog.FileName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase))
                        format = ImageFormat.Jpeg;
                    else if (dialog.FileName.EndsWith(".bmp", StringComparison.OrdinalIgnoreCase))
                        format = ImageFormat.Bmp;

                    try
                    {
                        _drawingBitmap.Save(dialog.FileName, format);
                        MessageBox.Show("Saved successfully!", "Save",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Save failed: " + ex.Message, "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void LoadButton_Click(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Filter = "Image Files|*.png;*.jpg;*.jpeg;*.bmp";
                dialog.Title = "Load Drawing";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (var img = Image.FromFile(dialog.FileName))
                        {
                            // Replace the canvas with the loaded image
                            _bitmapGraphics.Dispose();
                            _drawingBitmap.Dispose();

                            _drawingBitmap = new Bitmap(img);
                            _bitmapGraphics = Graphics.FromImage(_drawingBitmap);
                            _bitmapGraphics.SmoothingMode =
                                System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                        }
                        this.Invalidate();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Load failed: " + ex.Message, "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

    
    // Mouse event handlers

     private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            // Ignore clicks on the toolbar
            if (e.Y < _toolbar.Height) return;

            if (e.Button == MouseButtons.Left)
            {
                _isDrawing = true;
                // Convert form coordinates to canvas coordinates
                _lastPoint = new Point(e.X, e.Y - _toolbar.Height);
            }
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_isDrawing) return;
            if (e.Y < _toolbar.Height) return;

            Point currentPoint = new Point(e.X, e.Y - _toolbar.Height);

            using (Pen pen = new Pen(_isEraserMode ? Color.White : _currentColor, _currentThickness))
            {
                pen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
                pen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
                _bitmapGraphics.DrawLine(pen, _lastPoint, currentPoint);
            }

            _lastPoint = currentPoint;
            this.Invalidate();
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            _isDrawing = false;
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            // Draw the canvas below the toolbar
            e.Graphics.DrawImageUnscaled(_drawingBitmap, 0, _toolbar.Height);
        }



        // Help - updates status label with current tool/colour
         private void UpdateStatus()
        {
            string tool = _isEraserMode ? "Eraser" : "Pen";
            _statusLabel.Text = $"Tool: {tool}  |  Color: {_currentColor.Name}  |  Size: {_currentThickness}";
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _bitmapGraphics.Dispose();
            _drawingBitmap.Dispose();
            base.OnFormClosed(e);
        }
}
    
}
