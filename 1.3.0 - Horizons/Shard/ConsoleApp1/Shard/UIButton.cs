using System;

namespace Shard.UI
{
    class Button : GameObject, InputListener
    {
        private string defaultSprite;
        private string hoverSprite;
        private string pressedSprite;

        private bool isHovered = false;
        private bool isPressed = false;

        public Action onClick;

        public Button(string DefaultSprite, Action OnClick, string HoverSprite = null, string PressedSprite = null)
        {
            defaultSprite = Bootstrap.getAssetManager().getAssetPath(DefaultSprite);
            onClick = OnClick;
            if (HoverSprite != null)
            {
                hoverSprite = Bootstrap.getAssetManager().getAssetPath(HoverSprite);
            }
            if(PressedSprite != null)
            {
                pressedSprite = Bootstrap.getAssetManager().getAssetPath(PressedSprite);
            }

            Transform.SpritePath = defaultSprite;

            Bootstrap.getInput().addListener(this);
        }
        public override void update()
        {
            Bootstrap.getDisplay().addToDraw(this);
        }

        public void handleInput(InputEvent inp, string eventType)
        {
            bool isMouseOver = inp.X >= Transform.X && inp.X <= Transform.X + Transform.Wid &&
                               inp.Y >= Transform.Y && inp.Y <= Transform.Y + Transform.Ht;

            if (eventType == "MouseMotion")
            {
                if (isMouseOver && !isHovered)
                {
                    isHovered = true;
                    if (hoverSprite != null)
                        Transform.SpritePath = hoverSprite;
                }
                else if (!isMouseOver && isHovered)
                {
                    isHovered = false;
                    Transform.SpritePath = defaultSprite;
                }
            }

            if (eventType == "MouseDown" && inp.Button == 1 && isMouseOver)
            {
                isPressed = true;
                if (pressedSprite != null)
                    Transform.SpritePath = pressedSprite;
            }

            if (eventType == "MouseUp" && inp.Button == 1 && isPressed)
            {
                isPressed = false;
                if (isMouseOver)
                {
                    onClick?.Invoke();
                    if (hoverSprite != null)
                        Transform.SpritePath = hoverSprite;
                    else
                        Transform.SpritePath = defaultSprite;
                }
                else
                {
                    Transform.SpritePath = defaultSprite;
                }
            }
        }
    }
}
