using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public OTP_worker(string target)
        {
            cookie = OtpNode.defaultCookie;
            string host = System.Net.Dns.GetHostName();
            remote = (target.IndexOf('@') < 0) ? target + "@" + host : target;
            nodename = Environment.UserName + "123@" + host;
            AbstractConnection.traceLevel = OtpTrace.Type.defaultLevel;
            Working = false;
            new Task(Process).Start();
        }

        public void Stop()
        {
            Stopping = true;
        }

        private void Process()
        {
            try
            {
                Working = true;
                ProcessInternal();
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

                //Registering on target node for global name "servantTray"
                {
                    Otp.Erlang.Object reply = mbox.rpcCall(
                        remote, "global", "register_name",
                        new Otp.Erlang.List(new Otp.Erlang.Atom("servantTray"), mbox.self()));

                    //WriteLine("<= [REPLY2]:" + (reply == null ? "null" : reply.ToString()));
                    if (reply.ToString() != "yes")
                        throw new Exception("Can't register name 'servantTray'");
                }

                {
                    Otp.Erlang.Object reply = mbox.rpcCall(
                        remote, "io", "format",
                        new Otp.Erlang.List(
@"servantTray process connected: ~w -> ~w
Echo message: global:send(servantTray, {self(),message}).
Stop servantTray: global:send(servantTray, stop).
",
                            new Otp.Erlang.List(mbox.self(), new Otp.Erlang.Atom("ok"))
                        ));

                    //WriteLine("<= [REPLY3]:" + (reply == null ? "null" : reply.ToString()));
                }

                Erlang.Object tupplePat = Erlang.Object.Format("{Pid, Mes}");
                Erlang.Object stopPat = Erlang.Object.Format("stop");
                Erlang.VarBind binding;
                while (!Stopping)
                {
                    Otp.Erlang.Object msg = mbox.receive();
                    if (tupplePat.match(msg, (binding = new Otp.Erlang.VarBind())))
                    {
                        var Pid = binding.find("Pid") as Erlang.Pid;
                        if (Pid != null)
                        {
                            mbox.send(Pid, binding.find("Mes"));
                        }
                    }
                    else if (stopPat.match(msg, (binding = new Otp.Erlang.VarBind())))
                    {
                        Stop();
                    }
                    System.Console.Out.WriteLine("IN msg: " + msg.ToString() + "\n");
                }

            }
            finally
            {
                node.closeMbox(mbox);
            }
            node.close();
        }

        private void EndWork(bool ok)
        {
            Working = true;
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
}
