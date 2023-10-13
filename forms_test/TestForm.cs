using System.Globalization;

namespace forms_test;

public partial class TestForm : Form
{
    public const int shapesNum = 7;
    public const int fieldWidth = 15;      //tiles
    public const int fieldHeight = 25;     //tiles
    public const int tileSize = 15;    //px
    public int[,] shape = new int[2, 4];
    private int[,] field = new int[fieldWidth, fieldHeight];
    public Bitmap bitField = new Bitmap(tileSize * fieldWidth, tileSize * fieldHeight);
    public PictureBox fieldPictureBox = new PictureBox();
    public Graphics graphics;
    public System.Windows.Forms.Timer tickTimer = new System.Windows.Forms.Timer();

    public TestForm()
    {
        InitializeComponent();

        graphics = Graphics.FromImage(bitField);

        for (int i = 0; i < fieldWidth; i++)
            field[i, fieldHeight - 1] = 1;

        for (int i = 0; i < fieldHeight; i++)
        {
            field[0, i] = 1;
            field[fieldWidth - 1, i] = 1;
        }

        SetShape();
        // FilledField();
    }
    public void SetShape()
    {
        int rnd = new Random(DateTime.Now.Millisecond).Next(shapesNum);
        System.Console.WriteLine(rnd);
        switch (rnd)
        {
            case 0: shape = new int[,] { { 1, 2, 3, 4 }, { 8, 8, 8, 8 } }; break;
            case 1: shape = new int[,] { { 1, 2, 1, 2 }, { 8, 8, 9, 9 } }; break;
            case 2: shape = new int[,] { { 1, 2, 3, 3 }, { 8, 8, 8, 9 } }; break;
            case 3: shape = new int[,] { { 1, 2, 3, 3 }, { 8, 8, 8, 7 } }; break;
            case 4: shape = new int[,] { { 2, 2, 3, 3 }, { 7, 8, 8, 9 } }; break;
            case 5: shape = new int[,] { { 2, 2, 3, 3 }, { 9, 8, 8, 7 } }; break;
            case 6: shape = new int[,] { { 2, 2, 3, 3 }, { 8, 7, 8, 9 } }; break;
        }
    }

    public void FilledField()
    {
        graphics.Clear(Color.Black);
        for (int i = 0; i < fieldWidth; i++)
            for (int j = 0; j < fieldHeight; j++)
                if (field[i, j] == 1)
                {
                    graphics.FillRectangle(Brushes.LightBlue, i * tileSize, j * tileSize, tileSize, tileSize);
                    graphics.DrawRectangle(Pens.Black, i * tileSize, j * tileSize, tileSize, tileSize);
                }

        for (int i = 0; i < 4; i++)
        {
            graphics.FillRectangle(Brushes.Red, shape[1, i] * tileSize, shape[0, i] * tileSize, tileSize, tileSize);
            graphics.DrawRectangle(Pens.Black, shape[1, i] * tileSize, shape[0, i] * tileSize, tileSize, tileSize);
        }

        fieldPictureBox.Image = bitField;
    }

    public bool FindMistake()
    {
        for (int i = 0; i < 4; i++)
            if (shape[1, i] >= fieldWidth || shape[0, i] >= fieldHeight ||
                shape[1, i] <= 0 || shape[0, i] <= 0 ||
                field[shape[1, i], shape[0, i]] == 1)
                return true;
        return false;
    }

