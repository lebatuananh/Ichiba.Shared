using System;
using System.Collections.Generic;
using Shared.RedisCache.Interfaces;
using StackExchange.Redis;

namespace Shared.RedisCache.Implements
{
    public class RedisConnection<TRedisConfig> where TRedisConfig : IRedisConfig
    {
        private const int SyncTimeout = 100000;

        private readonly TRedisConfig _redisConfig;
        private readonly SocketManager _socketManager;
        public readonly object SyncConnectionLock = new object();
        public readonly object SyncLock = new object();
        public readonly object SyncReadConnectionLock = new object();
        private ConnectionMultiplexer[] _readConnections;
        private int _readIndexConnection;
        private ConnectionMultiplexer _writeConnection;

        public RedisConnection(TRedisConfig redisConfig)
        {
            _redisConfig = redisConfig;

            _socketManager = new SocketManager(GetType().Name);
            _writeConnection = GetNewWriteConnection();
            _readConnections = GetNewReadConnections();
        }

        public ConnectionMultiplexer GetWriteConnection
        {
            get
            {
                lock (SyncConnectionLock)
                {
                    if (_writeConnection == null)
                    {
                        _writeConnection = GetNewWriteConnection();
                    }
                    else
                    {
                        if (!_writeConnection.IsConnected)
                            _writeConnection = GetNewWriteConnection();
                    }

                    return _writeConnection;
                }
            }
        }


        public IDatabase WriteConnection
        {
            get
            {
                var connection = GetWriteConnection.GetDatabase(_redisConfig.DbId);
                return connection;
            }
        }

        public ConnectionMultiplexer GetReadConnection
        {
            get
            {
                lock (SyncReadConnectionLock)
                {
                    var connection = _readConnections[_readIndexConnection++];
                    if (_readIndexConnection >= _readConnections.Length) _readIndexConnection = 0;
                    if (connection == null)
                    {
                        _readConnections = GetNewReadConnections();
                    }
                    else
                    {
                        if (!connection.IsConnected)
                            _readConnections = GetNewReadConnections();
                    }

                    return connection;
                }
            }
        }

        public IDatabase ReadConnection
        {
            get
            {
                var connection = GetReadConnection.GetDatabase(_redisConfig.DbId);

                return connection;
            }
        }

        public List<string> GetServer(bool isMaster)
        {
            var config = ConfigurationOptions.Parse(_redisConfig.Server);
            config.KeepAlive = 180;
            config.SyncTimeout = SyncTimeout;
            config.AbortOnConnectFail = false;
            config.AllowAdmin = true;
            config.SocketManager = _socketManager;
            if (!string.IsNullOrWhiteSpace(_redisConfig.Password)) config.Password = _redisConfig.Password;
            var connection = ConnectionMultiplexer.ConnectAsync(config);
            var muxer = connection.Result;
            var endpoint = _redisConfig.Server.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries);
            try
            {
                foreach (var endPoint in endpoint)
                {
                    var server = muxer.GetServer(endPoint);
                    if (isMaster)
                    {
                        if (server != null && server.IsConnected && !server.IsSlave) return new List<string> {endPoint};
                    }
                    else
                    {
                        var endPoints = new List<string>();
                        if (server != null && server.IsConnected) endPoints.Add(endPoint);
                        return endPoints;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            throw new Exception("Redis not found");
        }

        private ConnectionMultiplexer GetNewWriteConnection()
        {
            var writeIp = GetServer(true);
            if (writeIp.Count > 0)
            {
                var config = ConfigurationOptions.Parse(writeIp[0]);
                config.KeepAlive = 180;
                config.SyncTimeout = SyncTimeout;
                config.AbortOnConnectFail = true;
                config.AllowAdmin = true;
                //config.ConnectTimeout = IoTimeOut;
                config.SocketManager = _socketManager;
                config.ConnectRetry = 5;
                if (!string.IsNullOrWhiteSpace(_redisConfig.Password)) config.Password = _redisConfig.Password;
                //config.CommandMap = CommandMap.Sentinel;
                //var connection = ConnectionMultiplexer.Connect(config/*, logger*/);
                var connection = ConnectionMultiplexer.Connect(config);
                var muxer = connection;
                return muxer;
            }

            throw new Exception("Redis Write not found");
        }

        private ConnectionMultiplexer[] GetNewReadConnections()
        {
            var readIps = GetServer(false);
            if (readIps.Count > 0)
            {
                var connectionMultiplexers = new ConnectionMultiplexer[readIps.Count];
                for (var i = 0; i < readIps.Count; i++)
                {
                    var config = ConfigurationOptions.Parse(readIps[i]);
                    config.KeepAlive = 180;
                    config.SyncTimeout = SyncTimeout;
                    config.AbortOnConnectFail = true;
                    config.AllowAdmin = true;
                    //config.ConnectTimeout = IoTimeOut;
                    config.SocketManager = _socketManager;
                    config.ConnectRetry = 5;
                    if (!string.IsNullOrWhiteSpace(_redisConfig.Password)) config.Password = _redisConfig.Password;
                    //config.CommandMap = CommandMap.Sentinel;
                    //var connection = ConnectionMultiplexer.Connect(config/*, logger*/);
                    var connection = ConnectionMultiplexer.Connect(config);
                    var muxer = connection;
                    connectionMultiplexers[i] = muxer;
                }

                return connectionMultiplexers;
            }

            throw new Exception("Redis Read not found");
        }
    }
}