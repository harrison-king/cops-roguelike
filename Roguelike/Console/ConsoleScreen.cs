// <copyright file="ConsoleScreen.cs" company="Jesse King">
// Copyright (c) Jesse King. All rights reserved.
// </copyright>

namespace Roguelike.Common.Console
{
    using System;

    /// <summary>
    /// Handles writing to a buffered console to avoid flickering.
    /// </summary>
    /// <remarks>
    /// This class is thread-safe.
    /// </remarks>
    public static class ConsoleScreen
    {
        /// <summary>
        /// The width of the console window.
        /// </summary>
        private static int windowWidth;

        /// <summary>
        /// The height of the console window.
        /// </summary>
        private static int windowHeight;

        /// <summary>
        /// The width of the console screen buffer.
        /// </summary>
        private static int bufferWidth;

        /// <summary>
        /// The height of the console screen buffer.
        /// </summary>
        private static int bufferHeight;

        /// <summary>
        /// The current screen buffer displayed.
        /// </summary>
        private static ConsoleChar[,] screenBuffer;

        /// <summary>
        /// The back buffer that is written to between draw calls.
        /// </summary>
        private static ConsoleChar[,] backBuffer;

        /// <summary>
        /// The default font color of the console.
        /// </summary>
        private static ConsoleColor foregroundColor;

        /// <summary>
        /// The default color of the console background.
        /// </summary>
        private static ConsoleColor backgroundColor;

        /// <summary>
        /// Whether to display the console cursor.
        /// </summary>
        private static bool displayCursor = false;

        /// <summary>
        /// The screen lock for thread-safety.
        /// </summary>
        private static object screenLock = new object();

        /// <summary>
        /// Initializes static members of the <see cref="ConsoleScreen"/> class.
        /// </summary>
        static ConsoleScreen()
        {
            // Initialize the console screen with the current console window properties.
            Initialize();
        }

        /// <summary>
        /// Gets the current width of the window.
        /// </summary>
        public static int WindowWidth
        {
            get
            {
                lock (ConsoleScreen.screenLock)
                {
                    return ConsoleScreen.windowWidth;
                }
            }
        }

        /// <summary>
        /// Gets the current height of the window.
        /// </summary>
        public static int WindowHeight
        {
            get
            {
                lock (ConsoleScreen.screenLock)
                {
                    return ConsoleScreen.windowHeight;
                }
            }
        }

        /// <summary>
        /// Gets the default foreground (font) color of the console.
        /// </summary>
        public static ConsoleColor ForegroundColor
        {
            get
            {
                lock (ConsoleScreen.screenLock)
                {
                    return ConsoleScreen.foregroundColor;
                }
            }
        }

