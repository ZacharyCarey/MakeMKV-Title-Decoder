using FfmpegInterface.FFProbeCore;
using Instances;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Utils;

namespace FfmpegInterface.FFMpegCore
{
    public class FFMpegCommand
    {
        internal List<string> Arguments;
        private readonly string ProgramExe;
        private string ArgumentsText => string.Join(' ', Arguments);

        internal List<Action> PreActions = new();
        internal List<Func<CancellationToken, Task>> DuringActions = new();
        internal List<Action> PostActions = new();

        private static readonly Regex ProgressRegex = new(@"time=(\d\d:\d\d:\d\d.\d\d?)", RegexOptions.Compiled);
        private TimeSpan? _totalTimespan;
        private Action<double>? _onPercentageProgress;
        private Action<TimeSpan>? _onTimeProgress;
        private Action<string>? _onOutput;
        private Action<string>? _onError;

        private event EventHandler<int> CancelEvent = null!;

        internal FFMpegCommand(string programExe)
        {
            ProgramExe = programExe;
            Arguments = new();
        }

        internal FFMpegCommand(string programExe, params string[] args)
        {
            ProgramExe = programExe;
            Arguments = new(args);
        }

        /// <summary>
        /// Register action that will be invoked during the ffmpeg processing, when a progress time is output and parsed and progress percentage is calculated.
        /// Total time is needed to calculate the percentage that has been processed of the full file.
        /// </summary>
        /// <param name="onPercentageProgress">Action to invoke when progress percentage is updated</param>
        /// <param name="totalTimeSpan">The total timespan of the mediafile being processed</param>
        public FFMpegCommand NotifyOnProgress(Action<double> onPercentageProgress, TimeSpan totalTimeSpan)
        {
            _totalTimespan = totalTimeSpan;
            _onPercentageProgress = onPercentageProgress;
            return this;
        }
        /// <summary>
        /// Register action that will be invoked during the ffmpeg processing, when a progress time is output and parsed
        /// </summary>
        /// <param name="onTimeProgress">Action that will be invoked with the parsed timestamp as argument</param>
        public FFMpegCommand NotifyOnProgress(Action<TimeSpan> onTimeProgress)
        {
            _onTimeProgress = onTimeProgress;
            return this;
        }

        /// <summary>
        /// Register action that will be invoked during the ffmpeg processing, when a line is output
        /// </summary>
        /// <param name="onOutput"></param>
        public FFMpegCommand NotifyOnOutput(Action<string> onOutput)
        {
            _onOutput = onOutput;
            return this;
        }
        public FFMpegCommand NotifyOnError(Action<string> onError)
        {
            _onError = onError;
            return this;
        }
        public FFMpegCommand CancellableThrough(out Action cancel, int timeout = 0)
        {
            cancel = () => CancelEvent?.Invoke(this, timeout);
            return this;
        }
        public FFMpegCommand CancellableThrough(CancellationToken token, int timeout = 0)
        {
            token.Register(() => CancelEvent?.Invoke(this, timeout));
            return this;
        }

        public bool ProcessSynchronously(bool throwOnError = true)
        {
            var processArguments = PrepareProcessArguments(out var cancellationTokenSource);

            IProcessResult? processResult = null;
            try
            {
                processResult = Process(processArguments, cancellationTokenSource).ConfigureAwait(false).GetAwaiter().GetResult();
            }
            catch (OperationCanceledException)
            {
                if (throwOnError)
                {
                    throw;
                }
            }

            return HandleCompletion(throwOnError, processResult?.ExitCode ?? -1, this.ArgumentsText, processResult?.ErrorData ?? Array.Empty<string>());
        }

        public async Task<bool> ProcessAsynchronously(bool throwOnError = false)
        {
            var processArguments = PrepareProcessArguments(out var cancellationTokenSource);

            IProcessResult? processResult = null;
            try
            {
                processResult = await Process(processArguments, cancellationTokenSource).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                if (throwOnError)
                {
                    throw;
                }

                return false;
            }

            return HandleCompletion(throwOnError, processResult?.ExitCode ?? -1, this.ArgumentsText, processResult?.ErrorData ?? Array.Empty<string>());
        }

