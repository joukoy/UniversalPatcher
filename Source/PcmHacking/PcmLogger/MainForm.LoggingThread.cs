using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace PcmHacking
{
    partial class MainForm
    {
        private ConcurrentQueue<Tuple<Logger, LogFileWriter, IEnumerable<string>>> logRowQueue = new ConcurrentQueue<Tuple<Logger, LogFileWriter, IEnumerable<string>>>();

        private AutoResetEvent endWriterThread = new AutoResetEvent(false);
        private AutoResetEvent rowAvailable = new AutoResetEvent(false);

        private static DateTime lastLogTime;

        private ManualResetEvent loggerThreadEnded = new ManualResetEvent(false);
        private ManualResetEvent writerThreadEnded = new ManualResetEvent(false);
        
        enum LogState
        {
            Nothing,
            DisplayOnly,
            StartSaving,
            Saving,
            StopSaving
        }

        private LogState logState = LogState.Nothing;

        /// <summary>
        /// Create a string that will look reasonable in the UI's main text box.
        /// TODO: Use a grid instead.
        /// </summary>
        private string FormatValuesForTextBox(Logger logger, IEnumerable<string> rowValues)
        {
            StringBuilder builder = new StringBuilder();
            IEnumerator<string> rowValueEnumerator = rowValues.GetEnumerator();
            foreach (ParameterGroup group in logger.DpidConfiguration.ParameterGroups)
            {
                foreach (LogColumn column in group.LogColumns)
                {
                    rowValueEnumerator.MoveNext();
                    builder.Append(rowValueEnumerator.Current);
                    builder.Append('\t');
                    builder.Append(column.Conversion.Units);
                    builder.Append('\t');
                    builder.AppendLine(column.Parameter.Name);
                }
            }

            foreach (LogColumn mathColumn in logger.MathValueProcessor.GetMathColumns())
            {
                rowValueEnumerator.MoveNext();
                builder.Append(rowValueEnumerator.Current);
                builder.Append('\t');
                builder.Append(mathColumn.Conversion.Units);
                builder.Append('\t');
                builder.AppendLine(mathColumn.Parameter.Name);
            }

            DateTime now = DateTime.Now;
            builder.AppendLine((now - lastLogTime).TotalMilliseconds.ToString("0.00") + "\tms\tQuery time");
            lastLogTime = now;

            return builder.ToString();
        }

        private Logger RecreateLogger()
        {            
            Logger logger = Logger.Create(this.Vehicle, this.osid, this.currentProfile.Columns);

            if (!logger.StartLogging())
            {
                this.loggerProgress.Invoke(
                    (MethodInvoker)
                    delegate ()
                    {
                        this.logValues.Text = "Unable to start logging.";
                    });

                Thread.Sleep(200);

                // Force a retry.
                return null;
            }

            return logger;
        }

        private Tuple<LogFileWriter,StreamWriter> StartSaving(Logger logger)
        {
            string logFilePath = GenerateLogFilePath();
            StreamWriter streamWriter = new StreamWriter(logFilePath);
            LogFileWriter logFileWriter = new LogFileWriter(streamWriter);

            IEnumerable<string> columnNames = logger.GetColumnNames();
            logFileWriter.WriteHeader(columnNames);

            return new Tuple<LogFileWriter, StreamWriter>(logFileWriter, streamWriter);
        }

        private void StopSaving(ref StreamWriter streamWriter)
        {
            if (streamWriter != null)
            {
                streamWriter.Dispose();
                streamWriter = null;
            }
        }

        private void ProcessRow(Logger logger, LogFileWriter logFileWriter)
        {
            var rowVal = logger.GetNextRow();
            if (rowVal != null)
            {
                IEnumerable<string> rowValues = rowVal;
                // Hand this data off to be written to disk and displayed in the UI.
                this.logRowQueue.Enqueue(
                    new Tuple<Logger, LogFileWriter, IEnumerable<string>>(
                        logger,
                        logFileWriter,
                        rowValues));

                this.rowAvailable.Set();
            }
        }

        /// <summary>
        /// The loop that reads data from the PCM.
        /// </summary>
        private void LoggingThread(object threadContext)
        {
            using (AwayMode lockScreenSuppressor = new AwayMode())
            {
                try
                {
                    // Start the write/display thread.
                    ThreadPool.QueueUserWorkItem(LogFileWriterThread, null);

#if Vpw4x
                    if (!this.Vehicle.VehicleSetVPW4x(VpwSpeed.FourX))
                    {
                        this.AddUserMessage("Unable to switch to 4x.");
                        return;
                    }
#endif

                    StreamWriter streamWriter = null;
                    try
                    {
                        LogProfile lastProfile = null;
                        Logger logger = null;
                        LogFileWriter logFileWriter = null;

                        while (!this.logStopRequested)
                        {
                            // Re-create the logger with an updated profile if necessary.
                            if (this.currentProfile != lastProfile)
                            {
                                this.StopSaving(ref streamWriter);

                                if ((this.currentProfile == null) || this.currentProfile.IsEmpty)
                                {
                                    this.logState = LogState.Nothing;
                                    lastProfile = this.currentProfile;
                                    logger = null;
                                }
                                else
                                {
                                    logger = this.RecreateLogger();
                                    if (logger != null)
                                    {
                                        lastProfile = this.currentProfile;

                                        // If this was the first profile to load...
                                        if (this.logState == LogState.Nothing)
                                        {
                                            this.logState = LogState.DisplayOnly;
                                        }
                                    }

                                    switch (logState)
                                    {
                                        case LogState.Nothing:
                                        case LogState.DisplayOnly:
                                        case LogState.StopSaving:
                                            break;

                                        default:
                                            var tuple = this.StartSaving(logger);
                                            logFileWriter = tuple.Item1;
                                            streamWriter = tuple.Item2;
                                            logState = LogState.Saving;
                                            break;
                                    }
                                }

                                this.Invoke(
                                    (MethodInvoker)
                                    delegate ()
                                    {
                                        this.startStopSaving.Enabled = logger != null;
                                    });

                            }

                            switch (logState)
                            {
                                case LogState.Nothing:
                                    this.loggerProgress.Invoke(
                                        (MethodInvoker)
                                        delegate ()
                                        {
                                            this.logValues.Text = "Please select some parameters, or open a log profile.";
                                        });

                                    Thread.Sleep(200);
                                    break;

                                case LogState.DisplayOnly:
                                    this.ProcessRow(logger, null);
                                    break;

                                case LogState.StartSaving:
                                    var tuple = this.StartSaving(logger);
                                    logFileWriter = tuple.Item1;
                                    streamWriter = tuple.Item2;
                                    logState = LogState.Saving;
                                    break;

                                case LogState.Saving:
                                    this.ProcessRow(logger, logFileWriter);
                                    break;

                                case LogState.StopSaving:
                                    this.StopSaving(ref streamWriter);
                                    this.logState = LogState.DisplayOnly;
                                    break;
                            }
                        }
                    }
                    finally
                    {
                        if (streamWriter != null)
                        {
                            streamWriter.Dispose();
                            streamWriter = null;
                        }

                        endWriterThread.Set();
                    }
                }
                catch (Exception exception)
                {
                    if (!logStopRequested)
                    {
                        var st = new StackTrace(exception, true);
                        // Get the top stack frame
                        var frame = st.GetFrame(st.FrameCount - 1);
                        // Get the line number from the stack frame
                        var line = frame.GetFileLineNumber();
                        Debug.WriteLine("Error, LoggingThread line " + line + ": " + exception.Message, "Error");

                        this.AddUserMessage("Logging halted. " + exception.Message);
                        this.AddDebugMessage(exception.ToString());
                        this.logValues.Invoke(
                            (MethodInvoker)
                            delegate ()
                            {
                                this.logValues.Text = "Logging halted. " + exception.Message;
                                this.startStopSaving.Focus();
                            });
                    }
                }
                finally
                {
                    this.loggerThreadEnded.Set();
#if Vpw4x
                    if (!this.Vehicle.VehicleSetVPW4x(VpwSpeed.Standard))
                    {
                        // Try twice...
                        this.Vehicle.VehicleSetVPW4x(VpwSpeed.Standard);
                    }
#endif
                }
            }
        }

        /// <summary>
        /// Background thread to write to disk and send updates to the UI.
        /// This minimizes the amount code that executes between requests for new rows of log data.
        /// </summary>
        private void LogFileWriterThread(object threadContext)
        {
            WaitHandle[] writerHandles = new WaitHandle[] { endWriterThread, rowAvailable };

            try
            {
                while (!logStopRequested)
                {
                    int index = WaitHandle.WaitAny(writerHandles);
                    if (index == 0)
                    {
                        this.BeginInvoke((MethodInvoker)
                        delegate ()
                        {
                            this.logValues.Text = "Logging halted.";
                        });

                        return;
                    }

                    Tuple<Logger, LogFileWriter, IEnumerable<string>> row;
                    if (logRowQueue.TryDequeue(out row))
                    {
                        if (row.Item2 != null)
                        {
                            row.Item2.WriteLine(row.Item3);
                        }

                        string formattedValues = FormatValuesForTextBox(row.Item1, row.Item3);

                        this.BeginInvoke((MethodInvoker)
                        delegate ()
                        {
                            this.logValues.Text = formattedValues;
                        });
                    }
                }
            }
            catch (Exception exception)
            {
                if (!logStopRequested)
                {
                    this.AddUserMessage("Log writing halted. " + exception.Message);
                    this.AddDebugMessage(exception.ToString());
/*
                    this.logValues.Invoke(
                            (MethodInvoker)
                            delegate ()
                            {
                                this.logValues.Text = "Log writing halted. " + exception.Message;
                                this.startStopSaving.Focus();
                            });
*/
                }
            }
            finally
            {
                this.writerThreadEnded.Set();
            }
        }
    }
}
