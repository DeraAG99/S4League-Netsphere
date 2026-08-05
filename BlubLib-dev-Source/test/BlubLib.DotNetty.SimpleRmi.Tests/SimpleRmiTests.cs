using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using BlubLib.Serialization;
using DotNetty.Common.Internal.Logging;
using DotNetty.Handlers.Logging;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace BlubLib.DotNetty.SimpleRmi.Tests
{
    [TestClass]
    public class SimpleRmiTests
    {
        [TestMethod]
        public void KeepAliveTest()
        {
            var tcs = new TaskCompletionSource<object>();
            var server = new ServerBootstrap()
                .Group(new MultithreadEventLoopGroup())
                .Channel<TcpServerSocketChannel>()
                .Handler(new ActionChannelInitializer<IChannel>(ch => { }))
                .ChildHandler(new ActionChannelInitializer<IChannel>(ch =>
                {
                    var rmi = new SimpleRmiHandler();
                    ch.Pipeline.AddLast("rmi", rmi)
                        .AddAfter("rmi", null, new MessageReceived((_, message) => { tcs.TrySetResult(message); }));
                }))
                .BindAsync(IPAddress.Loopback, 29000).WaitEx();

            IChannel client = null;
            try
            {
                client = new Bootstrap()
                    .Group(new MultithreadEventLoopGroup())
                    .Channel<TcpSocketChannel>()
                    .Handler(new ActionChannelInitializer<IChannel>(ch =>
                    {
                        var rmi = new SimpleRmiHandler();
                        ch.Pipeline.AddLast(rmi);
                    }))
                    .ConnectAsync(IPAddress.Loopback, 29000).WaitEx();

                client.KeepAliveAsync().WaitEx();

                object result = null;
                try
                {
                    using (var cts = new CancellationTokenSource(2000))
                        result = tcs.Task.WaitEx(cts.Token);
                }
                catch (OperationCanceledException)
                {
                    Assert.Fail("Did not receive keep alive");
                }

                var type = result.GetType();
                Assert.AreEqual(typeof(KeepAliveMessage), type);
            }
            finally
            {
                client?.CloseAsync().WaitEx();
                server.CloseAsync().WaitEx();
            }
        }

        [TestMethod]
        public void InheritanceTest()
        {
            InternalLoggerFactory.DefaultFactory.AddProvider(new ConsoleLoggerProvider());
            var foo = new TaskCompletionSource<string>();
            var bar = new TaskCompletionSource<int>();
            var test = new TaskCompletionSource<Guid>();

            var server = new ServerBootstrap()
                .Group(new MultithreadEventLoopGroup())
                .Channel<TcpServerSocketChannel>()
                .Handler(new ActionChannelInitializer<IChannel>(ch => { }))
                .ChildHandler(new ActionChannelInitializer<IChannel>(ch =>
                {
                    var rmi = new SimpleRmiHandler();
                    rmi.AddService(new Service(foo, bar, test));
                    ch.Pipeline.AddLast(rmi).AddFirst(new LoggingHandler());
                }))
                .BindAsync(IPAddress.Loopback, 29000).WaitEx();

            IChannel client = null;
            try
            {
                client = new Bootstrap()
                    .Group(new MultithreadEventLoopGroup())
                    .Channel<TcpSocketChannel>()
                    .Handler(new ActionChannelInitializer<IChannel>(ch =>
                    {
                        var rmi = new SimpleRmiHandler();
                        ch.Pipeline.AddLast("rmi", rmi)
                            .AddFirst(new LoggingHandler());
                    }))
                    .ConnectAsync(IPAddress.Loopback, 29000).WaitEx();

                var proxy = client.GetProxy<ITest>();

                proxy.Foo("test");
                string fooResult = null;
                try
                {
                    using (var cts = new CancellationTokenSource(1000))
                        fooResult = foo.Task.WaitEx(cts.Token);
                }
                catch (OperationCanceledException)
                {
                    Assert.Fail("Did not receive Foo");
                }
                Assert.AreEqual("test", fooResult);

                proxy.Bar(2);
                var barResult = 0;
                try
                {
                    using (var cts = new CancellationTokenSource(5000))
                        barResult = bar.Task.WaitEx(cts.Token);
                }
                catch (OperationCanceledException)
                {
                    Assert.Fail("Did not receive bar");
                }
                Assert.AreEqual(2, barResult);

                var guid = Guid.NewGuid();
                proxy.Test(guid);
                var testResult = Guid.Empty;
                try
                {
                    using (var cts = new CancellationTokenSource(5000))
                        testResult = test.Task.WaitEx(cts.Token);
                }
                catch (OperationCanceledException)
                {
                    Assert.Fail("Did not receive bar");
                }
                Assert.AreEqual(guid, testResult);
            }
            finally

            {
                client?.CloseAsync().WaitEx();
                server.CloseAsync().WaitEx();
            }
        }

        [TestMethod]
        public void ParameterAttributeTest()
        {
            var tcs = new TaskCompletionSource<byte[]>(TaskCreationOptions.RunContinuationsAsynchronously);
            var server = new ServerBootstrap()
                .Group(new MultithreadEventLoopGroup())
                .Channel<TcpServerSocketChannel>()
                .Handler(new ActionChannelInitializer<IChannel>(ch => { }))
                .ChildHandler(new ActionChannelInitializer<IChannel>(ch =>
                {
                    var rmi = new SimpleRmiHandler();
                    rmi.AddService(new Service(tcs));
                    ch.Pipeline.AddLast(rmi);
                }))
                .BindAsync(IPAddress.Loopback, 29000).WaitEx();
            IChannel client = null;
            try
            {
                client = new Bootstrap()
                    .Group(new MultithreadEventLoopGroup())
                    .Channel<TcpSocketChannel>()
                    .Handler(new ActionChannelInitializer<IChannel>(ch =>
                    {
                        var rmi = new SimpleRmiHandler();
                        ch.Pipeline.AddLast("rmi", rmi);
                    }))
                    .ConnectAsync(IPAddress.Loopback, 29000).WaitEx();

                var proxy = client.GetProxy<IParameterAttribute>();
                var arr = new byte[] {3, 2, 1};
                byte[] returnArr = null;
                try
                {
                    using (var cts = new CancellationTokenSource(5000))
                        returnArr = proxy.Custom(arr).WaitEx(cts.Token);
                }
                catch (OperationCanceledException)
                {
                    Assert.Fail("Custom timeout");
                }
                CollectionAssert.AreEqual(new byte[] {1, 2, 3}, returnArr);

                byte[] result = null;
                try
                {
                    using (var cts = new CancellationTokenSource(5000))
                        result = tcs.Task.WaitEx(cts.Token);
                }
                catch (OperationCanceledException)
                {
                    Assert.Fail("Did not receive Custom");
                }
                CollectionAssert.AreEqual(arr, result);
            }
            finally
            {
                client?.CloseAsync().WaitEx();
                server.CloseAsync().WaitEx();
            }
        }

        [TestMethod]
        public void BidirectionalTest()
        {
            var serverTcs = new TaskCompletionSource<string>();
            var clientTcs = new TaskCompletionSource<string>();
            IChannel serverSession = null;
            var server = new ServerBootstrap()
                .Group(new MultithreadEventLoopGroup())
                .Channel<TcpServerSocketChannel>()
                .Handler(new ActionChannelInitializer<IChannel>(ch => { }))
                .ChildHandler(new ActionChannelInitializer<IChannel>(ch =>
                {
                    var rmi = new SimpleRmiHandler();
                    rmi.AddService(new Service(serverTcs, null, null));
                    ch.Pipeline.AddLast(rmi);
                    serverSession = ch;
                }))
                .BindAsync(IPAddress.Loopback, 29000).WaitEx();
            IChannel client = null;
            try
            {
                client = new Bootstrap()
                    .Group(new MultithreadEventLoopGroup())
                    .Channel<TcpSocketChannel>()
                    .Handler(new ActionChannelInitializer<IChannel>(ch =>
                    {
                        var rmi = new SimpleRmiHandler();
                        rmi.AddService(new Service(clientTcs, null, null));
                        ch.Pipeline.AddLast("rmi", rmi);
                    }))
                    .ConnectAsync(IPAddress.Loopback, 29000).WaitEx();
                client.GetProxy<IFoo>().Foo("test");
                string result = null;
                try
                {
                    using (var cts = new CancellationTokenSource(1000))
                        result = serverTcs.Task.WaitEx(cts.Token);
                }
                catch (OperationCanceledException)
                {
                    Assert.Fail("Did not receive server");
                }
                Assert.AreEqual("test", result);
                serverSession.GetProxy<IFoo>().Foo("test2");
                try
                {
                    using (var cts = new CancellationTokenSource(1000))
                        result = clientTcs.Task.WaitEx(cts.Token);
                }
                catch (OperationCanceledException)
                {
                    Assert.Fail("Did not receive client");
                }
                Assert.AreEqual("test2", result);
            }
            finally
            {
                client?.CloseAsync().WaitEx();
                server.CloseAsync().WaitEx();
            }
        }

        private class MessageReceived : ChannelHandlerAdapter
        {
            private readonly Action<IChannelHandlerContext, object> _callback;

            public MessageReceived(Action<IChannelHandlerContext, object> callback)
            {
                _callback = callback;
            }

            public override void ChannelRead(IChannelHandlerContext context, object message)
            {
                _callback(context, message);
                base.ChannelRead(context, message);
            }
        }
    }

    public class ConsoleLoggerProvider : ILoggerProvider
    {
        public ILogger CreateLogger(string categoryName)
        {
            return new ConsoleLogger(categoryName);
        }

        public void Dispose()
        {
        }

        private class ConsoleLogger : ILogger
        {
            private readonly string _name;

            public ConsoleLogger(string name)
            {
                _name = name;
            }

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
                Func<TState, Exception, string> formatter)
            {
                Console.WriteLine($"[{logLevel.ToString().ToUpper()}] {_name}: {formatter(state, exception)}");
            }

            public bool IsEnabled(LogLevel logLevel)
            {
                return true;
            }

            public IDisposable BeginScope<TState>(TState state)
            {
                return new DummyScope();
            }

            private class DummyScope : IDisposable
            {
                public void Dispose()
                {
                }
            }
        }
    }

    public class Service : RmiService, ITest, IParameterAttribute
    {
        private readonly TaskCompletionSource<string> _foo;
        private readonly TaskCompletionSource<int> _bar;
        private readonly TaskCompletionSource<Guid> _test;
        private readonly TaskCompletionSource<byte[]> _custom;

        public Service(TaskCompletionSource<string> foo, TaskCompletionSource<int> bar, TaskCompletionSource<Guid> test)
        {
            _foo = foo;
            _bar = bar;
            _test = test;
        }

        public Service(TaskCompletionSource<byte[]> custom)
        {
            _custom = custom;
        }

        public void Foo(string str)
        {
            _foo.TrySetResult(str);
        }

        public void Bar(int a)
        {
            _bar.TrySetResult(a);
        }

        public void Test(Guid guid)
        {
            _test.TrySetResult(guid);
        }

        public Task<byte[]> Custom(byte[] arr)
        {
            _custom.TrySetResult(arr);
            return Task.FromResult(new byte[] {1, 2, 3});
        }
    }

    public class CustomSerializer : ISerializer<byte[]>
    {
        public bool CanHandle(Type type) => type == typeof(byte[]);

        public void Serialize(BinaryWriter writer, byte[] value)
        {
            writer.Write(value.Length);
            writer.Write(value);
        }

        public byte[] Deserialize(BinaryReader reader)
        {
            return reader.ReadBytes(reader.ReadInt32());
        }
    }

    [RmiContract]
    public interface IParameterAttribute
    {
        [Rmi]
        [return: RmiSerializer(typeof(CustomSerializer))]
        Task<byte[]> Custom([RmiSerializer(typeof(CustomSerializer))] byte[] arr);
    }

    [RmiContract]
    public interface IFoo
    {
        [Rmi]
        void Foo(string str);
    }

    [RmiContract]
    public interface IBar : IFoo
    {
        [Rmi]
        void Bar(int a);
    }

    [RmiContract]
    public interface ITest : IBar
    {
        [Rmi]
        void Test(Guid guid);
    }
}
