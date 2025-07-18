using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MovieCatalogGUI;

public enum SlideDirection
{
    Right,
    Bottom
}

public class SlidingPanelContainer : Panel
{
    private readonly Stack<UserControl> panelStack = new Stack<UserControl>();
    private readonly int panelWidth;

    public SlidingPanelContainer(int width)
    {
        this.panelWidth = width;
        this.Dock = DockStyle.Right;
        this.Width = 0;
        this.BackColor = Color.Transparent;
    }

    public int PanelCount => panelStack.Count;

    public UserControl TopPanel => panelStack.Count > 0 ? panelStack.Peek() : null;

    /// <summary>
    /// Returns true if a panel of the given type is already in the stack.
    /// </summary>
    public bool ContainsPanelOfType(Type panelType)
    {
        foreach (var panel in panelStack)
        {
            if (panel.GetType() == panelType)
                return true;
        }
        return false;
    }

    public async Task SlideIn(UserControl panel, SlideDirection direction = SlideDirection.Right)
    {
        // Prevent duplicate panels of the same type (optional)
        // if (ContainsPanelOfType(panel.GetType())) return;

        panelStack.Push(panel);

        if (direction == SlideDirection.Right)
        {
            panel.Width = panelWidth;
            panel.Height = this.Height;
            panel.Left = this.Width;
            panel.Top = 0;
            panel.Dock = DockStyle.None;
            this.Controls.Add(panel);
            panel.BringToFront();

            int steps = 20;
            int startWidth = this.Width;
            int targetWidth = panelWidth * panelStack.Count;
            for (int i = 0; i <= steps; i++)
            {
                this.Width = startWidth + (targetWidth - startWidth) * i / steps;
                await Task.Delay(8);
            }
            this.Width = targetWidth;
        }
        else if (direction == SlideDirection.Bottom)
        {
            panel.Width = this.Width;
            panel.Height = panelWidth;
            panel.Left = 0;
            panel.Top = this.Height;
            panel.Dock = DockStyle.None;
            this.Controls.Add(panel);
            panel.BringToFront();

            int steps = 20;
            int startHeight = this.Height;
            int targetHeight = this.Height + panelWidth;
            for (int i = 0; i <= steps; i++)
            {
                panel.Top = startHeight - (panelWidth * i / steps);
                await Task.Delay(8);
            }
            panel.Top = this.Height - panelWidth;
        }
    }

    public async Task SlideOut()
    {
        if (panelStack.Count == 0) return;
        var panel = panelStack.Pop();

        int steps = 20;
        int startWidth = this.Width;
        int targetWidth = panelWidth * panelStack.Count;
        for (int i = 0; i <= steps; i++)
        {
            this.Width = startWidth + (targetWidth - startWidth) * i / steps;
            await Task.Delay(8);
        }
        this.Width = targetWidth;
        this.Controls.Remove(panel);
        panel.Dispose();
    }

    public bool IsPanelShown => panelStack.Count > 0;
}