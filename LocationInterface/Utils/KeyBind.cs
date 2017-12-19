using Microsoft.Xna.Framework.Input;
using System;

namespace LocationInterface.Utils
{
    public class KeyListener
    {
        public Keys Key { get; set; }
        public bool ExecuteOnce { get; protected set; }
        public Action Command { get; set; }

        public bool KeyDown { get; protected set; }

        /// <summary>
        /// Initialise a new KeyBind
        /// </summary>
        /// <param name="key">The key to bind to</param>
        /// <param name="command">The command to run on key press</param>
        public KeyListener(Keys key, Action command)
        {
            Key = key;
            Command = command;
            ExecuteOnce = true;
            KeyDown = false;
        }
        /// <summary>
        /// Initialise a new KeyBind
        /// </summary>
        /// <param name="key">The key to bind to</param>
        /// <param name="command">The command to run on key press</param>
        /// <param name="executeOnce">If true, execute the command only once when held down.</param>
        public KeyListener(Keys key, Action command, bool executeOnce)
        {
            Key = key;
            Command = command;
            ExecuteOnce = executeOnce;
            KeyDown = false;
        }

        /// <summary>
        /// Update the keybind state (poll to check if the key is down)
        /// </summary>
        public void Update(KeyboardState keyboardState)
        {
            // If the key is down and the key has not been down or execute once is not enabled
            if (keyboardState.IsKeyDown(Key) && !(KeyDown && ExecuteOnce))
            {
                // Invoke the command if it is not null
                Command?.Invoke();
                // Set the key as having been down
                KeyDown = true;
            }
            // Else if the key is not down and the key has been down
            else if (!keyboardState.IsKeyDown(Key) && KeyDown)
                // Set the key as not having been down
                KeyDown = false;
        }
    }
}
