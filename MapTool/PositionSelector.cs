using System.Drawing;
using System.Windows.Forms;
using Capricorn.Drawing;
using Capricorn.IO;

namespace ChaosTool
{
    internal partial class PositionSelector : Form
    {
        private MainForm MainForm;

        internal PositionSelector(MainForm mainForm, string fieldName = "field001")
        {
            MainForm = mainForm;

            InitializeComponent();

            var archive = DATArchive.FromFile($@"{Paths.DarkAgesDir}setoa.dat");
            var epf = EPFImage.FromArchive($@"{fieldName}.epf", archive);
            var pal = Palette256.FromArchive($@"{fieldName}.pal", archive);

            BackgroundImage = DAGraphics.RenderImage(epf[0], pal);
            BackgroundImageLayout = ImageLayout.Stretch;
            ClientSize = new Size(640, 480);
        }

        private void PositionSelector_MouseClick(object sender, MouseEventArgs e)
        {
            MainForm.NodePositionXNum.Value = e.Location.X;
            MainForm.NodePositionYNum.Value = e.Location.Y;

            Close();
        }
    }
}
