namespace XDPM_App
{
    public class TabItemBox
    {
        public string TabItemName;
        public int CanvasCount;

        public TabItemBox() { }

        public TabItemBox(string name, int canvasCount)
        {
            TabItemName = name;
            CanvasCount = canvasCount;
        }
    }
}
