using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Otp;
using Erlang = Otp.Erlang;

namespace ServantTray
{
    internal class OTP_worker
    {
        static bool logReadWrite = false;

        private string remote;
        private string cookie;
        private string nodename;
        public bool Working { get; private set; }
        public bool Stopping { get; private set; }
        private Queue<OTPWorkerCommand> _commands = new Queue<OTPWorkerCommand>();
        private readonly Object _commands_lock = new Object();

        public OTP_worker(string target, string _cookie)
        {
            cookie = _cookie ?? OtpNode.defaultCookie;
            string host = System.Net.Dns.GetHostName();
            remote = (target.IndexOf('@') < 0) ? target + "@" + host : target;
            nodename = Environment.UserName + "_servant_tray@" + host;
            AbstractConnection.traceLevel = OtpTrace.Type.defaultLevel;
            Working = false;
            new Task(Process).Start();
        }

        public void Stop()
        {
            lock (_commands_lock)
            {
                _commands.Enqueue(new OTPWorkerCommand { type = OTPWorkerCommandType.STOP });
            }
            while (Working)
                Thread.Sleep(100);
        }

        public void GetList(Action<IEnumerable<Tuple<String, object>>> handler)
        {
            lock (_commands_lock)
            {
                _commands.Enqueue(new OTPWorkerCommand { type = OTPWorkerCommandType.GETLIST, param = handler });
            }
        }

        public void MenuItemClicked(object code)
        {
            lock (_commands_lock)
            {
                _commands.Enqueue(new OTPWorkerCommand { type = OTPWorkerCommandType.CLICKED, param = code });
            }
        }

        private void Process()
        {
            try
            {
                Working = true;
                ProcessInternal();
                EndWork(true);
            }
            catch (Exception ex)
            {
                WriteLine(ex.Message);
                EndWork(false);
            }
        }

        private void ProcessInternal()
        {
            OtpNode node = new OtpNode(false, nodename, cookie, true);
            WriteLine("This node is called {0} and is using cookie='{1}'.",
                node.node(), node.cookie());

            OtpCookedConnection.ConnectTimeout = 2000;
            OtpCookedConnection conn = node.connection(remote);

            if (conn == null)
                throw new Exception("Can't connect to node " + remote);

            conn.OnReadWrite += OnReadWrite;

            // If using short names or IP address as the host part of the node name,
            // get the short name of the peer.
            remote = conn.peer.node();

            WriteLine("   successfully connected to node " + remote + "\n");

            OtpMbox mbox = null;

            try
            {
                mbox = node.createMbox();

                while (!Stopping)
                {
                    if (_commands.Count == 0)
                    {
                        Thread.Sleep(100);
                        continue;
                    }

                    OTPWorkerCommand cmd = new OTPWorkerCommand { type = OTPWorkerCommandType.NO_COMMAND, param = null };
                    lock (_commands_lock)
                    {
                        if (_commands.Count < 0)
                            continue;
                        cmd = _commands.Dequeue();
                    }
                    switch (cmd.type)
                    {
                        case OTPWorkerCommandType.STOP:
                            Stopping = true;
                            break;
                        case OTPWorkerCommandType.GETLIST:
                            GetList(mbox, (Action<IEnumerable<Tuple<String, object>>>)cmd.param);
                            break;
                        case OTPWorkerCommandType.CLICKED:
                            SendDo(mbox, (Erlang.Object)cmd.param);
                            break;
                        default:
                            WriteLine("Unknown command: {0} {1}", cmd.type, cmd.param);
                            break;
                    }
                }

            }
            finally
            {
                node.closeMbox(mbox);
            }
            node.close();
        }

        private void GetList(OtpMbox mbox, Action<IEnumerable<Tuple<String, object>>> handler)
        {
            Otp.Erlang.Object reply = mbox.rpcCall(
                remote, "servant_tasklist", "getForMenu",
                new Otp.Erlang.List());

            Erlang.Object okPat = Erlang.Object.Format("{ok, List}");
            Erlang.Object itemPat = Erlang.Object.Format("{Text,Code}");
            Erlang.VarBind binding;
            if (okPat.match(reply, (binding = new Otp.Erlang.VarBind())))
            {
                Erlang.List list = binding.find("List") as Erlang.List;
                handler(ListToMenuItems(list));
            }
            else
            {
                WriteLine("Bad reply on getList {0}", reply.ToString());
            }
        }

        private IEnumerable<Tuple<String, object>> ListToMenuItems(Erlang.List list)
        {
            Erlang.Object itemPat = Erlang.Object.Format("{Text,Code}");
            Erlang.VarBind binding;
            foreach (Erlang.Object item in list)
            {
                if (itemPat.match(item, (binding = new Otp.Erlang.VarBind())))
                {
                    Erlang.Object text = binding.find("Text");
                    string textStr = text.stringValue();
                    Erlang.Object code = binding.find("Code");
                    yield return new Tuple<String, object>(textStr, code);
                }
            }
        }

        private void SendDo(OtpMbox mbox, Erlang.Object code)
        {
            Otp.Erlang.Object reply = mbox.rpcCall(
                remote, "servant_tasklist", "doFromMenu",
                new Otp.Erlang.List(code));
            WriteLine("Do reply: {0}", reply);
        }

        private void EndWork(bool ok)
        {
            Working = false;
        }

        private static void WriteLine(string format, params object[] args)
        {
            string str = String.Format(format, args);
            System.Diagnostics.Trace.WriteLine(str);
        }

        private static void OnReadWrite(AbstractConnection con, AbstractConnection.Operation op,
            long lastBytes, long totalBytes, long totalMsgs)
        {
            if (!logReadWrite)
                return;
            WriteLine("{0} {1} bytes (total: {2} bytes, {3} msgs)",
                (op == AbstractConnection.Operation.Read ? "Read " : "Written "),
                lastBytes, totalBytes, totalMsgs);
        }
    }

    internal enum OTPWorkerCommandType
    {
        NO_COMMAND = 0,
        STOP,
        GETLIST,
        CLICKED,
    }

    internal struct OTPWorkerCommand
    {
        public OTPWorkerCommandType type;
        public Object param;
    }
}
