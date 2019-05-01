using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SDL2.SDL;

namespace VCS
{
    public class Viewport
    {
        public string Name { get; set; }
        public Renderer Renderer { get; set; }
        public List<Sprite> Sprites { get; set; } = new List<Sprite>();
        public int X { get; set; } = 0;
        public int Y { get; set; } = 0;
        public int Z { get; set; } = 0;
        public int Width { get; set; } = -1;
        public int Height { get; set; } = -1;
        public bool Disposed { get; set; } = false;
        public bool Visible { get; set; } = true;

        public Viewport(Renderer Renderer, int Width, int Height)
        {
            this.Renderer = Renderer;
            this.Width = Width;
            this.Height = Height;
            this.Renderer.Viewports.Add(this);
        }

        public void Update()
        {
            this.Renderer.Update();
        }

        public void Dispose()
        {
            for (int i = 0; i < Sprites.Count; i++)
            {
                Sprites[i].Dispose();
                Sprites.RemoveAt(i);
            }
            this.Disposed = true;
        }

        public void ForceUpdate()
        {
            this.Renderer.ForceUpdate();
        }
    }
}
