using System;

namespace Shard.UI
{
    class Slider : InputListener
    {
        private GameObject background;
        private GameObject fill;
        private GameObject handle;

        private float minValue;
        private float maxValue;
        public float currentValue { get; private set; }

        private bool isDragging = false;

        public Action<float> onValueChanged;

        public int X { get; set; }
        public int Y { get; set; }

        public Slider(string BackgroundSprite, string FillSprite, string HandleSprite, float MinValue, float MaxValue, float InitialValue, Action<float> OnValueChanged, int x, int y)
        {
            background = new GameObject { Transform = { SpritePath = Bootstrap.getAssetManager().getAssetPath(BackgroundSprite) } };
            fill = new GameObject { Transform = { SpritePath = Bootstrap.getAssetManager().getAssetPath(FillSprite) } };
            handle = new GameObject { Transform = { SpritePath = Bootstrap.getAssetManager().getAssetPath(HandleSprite) } };

            minValue = MinValue;
            maxValue = MaxValue;
            currentValue = Math.Clamp(InitialValue, minValue, maxValue);

            onValueChanged = OnValueChanged;

            X = x;
            Y = y;

            Bootstrap.getInput().addListener(this);
        }

        public void update()
        {
            background.Transform.X = X;
            background.Transform.Y = Y;

            Bootstrap.getDisplay().addToDraw(background);

            fill.Transform.Scalex = (currentValue - minValue) / (maxValue - minValue);
            fill.Transform.X = X;
            fill.Transform.Y = Y;
            Bootstrap.getDisplay().addToDraw(fill);

            int handleX = Math.Clamp(
                (int)(X + (currentValue - minValue) / (maxValue - minValue) * (background.Transform.Wid - handle.Transform.Wid)),
                X,
                X + background.Transform.Wid - handle.Transform.Wid
            );
            handle.Transform.X = handleX;
            handle.Transform.Y = Y;

            Bootstrap.getDisplay().addToDraw(handle);
        }

        public void handleInput(InputEvent inp, string eventType)
        {
            bool isMouseOverHandle = inp.X >= handle.Transform.X && inp.X <= handle.Transform.X + handle.Transform.Wid &&
                                     inp.Y >= handle.Transform.Y && inp.Y <= handle.Transform.Y + handle.Transform.Ht;

            if (eventType == "MouseDown" && inp.Button == 1 && isMouseOverHandle)
            {
                isDragging = true;
            }

            if (eventType == "MouseUp" && inp.Button == 1)
            {
                isDragging = false;
            }

            if (eventType == "MouseMotion" && isDragging)
            {
                int newHandleX = Math.Clamp(inp.X, X, X + background.Transform.Wid - handle.Transform.Wid);
                float newValue = MathUtils.Lerp(minValue, maxValue, (float)(newHandleX - X) / (background.Transform.Wid - handle.Transform.Wid));

                if (!MathUtils.Approximately(newValue, currentValue))
                {
                    currentValue = newValue;
                    onValueChanged?.Invoke(currentValue);
                }
            }
        }
    }
}
