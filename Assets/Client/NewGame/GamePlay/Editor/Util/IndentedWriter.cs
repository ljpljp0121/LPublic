using System;
using System.IO;

#if UNITY_EDITOR
namespace Game.Editor
{
    public sealed class IndentedWriter : IDisposable
    {
        private StreamWriter streamWriter;

        public IndentedWriter(StreamWriter streamWriter, int indent = 0, int spaces = 4)
        {
            this.streamWriter = streamWriter;
            Indent = indent;
            Spaces = spaces;
        }

        public int Spaces { get; set; }

        public int Indent { get; set; }

        public string GetIndentation()
        {
            return new string(' ', Math.Max(0, Indent * Spaces));
        }

        public void Write(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                streamWriter.Write(text);
            else
                streamWriter.Write(GetIndentation() + text);
        }

        public void WriteLine(string line = null)
        {
            if (string.IsNullOrWhiteSpace(line))
                streamWriter.WriteLine();
            else
                streamWriter.WriteLine(GetIndentation() + line);
        }

        private bool isDisposed;

        public void Dispose()
        {
            if (!isDisposed)
            {
                isDisposed = true;

                streamWriter.Dispose();
                streamWriter = null;
            }

            GC.SuppressFinalize(this);
        }
    }
}
#endif