        /// <summary>
        /// Gets the default background color of the console.
        /// </summary>
        public static ConsoleColor BackgroundColor
        {
            get
            {
                lock (ConsoleScreen.screenLock)
                {
                    return ConsoleScreen.backgroundColor;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to display the cursor in the console.
        /// </summary>
        public static bool DisplayCursor
        {
            get
            {
                lock (ConsoleScreen.screenLock)
                {
                    return ConsoleScreen.displayCursor;
                }
            }

            set
            {
                lock (ConsoleScreen.screenLock)
                {
                    ConsoleScreen.displayCursor = value;
                }
            }
        }

        /// <summary>
        /// Initializes the console screen.
        /// </summary>
        /// <remarks>
        /// Initializes to the current console width and height,
        /// as well as the current background and foreground colors.
        /// </remarks>
        public static void Initialize()
        {
            Initialize(Console.WindowWidth, Console.WindowHeight, Console.BackgroundColor, Console.ForegroundColor);
        }

        /// <summary>
        /// Initializes the console screen.
        /// </summary>
        /// <param name="width">The width of the console window measured in columns.</param>
        /// <param name="height">The height of the console window measured in rows.</param>
        /// <remarks>
        /// Initializes to the specified width and height,
        /// using the current background and foreground colors.
        /// </remarks>
        public static void Initialize(int width, int height)
        {
            Initialize(width, height, Console.BackgroundColor, Console.ForegroundColor);
        }

        /// <summary>
        /// Initializes the console screen.
        /// </summary>
        /// <param name="width">The width of the console window measured in columns.</param>
        /// <param name="height">The height of the console window measured in rows.</param>
        /// <param name="backgroundColor">The background color of the console.</param>
        /// <param name="foregroundColor">The foreground color of the console (font).</param>
        /// <param name="displayCursor">Whether to display the console cursor.</param>
        public static void Initialize(int width, int height, ConsoleColor backgroundColor, ConsoleColor foregroundColor, bool displayCursor = false)
        {
            lock (ConsoleScreen.screenLock)
            {
                // Initialize the console buffers.
                ConsoleScreen.bufferWidth = width;
                ConsoleScreen.bufferHeight = height;
                ConsoleScreen.screenBuffer = new ConsoleChar[width, height];
                ConsoleScreen.backBuffer = new ConsoleChar[width, height];

                // Initialize the screen size.
                ConsoleScreen.windowWidth = Math.Max(width, Console.WindowWidth);
                ConsoleScreen.windowHeight = Math.Max(height, Console.WindowHeight);

                try
                {
                    // Attempt to set the console size (not implemented on all platforms).
                    Console.SetWindowSize(ConsoleScreen.windowWidth, ConsoleScreen.windowHeight);
                }
                catch (NotImplementedException)
                {
                    // Setting console size is not implemented on this platform, use the current console size.
                    ConsoleScreen.windowWidth = Console.WindowWidth;
                    ConsoleScreen.windowHeight = Console.WindowHeight;
                }

                // Set the misc. console properties.
                ConsoleScreen.backgroundColor = backgroundColor;
                ConsoleScreen.foregroundColor = foregroundColor;
                ConsoleScreen.displayCursor = displayCursor;

                // Clear the screen after setting all the console properties.
                ClearConsole();
            }
        }

        /// <summary>
        /// Writes a string to the console buffer.
        /// </summary>
        /// <param name="x">The x-coordinate of the screen to write to.</param>
        /// <param name="y">The y-coordinate of the screen to write to.</param>
        /// <param name="text">The text to write to the console.</param>
        /// <remarks>
        /// This writes to the console back buffer, and will not be displayed until
        /// the <see cref="Draw"/> method is invoked.
        /// </remarks>
        public static void Write(int x, int y, string text)
        {
            Write(x, y, text, ConsoleScreen.foregroundColor, ConsoleScreen.backgroundColor);
        }

        /// <summary>
        /// Writes a string to the console buffer.
        /// </summary>
        /// <param name="x">The x-coordinate of the screen to write to.</param>
        /// <param name="y">The y-coordinate of the screen to write to.</param>
        /// <param name="text">The text to write to the console.</param>
        /// <param name="foregroundColor">The font color.</param>
        /// <param name="backgroundColor">The background color.</param>
        /// <remarks>
        /// This writes to the console back buffer, and will not be displayed until
        /// the <see cref="Draw"/> method is invoked.
        /// </remarks>
        public static void Write(int x, int y, string text, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            lock (ConsoleScreen.screenLock)
            {
                // Write each char in the string.
                foreach (char character in text)
                {
                    ConsoleChar consoleChar = new ConsoleChar(character, foregroundColor, backgroundColor);
                    Write(x, y, consoleChar);
                    ++x;
                }
            }
        }

        /// <summary>
        /// Writes a character to the console buffer.
        /// </summary>
        /// <param name="x">The x-coordinate of the screen to write to.</param>
        /// <param name="y">The y-coordinate of the screen to write to.</param>
        /// <param name="character">The character to write to the console.</param>
        /// <remarks>
        /// This writes to the console back buffer, and will not be displayed until
        /// the <see cref="Draw"/> method is invoked.
        /// </remarks>
        public static void Write(int x, int y, char character)
        {
            Write(x, y, character, ConsoleScreen.foregroundColor, ConsoleScreen.backgroundColor);
        }

        /// <summary>
        /// Write a character to the console buffer.
        /// </summary>
        /// <param name="x">The x-coordinate of the screen to write to.</param>
        /// <param name="y">The y-coordinate of the screen to write to.</param>
        /// <param name="character">The character to write to the console.</param>
        /// <param name="foregroundColor">The font color.</param>
        /// <param name="backgroundColor">The background color.</param>
        /// <remarks>
        /// This writes to the console back buffer, and will not be displayed until
        /// the <see cref="Draw"/> method is invoked.
        /// </remarks>
        public static void Write(int x, int y, char character, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            Write(x, y, new ConsoleChar(character, foregroundColor, backgroundColor));
        }

        /// <summary>
        /// Write a character to the console buffer.
        /// </summary>
        /// <param name="x">The x-coordinate of the screen to write to.</param>
        /// <param name="y">The y-coordinate of the screen to write to.</param>
        /// <param name="consoleChar">The console character to write.</param>
        /// <remarks>
        /// This writes to the console back buffer, and will not be displayed until
        /// the <see cref="Draw"/> method is invoked.
        /// </remarks>
        public static void Write(int x, int y, ConsoleChar consoleChar)
        {
            if (consoleChar == null)
            {
                throw new ArgumentNullException(nameof(consoleChar));
            }

            if (x < 0)
            {
                throw new ArgumentException("Coordinates cannot be negative.", nameof(x));
            }

            if (y < 0)
            {
                throw new ArgumentException("Coordinates cannot be negative.", nameof(y));
            }

            if (x >= ConsoleScreen.bufferWidth)
            {
                throw new ArgumentException("Coordinates cannot exceed the console buffer.", nameof(x));
            }

            if (y >= ConsoleScreen.bufferHeight)
            {
                throw new ArgumentException("Coordinates cannot exceed the console buffer.", nameof(y));
            }

            lock (ConsoleScreen.screenLock)
            {
                // Set the character in the back buffer.
                ConsoleScreen.backBuffer[x, y] = consoleChar;
            }
        }

        /// <summary>
        /// Draw the buffer to console screen.
        /// After the buffer is printed to screen it is cleared for the next series of writes.
        /// </summary>
        public static void Draw()
        {
            lock (ConsoleScreen.screenLock)
            {
                // Handle if the window has been resized.
                HandleWindowResize();

                // Print out each row.
                int height = Math.Min(ConsoleScreen.bufferHeight, ConsoleScreen.windowHeight);
                for (int y = 0; y < height; ++y)
                {
                    // Print out each character in the row.
                    int width = Math.Min(ConsoleScreen.bufferWidth, ConsoleScreen.windowWidth);
                    for (int x = 0; x < width; ++x)
                    {
                        // Get the char from the back buffer to write and the current char on screen.
                        ConsoleChar bufferChar = ConsoleScreen.backBuffer[x, y];
                        ConsoleChar currentChar = ConsoleScreen.screenBuffer[x, y];

                        // If the buffer char and the current char are both null, move on (no write required).
                        // If the buffer char is null and the current char is not, write a space to clear it from screen.
                        if (bufferChar == null)
                        {
                            if (currentChar == null)
                            {
                                continue;
                            }

                            bufferChar = new ConsoleChar(' ', ConsoleScreen.backgroundColor, ConsoleScreen.foregroundColor);
                        }

                        // If the buffer character is different than what is currently displayed,
                        // print the new char to screen.
                        if (bufferChar != currentChar)
                        {
                            PrintConsoleChar(x, y, bufferChar);
                        }
                    }
                }

                // Set the back buffer to the current screen buffer.
                // Clear the back buffer for the next set of writes.
                ConsoleScreen.screenBuffer = ConsoleScreen.backBuffer;
                ConsoleScreen.backBuffer = new ConsoleChar[ConsoleScreen.bufferWidth, ConsoleScreen.bufferHeight];
            }
        }

        /// <summary>
        /// Redraws the entire buffer to the console screen.
        /// </summary>
        /// <remarks>This could cause flickering.</remarks>
        public static void Redraw()
        {
            lock (ConsoleScreen.screenLock)
            {
                // Ensure the window size is up to date.
                ConsoleScreen.windowWidth = Console.WindowWidth;
                ConsoleScreen.windowHeight = Console.WindowHeight;

                // Clear the console.
                ClearConsole();

                // Re-print each row.
                int height = Math.Min(ConsoleScreen.bufferHeight, ConsoleScreen.windowHeight);
                for (int y = 0; y < height; ++y)
                {
                    // Re-print each character in the row.
                    int width = Math.Min(ConsoleScreen.bufferWidth, ConsoleScreen.windowWidth);
                    for (int x = 0; x < width; ++x)
                    {
                        ConsoleChar consoleChar = ConsoleScreen.screenBuffer[x, y];
                        if (consoleChar != null)
                        {
                            PrintConsoleChar(x, y, consoleChar);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Clears the console screen and buffer.
        /// </summary>
        public static void Clear()
        {
            lock (ConsoleScreen.screenLock)
            {
                // Clear the buffers and the console.
                ConsoleScreen.screenBuffer = new ConsoleChar[ConsoleScreen.bufferWidth, ConsoleScreen.bufferHeight];
                ConsoleScreen.backBuffer = new ConsoleChar[ConsoleScreen.bufferWidth, ConsoleScreen.bufferHeight];
                ClearConsole();
            }
        }

        /// <summary>
        /// Clears the console window.
        /// </summary>
        /// <remarks>This does not clear the buffer.</remarks>
        private static void ClearConsole()
        {
            Console.CursorVisible = ConsoleScreen.displayCursor;
            Console.BackgroundColor = ConsoleScreen.backgroundColor;
            Console.ForegroundColor = ConsoleScreen.foregroundColor;
            Console.Clear();
        }

        /// <summary>
        /// Handles if the console window is resized.
        /// </summary>
        private static void HandleWindowResize()
        {
            // Redraw the screen if the size changed to avoid artifacts.
            if (ConsoleScreen.windowWidth != Console.WindowWidth
                || ConsoleScreen.windowHeight != Console.WindowHeight)
            {
                Redraw();
            }
        }

        /// <summary>
        /// Prints a character to the console.
        /// </summary>
        /// <param name="x">The x-coordinate of the screen to write to.</param>
        /// <param name="y">The y-coordinate of the screen to write to.</param>
        /// <param name="consoleChar">The console character to write.</param>
        private static void PrintConsoleChar(int x, int y, ConsoleChar consoleChar)
        {
            Console.SetCursorPosition(x, y);
            Console.BackgroundColor = consoleChar.BackgroundColor;
            Console.ForegroundColor = consoleChar.ForegroundColor;
            Console.Write(consoleChar.PrintChar);
        }
    }
}