        private async Task<IProcessResult> Process(ProcessArguments processArguments, CancellationTokenSource cancellationTokenSource)
        {
            IProcessResult processResult = null!;

            foreach (var action in PreActions)
            {
                action();
            }

            using var instance = processArguments.Start();
            var cancelled = false;
            void OnCancelEvent(object sender, int timeout)
            {
                cancelled = true;
                instance.SendInput("q");

                if (!cancellationTokenSource.Token.WaitHandle.WaitOne(timeout, true))
                {
                    cancellationTokenSource.Cancel();
                    instance.Kill();
                }
            }

            CancelEvent += OnCancelEvent;

            try
            {
                await Task.WhenAll(instance.WaitForExitAsync().ContinueWith(t =>
                {
                    processResult = t.Result;
                    cancellationTokenSource.Cancel();
                    foreach (var action in PostActions)
                    {
                        action();
                    }
                }), During(cancellationTokenSource.Token)).ConfigureAwait(false);

                if (cancelled)
                {
                    throw new OperationCanceledException("ffmpeg processing was cancelled");
                }

                return processResult;
            }
            finally
            {
                CancelEvent -= OnCancelEvent;
            }
        }

        private async Task During(CancellationToken cancellationToken = default)
        {
            await Task.WhenAll(DuringActions.Select(io => io(cancellationToken))).ConfigureAwait(false);
        }

        private bool HandleCompletion(bool throwOnError, int exitCode, string arguments, IReadOnlyList<string> errorData)
        {
            if (throwOnError && exitCode != 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(arguments);
                foreach(var line in errorData)
                {
                    Console.WriteLine(line);
                }
                Console.ResetColor();
                throw new Exception($"ffmpeg exited with non-zero exit-code ({exitCode} - {string.Join("\n", errorData)})");
            }

            _onPercentageProgress?.Invoke(100.0);
            if (_totalTimespan.HasValue)
            {
                _onTimeProgress?.Invoke(_totalTimespan.Value);
            }

            return exitCode == 0;
        }

        private void ErrorData(object sender, string msg)
        {
            _onError?.Invoke(msg);

            var match = ProgressRegex.Match(msg);
            if (!match.Success)
            {
                return;
            }

            var processed = MediaAnalysisUtils.ParseDuration(match.Groups[1].Value);
            _onTimeProgress?.Invoke(processed);

            if (_onPercentageProgress == null || _totalTimespan == null)
            {
                return;
            }

            var percentage = Math.Round(processed.TotalSeconds / _totalTimespan.Value.TotalSeconds * 100, 2);
            _onPercentageProgress(percentage);
        }

        private void OutputData(object sender, string msg)
        {
            Debug.WriteLine(msg);
            _onOutput?.Invoke(msg);
        }

        private ProcessArguments PrepareProcessArguments(out CancellationTokenSource cancellationTokenSource)
        {
            var arguments = ArgumentsText;
            Debug.WriteLine(arguments);

            //If neither local nor global loglevel is null, set the argument.
            //if (_logLevel != null)
            //{
            //    var normalizedLogLevel = _logLevel.ToString() .ToLower();
            //    arguments += $" -v {normalizedLogLevel}";
            //}

            string? binPath = CommandLineInterface.SearchLocalExeFiles(Path.Combine("lib", ProgramExe));
            if (binPath == null)
            {
                throw new Exception("Failed to locate ffmepg binary.");
            }

            var startInfo = new ProcessStartInfo
            {
                FileName = binPath,
                Arguments = arguments,
                //StandardOutputEncoding = ffOptions.Encoding,
                //StandardErrorEncoding = ffOptions.Encoding,
                //WorkingDirectory = ffOptions.WorkingDirectory
            };
            var processArguments = new ProcessArguments(startInfo);
            cancellationTokenSource = new CancellationTokenSource();

            if (_onOutput != null)
            {
                processArguments.OutputDataReceived += OutputData;
            }

            if (_onError != null || _onTimeProgress != null || _onPercentageProgress != null && _totalTimespan != null)
            {
                processArguments.ErrorDataReceived += ErrorData;
            }

            return processArguments;
        }
    }
}
