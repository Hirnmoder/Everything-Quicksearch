using System;
using System.Diagnostics;
using System.IO.Pipes;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;

namespace Quicksearch.Util
{
    internal static class ApplicationInstanceWatcher
    {
        internal delegate void ShutdownRequestDelegate();
        internal static event ShutdownRequestDelegate ShutdownRequest;

        internal static string GUID { get; }

        private static string PipeName;
        private static PipeStream Pipe;

        private static Thread ServerWaitThread;
        private static bool Wait;

        private const byte ShutdownSignal = (byte)'S';

        static ApplicationInstanceWatcher()
        {
            GUID = typeof(ApplicationInstanceWatcher).GUID.ToString("N");
            PipeName = $"QuicksearchPipe{GUID}";
        }

        internal static bool CreatePipe()
        {
            // Destroy old Pipes
            Exit();

            try
            {
                PipeSecurity ps = new PipeSecurity();
                ps.SetAccessRule(new PipeAccessRule(WindowsIdentity.GetCurrent().User, PipeAccessRights.ReadWrite, AccessControlType.Allow));

                Pipe = new NamedPipeServerStream(PipeName,
                                                 PipeDirection.InOut,
                                                 1,
                                                 PipeTransmissionMode.Message,
                                                 PipeOptions.None,
                                                 0,
                                                 0,
                                                 ps);


                Wait = true;
                ServerWaitThread = new Thread(new ThreadStart(WaitForShutdownSignal));
                ServerWaitThread.IsBackground = true;
                ServerWaitThread.Priority = ThreadPriority.Lowest;
                ServerWaitThread.Start();


                return true;
            }
            catch (UnauthorizedAccessException uae)
            {
                Pipe = new NamedPipeClientStream(".",
                    PipeName,
                    PipeDirection.InOut,
                    PipeOptions.None,
                    TokenImpersonationLevel.None);

                return false;
            }
            catch (Exception ex)
            {
                Debug.Print(ex.ToString());
                return false;
            }
        }

        internal static void CreatePipeAndShutdownOtherInstances()
        {
            var t = new Thread(new ThreadStart(() =>
            {
                while (!CreatePipe())
                {
                    SendShutdownToAllOtherInstances();
                    Thread.Sleep(500);
                }
            }));
            t.IsBackground = true;
            t.Priority = ThreadPriority.Lowest;
            t.Start();
        }

        private static void WaitForShutdownSignal()
        {
            try
            {
                var buffer = new byte[16 * 1024]; // 16 KB
                while (Wait)
                {
                    if (Pipe is NamedPipeServerStream npss)
                    {
                        try
                        {
                            npss.WaitForConnection();
                            if (Pipe.Read(buffer, 0, buffer.Length) > 0)
                            {
                                if(buffer[0] == ShutdownSignal)
                                {
                                    Wait = false;
                                    ShutdownRequest?.Invoke();
                                }
                            }
                            npss.Disconnect();
                        }
                        catch (Exception ex)
                        {
                            Debug.Print(ex.ToString());
                        }
                    }
                }
            }
            catch (ThreadAbortException)
            {

            }
            catch (Exception ex)
            {
                Debug.Print(ex.ToString());
            }
        }

        internal static void SendShutdownToAllOtherInstances()
        {
            try
            {
                if (Pipe is NamedPipeClientStream npcs)
                {
                    npcs.Connect(100);
                    if (npcs.IsConnected)
                    {
                        npcs.WriteByte(ShutdownSignal);
                        npcs.Flush();
                        npcs.WaitForPipeDrain();
                    }
                }
            }
            catch(Exception ex)
            {
                Debug.Print(ex.ToString());
            }
        }

        internal static void Exit()
        {
            try
            {
                Wait = false;
                Pipe?.Dispose();
                Pipe = null;
            }
            catch(Exception ex)
            {
                Debug.Print(ex.ToString());
            }
        }
    }
}
