using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MakeMKV_Title_Decoder {
    internal class ConsoleLog : TextWriter {

        private FileStream fileStream;
        private StreamWriter streamWriter;
        private TextWriter consoleWriter;
        bool disposed = false;

        public override Encoding Encoding => streamWriter.Encoding;
        public override IFormatProvider FormatProvider => streamWriter.FormatProvider;
        public override string NewLine { 
            get => streamWriter.NewLine;
            set
            {
                streamWriter.NewLine = value;
                consoleWriter.NewLine = value;
            }
        }

        public static bool CreateLogger(string filepath) {
            try
            {
                if (File.Exists(filepath))
                {
                    File.Delete(filepath);
                }
                FileStream stream = File.OpenWrite(filepath);
                StreamWriter writer = new StreamWriter(stream);

                new ConsoleLog(stream, writer);
                return true;
            } catch(Exception ex)
            {
                Console.WriteLine("Failed to open log: " + ex.Message);
                return false;
            }
        }

        private ConsoleLog(FileStream stream, StreamWriter writer) {
            this.fileStream = stream;
            this.streamWriter = writer;
            this.consoleWriter = Console.Out;

            // Sync stream properties
            this.streamWriter.NewLine = this.consoleWriter.NewLine;

            Console.SetOut(this);
        }

        ~ConsoleLog() {
            if (!disposed)
            {
                this.streamWriter.Flush();
                this.streamWriter.Dispose();

                this.consoleWriter.Flush();
                this.consoleWriter.Dispose();
                disposed = true;
            }
        }

        public override void Close() {
            this.streamWriter.Close();
            this.consoleWriter.Close();
        }

        protected override void Dispose(bool disposing) {
            if (!disposed)
            {
                this.streamWriter.Dispose();
                this.consoleWriter.Dispose();
                this.disposed = true;
            }
        }

        public override ValueTask DisposeAsync() {
            return new ValueTask(new Task(async () =>
            {
                ValueTask task1 = this.streamWriter.DisposeAsync();
                ValueTask task2 = this.consoleWriter.DisposeAsync();
                await Task.WhenAll(task1.AsTask(), task2.AsTask());
                this.disposed = true;
            }));
        }

        public override void Flush() {
            streamWriter.Flush();
            consoleWriter.Flush();
        }

        public override Task FlushAsync() {
            Task task1 = this.streamWriter.FlushAsync();
            Task task2 = this.consoleWriter.FlushAsync();
            return Task.WhenAll(task1, task2);
        }

        public override Task FlushAsync(CancellationToken cancellationToken) {
            Task task1 = this.streamWriter.FlushAsync(cancellationToken);
            Task task2 = this.consoleWriter.FlushAsync(cancellationToken);
            return Task.WhenAll(task1, task2);
        }

        #region Writes
        public override void Write(char value) {
            this.streamWriter.Write(value);
            this.consoleWriter.Write(value);
        }

        public override void Write(char[]? buffer) {
            this.streamWriter.Write(buffer);
            this.consoleWriter.Write(buffer);
        }

        public override void Write(char[] buffer, int index, int count) {
            this.streamWriter.Write(buffer, index, count);
            this.consoleWriter.Write(buffer, index, count);
        }

        public override void Write(ReadOnlySpan<char> buffer) {
            this.streamWriter.Write(buffer);
            this.consoleWriter.Write(buffer);
        }

        public override void Write(bool value) {
            this.streamWriter.Write(value);
            this.consoleWriter.Write(value);
        }

        public override void Write(int value) {
            this.streamWriter.Write(value);
            this.consoleWriter.Write(value);
        }

        public override void Write(uint value) {
            this.streamWriter.Write(value);
            this.consoleWriter.Write(value);
        }

        public override void Write(long value) {
            this.streamWriter.Write(value);
            this.consoleWriter.Write(value);
        }

        public override void Write(ulong value) {
            this.streamWriter.Write(value);
            this.consoleWriter.Write(value);
        }

        public override void Write(float value) {
            this.streamWriter.Write(value);
            this.consoleWriter.Write(value);
        }

        public override void Write(double value) {
            this.streamWriter.Write(value);
            this.consoleWriter.Write(value);
        }

        public override void Write(decimal value) {
            this.streamWriter.Write(value);
            this.consoleWriter.Write(value);
        }

        public override void Write(string? value) {
            this.streamWriter.Write(value);
            this.consoleWriter.Write(value);
        }

        public override void Write(object? value) {
            this.streamWriter.Write(value);
            this.consoleWriter.Write(value);
        }

        public override void Write(StringBuilder? value) {
            this.streamWriter.Write(value);
            this.consoleWriter.Write(value);
        }

        public override void Write([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, object? arg0) {
            this.streamWriter.Write(format, arg0);
            this.consoleWriter.Write(format, arg0);
        }

        public override void Write([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, object? arg0, object? arg1) {
            this.streamWriter.Write(format, arg0, arg1);
            this.consoleWriter.Write(format, arg0, arg1);
        }

        public override void Write([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, object? arg0, object? arg1, object? arg2) {
            this.streamWriter.Write(format, arg0, arg1, arg2);
            this.consoleWriter.Write(format, arg0, arg1, arg2);
        }

        public override void Write([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, params object?[] arg) {
            this.streamWriter.Write(format, arg);
            this.consoleWriter.Write(format, arg);
        }

        public override void WriteLine() {
            this.streamWriter.WriteLine();
            this.consoleWriter.WriteLine();
        }

        public override void WriteLine(char value) {
            this.streamWriter.WriteLine(value);
            this.consoleWriter.WriteLine(value);
        }

        public override void WriteLine(char[]? buffer) {
            this.streamWriter.WriteLine(buffer);
            this.consoleWriter.WriteLine(buffer);
        }

        public override void WriteLine(char[] buffer, int index, int count) {
            this.streamWriter.WriteLine(buffer, index, count);
            this.consoleWriter.WriteLine(buffer, index, count);
        }

        public override void WriteLine(ReadOnlySpan<char> buffer) {
            this.streamWriter.WriteLine(buffer);
            this.consoleWriter.WriteLine(buffer);
        }

        public override void WriteLine(bool value) {
            this.streamWriter.WriteLine(value);
            this.consoleWriter.WriteLine(value);
        }

        public override void WriteLine(int value) {
            this.streamWriter.WriteLine(value);
            this.consoleWriter.WriteLine(value);
        }

        public override void WriteLine(uint value) {
            this.streamWriter.WriteLine(value);
            this.consoleWriter.WriteLine(value);
        }

        public override void WriteLine(long value) {
            this.streamWriter.WriteLine(value);
            this.consoleWriter.WriteLine(value);
        }

        public override void WriteLine(ulong value) {
            this.streamWriter.WriteLine(value);
            this.consoleWriter.WriteLine(value);
        }

        public override void WriteLine(float value) {
            this.streamWriter.WriteLine(value);
            this.consoleWriter.WriteLine(value);
        }

        public override void WriteLine(double value) {
            this.streamWriter.WriteLine(value);
            this.consoleWriter.WriteLine(value);
        }

        public override void WriteLine(decimal value) {
            this.streamWriter.WriteLine(value);
            this.consoleWriter.WriteLine(value);
        }

        public override void WriteLine(string? value) {
            this.streamWriter.WriteLine(value);
            this.consoleWriter.WriteLine(value);
        }

        public override void WriteLine(StringBuilder? value) {
            this.streamWriter.WriteLine(value);
            this.consoleWriter.WriteLine(value);
        }

        public override void WriteLine(object? value) {
            this.streamWriter.WriteLine(value);
            this.consoleWriter.WriteLine(value);
        }

        public override void WriteLine([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, object? arg0) {
            this.streamWriter.WriteLine(format, arg0);
            this.consoleWriter.WriteLine(format, arg0);
        }

        public override void WriteLine([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, object? arg0, object? arg1) {
            this.streamWriter.WriteLine(format, arg0, arg1);
            this.consoleWriter.WriteLine(format, arg0, arg1);
        }

        public override void WriteLine([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, object? arg0, object? arg1, object? arg2) {
            this.streamWriter.WriteLine(format, arg0, arg1, arg2);
            this.consoleWriter.WriteLine(format, arg0, arg1, arg2);
        }

        public override void WriteLine([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, params object?[] arg) {
            this.streamWriter.WriteLine(format, arg);
            this.consoleWriter.WriteLine(format, arg);
        }
        #endregion

        #region Task based Async APIs
        public override Task WriteAsync(char value) {
            Task task1 = this.streamWriter.WriteAsync(value);
            Task task2 = this.consoleWriter.WriteAsync(value);
            return Task.WhenAll(task1, task2);
        }

        public override Task WriteAsync(string? value) {
            Task task1 = this.streamWriter.WriteAsync(value);
            Task task2 = this.consoleWriter.WriteAsync(value);
            return Task.WhenAll(task1, task2);
        }

        public override Task WriteAsync(StringBuilder? value, CancellationToken cancellationToken = default) {
            Task task1 = this.streamWriter.WriteAsync(value, cancellationToken);
            Task task2 = this.consoleWriter.WriteAsync(value, cancellationToken);
            return Task.WhenAll(task1, task2);
        }

        public Task WriteAsync(char[]? buffer) {
            Task task1 = this.streamWriter.WriteAsync(buffer);
            Task task2 = this.consoleWriter.WriteAsync(buffer);
            return Task.WhenAll(task1, task2);
        }

        public override Task WriteAsync(char[] buffer, int index, int count) {
            Task task1 = this.streamWriter.WriteAsync(buffer, index, count);
            Task task2 = this.consoleWriter.WriteAsync(buffer, index, count);
            return Task.WhenAll(task1, task2);
        }

        public override Task WriteAsync(ReadOnlyMemory<char> buffer, CancellationToken cancellationToken = default) {
            Task task1 = this.streamWriter.WriteAsync(buffer, cancellationToken);
            Task task2 = this.consoleWriter.WriteAsync(buffer, cancellationToken);
            return Task.WhenAll(task1, task2);
        }

        public override Task WriteLineAsync(char value) {
            Task task1 = this.streamWriter.WriteLineAsync(value);
            Task task2 = this.consoleWriter.WriteLineAsync(value);
            return Task.WhenAll(task1, task2);
        }

        public override Task WriteLineAsync(string? value) {
            Task task1 = this.streamWriter.WriteLineAsync(value);
            Task task2 = this.consoleWriter.WriteLineAsync(value);
            return Task.WhenAll(task1, task2);
        }

        public override Task WriteLineAsync(StringBuilder? value, CancellationToken cancellationToken = default) {
            Task task1 = this.streamWriter.WriteLineAsync(value, cancellationToken);
            Task task2 = this.consoleWriter.WriteLineAsync(value, cancellationToken);
            return Task.WhenAll(task1, task2);
        }

        public Task WriteLineAsync(char[]? buffer) {
            Task task1 = this.streamWriter.WriteLineAsync(buffer);
            Task task2 = this.consoleWriter.WriteLineAsync(buffer);
            return Task.WhenAll(task1, task2);
        }

        public override Task WriteLineAsync(char[] buffer, int index, int count) {
            Task task1 = this.streamWriter.WriteLineAsync(buffer, index, count);
            Task task2 = this.consoleWriter.WriteLineAsync(buffer, index, count);
            return Task.WhenAll(task1, task2);
        }

        public override Task WriteLineAsync(ReadOnlyMemory<char> buffer, CancellationToken cancellationToken = default) {
            Task task1 = this.streamWriter.WriteLineAsync(buffer, cancellationToken);
            Task task2 = this.consoleWriter.WriteLineAsync(buffer, cancellationToken);
            return Task.WhenAll(task1, task2);
        }

        public override Task WriteLineAsync() {
            Task task1 = this.streamWriter.WriteLineAsync();
            Task task2 = this.consoleWriter.WriteLineAsync();
            return Task.WhenAll(task1, task2);
        }
        #endregion
    }
}