    private void TestForm_KeyDown(object sender, KeyEventArgs e)
    {
        switch (e.KeyCode)
        {
            case Keys.Escape:
                Environment.Exit(0);
                break;
            case Keys.A:
                for (int i = 0; i < 4; i++)
                    shape[1, i]--;
                if (FindMistake())
                    for (int i = 0; i < 4; i++)
                        shape[1, i]++;
                break;
            case Keys.D:
                for (int i = 0; i < 4; i++)
                    shape[1, i]++;
                if (FindMistake())
                    for (int i = 0; i < 4; i++)
                        shape[1, i]--;
                break;
            case Keys.W:
                var shapeT = new int[2, 4];
                Array.Copy(shape, shapeT, shape.Length);
                int maxX = 0, maxY = 0;
                for (int i = 0; i < 4; i++)
                {
                    if (shape[0, i] > maxY)
                        maxY = shape[0, i];
                    if (shape[1, i] > maxX)
                        maxX = shape[1, i];
                }
                for (int i = 0; i < 4; i++)
                {
                    int temp = shape[0, i];
                    shape[0, i] = maxY - (maxX - shape[1, i]) - 1;
                    shape[1, i] = maxX - (3 - (maxY - temp)) + 1;
                }
                if (FindMistake())
                    Array.Copy(shapeT, shape, shape.Length);
                break;
            case Keys.S:
                tickTimer.Interval = 30;
                break;
        }
        FilledField();
    }

    private void TickTimer_Tick(object sender, EventArgs e)
    {
        if (field[8, 3] == 1)
            Environment.Exit(0);
        for (int i = 0; i < 4; i++)
            shape[0, i]++;
        if (FindMistake())
        {
            for (int i = 0; i < 4; i++)
                field[shape[1, i], --shape[0, i]]++;
            SetShape();
        }

        for (int i = fieldHeight - 2; i > 2; i--)
        {
            var cross = (
                        from t in Enumerable.Range(0, field.GetLength(0)).
                        Select(j => field[j, i]).
                        ToArray()
                        where t == 1
                        select t
                        ).Count();

            if (cross == fieldWidth)
                for (int k = i; k > 1; k--)
                    for (int l = 1; l < fieldWidth - 1; l++)
                        field[l, k] = field[l, k - 1];
        }

        FilledField();
        tickTimer.Interval = 1000;
    }


    private void InitializeComponent()
    {
        //this.components = new System.ComponentModel.Container();
        this.AutoScaleMode = AutoScaleMode.Font;
        this.FormBorderStyle = FormBorderStyle.FixedSingle;
        // this.ClientSize = new Size(bitField.Width, bitField.Height);
        this.Size = new Size(bitField.Width * 2, bitField.Height * 2);
        this.Text = "Tetris";
        this.KeyDown += new KeyEventHandler(TestForm_KeyDown);
        // this.KeyDown += TestForm_KeyDown;

        fieldPictureBox.Dock = DockStyle.Fill;
        fieldPictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
        // fieldPictureBox.BackColor = Color.Yellow;
        // fieldPictureBox.Paint += new PaintEventHandler(TestForm_Paint);
        this.Controls.Add(fieldPictureBox);
        tickTimer.Interval = 1000;
        tickTimer.Enabled = true;
        tickTimer.Tick += new EventHandler(TickTimer_Tick);
        // tickTimer.Tick += TickTimer_Tick;
    }


}

// private void TestForm_Paint(object sender, PaintEventArgs e)
// {
//     // Create a local version of the graphics object for the PictureBox.
//     Graphics g = e.Graphics;

//     // Draw a string on the PictureBox.
//     g.DrawString("This is a diagonal line drawn on the control",
//         new Font("Arial",10), System.Drawing.Brushes.Blue, new Point(30,30));
//     // Draw a line in the PictureBox.
//     g.DrawLine(System.Drawing.Pens.Red, fieldPictureBox.Left, fieldPictureBox.Top,
//         fieldPictureBox.Right, fieldPictureBox.Bottom);
// }



// private void TestForm_Paint(object sender, PaintEventArgs e)
// {
//     // Graphics graphics = this.CreateGraphics();
//     Rectangle rectangle = new Rectangle(100, 100, 200, 200);

//     e.Graphics.DrawEllipse(Pens.Black, rectangle);
//     e.Graphics.DrawRectangle(Pens.Red, rectangle);

//     rectangle.Offset(new Point(5, 5));
// }

