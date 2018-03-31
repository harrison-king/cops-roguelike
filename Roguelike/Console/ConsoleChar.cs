// <copyright file="ConsoleChar.cs" company="Jesse King">
// Copyright (c) Jesse King. All rights reserved.
// </copyright>

namespace Roguelike.Common.Console
{
    using System;

    /// <summary>
    /// Represents a character in a console (character, font color, and background color).
    /// </summary>
    public class ConsoleChar : IEquatable<ConsoleChar>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleChar"/> class.
        /// </summary>
        /// <param name="printChar">The character to print.</param>
        /// <param name="foregroundColor">The font color.</param>
        /// <param name="backgroundColor">The background color.</param>
        public ConsoleChar(char printChar, ConsoleColor foregroundColor = ConsoleColor.White, ConsoleColor backgroundColor = ConsoleColor.Black)
        {
            this.PrintChar = printChar;
            this.BackgroundColor = backgroundColor;
            this.ForegroundColor = foregroundColor;
        }

        /// <summary>
        /// Gets the character printed in the console.
        /// </summary>
        public char PrintChar
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the foreground (font) color of the character printed in the console.
        /// </summary>
        public ConsoleColor ForegroundColor
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the background color of the character printed in the console.
        /// </summary>
        public ConsoleColor BackgroundColor
        {
            get;
            private set;
        }

        /// <summary>
        /// Implements the == operator.
        /// </summary>
        /// <param name="lhs">The left-hand-object to compare.</param>
        /// <param name="rhs">The right-hand-object to compare.</param>
        /// <returns>
        /// True if <paramref name="lhs"/> and <paramref name="rhs"/> are equal, false otherwise.
        /// </returns>
        public static bool operator ==(ConsoleChar lhs, ConsoleChar rhs)
        {
            if (object.ReferenceEquals(lhs, null))
            {
                return object.ReferenceEquals(rhs, null);
            }

            return lhs.Equals(rhs);
        }

        /// <summary>
        /// Implements the != operator.
        /// </summary>
        /// <param name="lhs">The left-hand-object to compare.</param>
        /// <param name="rhs">The right-hand-object to compare.</param>
        /// <returns>
        /// True if <paramref name="lhs"/> and <paramref name="rhs"/> are not equal, false otherwise.
        /// </returns>
        public static bool operator !=(ConsoleChar lhs, ConsoleChar rhs)
        {
            if (object.ReferenceEquals(lhs, null))
            {
                return !object.ReferenceEquals(rhs, null);
            }

            return !lhs.Equals(rhs);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>True if the current object is equal to the <paramref name="obj"/> parameter, false otherwise.</returns>
        public override bool Equals(object obj)
        {
            return this.Equals(obj as ConsoleChar);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another <see cref="ConsoleChar"/> class.
        /// </summary>
        /// <param name="consoleChar">A <see cref="ConsoleChar"/> to compare with this instance.</param>
        /// <returns>True if the current object is equal to the <paramref name="consoleChar"/> parameter, false otherwise.</returns>
        public bool Equals(ConsoleChar consoleChar)
        {
            if (consoleChar == null)
            {
                return false;
            }

            return this.PrintChar == consoleChar.PrintChar
                && this.ForegroundColor == consoleChar.ForegroundColor
                && this.BackgroundColor == consoleChar.BackgroundColor;
        }

        /// <summary>
        /// Gets the hash code for this instance.
        /// </summary>
        /// <returns>
        /// The hash code for this instance.
        /// </returns>
        public override int GetHashCode()
        {
            return this.PrintChar.GetHashCode()
                ^ this.ForegroundColor.GetHashCode()
                ^ this.BackgroundColor.GetHashCode();
        }
    }
}