using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shared.Common;
using Shared.Common.Extension;
using Shared.RedisCache.Interfaces;
using StackExchange.Redis;

namespace Shared.RedisCache.Implements
{
    public class RedisStorage : IRedisStorage
    {
        protected readonly IDatabase ReadDatabase;
        protected readonly IDatabase WriteDatabase;

        public RedisStorage()
        {
            WriteDatabase = RedisConnection.WriteConnection;
            ReadDatabase = RedisConnection.ReadConnection;
        }

        public RedisStorage(IDatabase _writeDatabase,
            IDatabase _readDatabase)
        {
            WriteDatabase = _writeDatabase;
            ReadDatabase = _readDatabase;
        }

        public async Task<bool> LockTake(string key, string value, TimeSpan timeout)
        {
            return await WriteDatabase.LockTakeAsync(key, value, timeout);
        }

        public async Task<bool> LockRelease(string key, string value)
        {
            return await WriteDatabase.LockReleaseAsync(key, value);
        }

        public async Task<bool> StringSet(string key, object value)
        {
            RedisValue redisValue = ConvertInput(value);
            return await WriteDatabase.StringSetAsync(key, redisValue);
        }

        public async Task<bool> StringSet(string key, object value, TimeSpan? timeout)
        {
            RedisValue redisValue = ConvertInput(value);
            return await WriteDatabase.StringSetAsync(key, redisValue, timeout.Value);
        }

        public async Task<bool> KeyDelete(string key)
        {
            return await WriteDatabase.KeyDeleteAsync(key);
        }

        public async Task<T> StringGet<T>(string key)
        {
            var value = await ReadDatabase.StringGetAsync(key);
            return ConvertOutput<T>(value);
        }

        public async Task<T[]> StringGet<T>(string[] keys)
        {
            var redisKeys = new RedisKey[keys.Length];
            for (var i = 0; i < keys.Length; i++) redisKeys[i] = keys[i];
            var values = await ReadDatabase.StringGetAsync(redisKeys);
            var results = new T[keys.Length];
            var j = 0;
            foreach (var redisValue in values)
            {
                if (redisValue.HasValue)
                {
                    var obj = ConvertOutput<T>(redisValue);
                    results[j] = obj;
                }

                j++;
            }

            return results;
        }

        public async Task<long> ListLeftPush(string key, object value)
        {
            return await WriteDatabase.ListLeftPushAsync(key, ConvertInput(value));
        }

        public async Task<bool> HashSet(string key, string field, object value)
        {
            RedisValue redisValue = ConvertInput(value);
            var result = await WriteDatabase.HashSetAsync(key, field, redisValue);
            return result;
        }

        public async Task<long> HashIncrement(string key, string field, long value)
        {
            return await WriteDatabase.HashIncrementAsync(key, field, value);
        }

        public async Task<long?> HashIncrement(string key, string field, long value, long minValue)
        {
            try
            {
                var luaScript =
                    @"local curentValue = tonumber(redis.call('HGET',@key,@field)); local value = tonumber(@value);
                                  if  curentValue == nil then redis.call('HSET',@key,@field, 0); curentValue = 0;  end 
                                  if (curentValue +  value) >= tonumber(@minvalue) then return redis.call('HINCRBY',@key,@field, @value) 
                                  else return nil end";
                var prepared = LuaScript.Prepare(luaScript);
                var val = await prepared.EvaluateAsync(WriteDatabase, new {key, field, value, minvalue = minValue});
                if (val.IsNull) return null;
                return val.AsLong();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task<KeyValuePair<string, KeyValuePair<string, T>[]>[]> HashGetAll<T>(string[] keys)
        {
            var luaScript = @"
local function splitString(inputstr, sep)
                                        if sep == nil then
                                                sep = '%s'
                                        end
                                        local t={{}} ; local i=1
                                        for str in string.gmatch(inputstr, '([^'..sep..']+)') do
                                                t[i] = str;
                                                i = i + 1;
                                        end
                                        return t
                                end
local resultKeys = splitString(@keys, '@');
local r = {}
                                    for _, v in pairs(resultKeys) do
                                      r[#r+1] = redis.call('HGETALL', v)
                                    end

                                    return r";
            var redisKeys = string.Join("@", keys);
            var prepared = LuaScript.Prepare(luaScript);
            var vals = await prepared.EvaluateAsync(ReadDatabase, new {keys = redisKeys});
            var valuePairs = new List<KeyValuePair<string, KeyValuePair<string, T>[]>>();
            if (!vals.IsNull)
            {
                var results = (RedisResult[]) vals;

                var j = 0;
                foreach (var redisResult in results)
                {
                    var valuePairsByKey = new List<KeyValuePair<string, T>>();
                    if (redisResult.IsNull) continue;
                    var resultsByKey = (RedisResult[]) redisResult;
                    for (var i = 0; i < resultsByKey.Length; i += 2)
                    {
                        var key = resultsByKey[i];
                        var value = resultsByKey[i + 1];
                        if (value != null)
                        {
                            var valueObject = ConvertOutput<T>((byte[]) value);
                            valuePairsByKey.Add(new KeyValuePair<string, T>((string) key, valueObject));
                        }
                        else
                        {
                            valuePairsByKey.Add(new KeyValuePair<string, T>((string) key, default));
                        }
                    }

                    valuePairs.Add(
                        new KeyValuePair<string, KeyValuePair<string, T>[]>(keys[j], valuePairsByKey.ToArray()));
                    j++;
                }
            }

            return valuePairs.ToArray();
        }

        public async Task<KeyValuePair<string, KeyValuePair<string, long>[]>[]> HashGetAll(string[] keys)
        {
            var luaScript = @"
local function splitString(inputstr, sep)
                                        if sep == nil then
                                                sep = '%s'
                                        end
                                        local t={{}} ; local i=1
                                        for str in string.gmatch(inputstr, '([^'..sep..']+)') do
                                                t[i] = str;
                                                i = i + 1;
                                        end
                                        return t
                                end
local resultKeys = splitString(@keys, '@');
local r = {}
                                    for _, v in pairs(resultKeys) do
                                      r[#r+1] = redis.call('HGETALL', v)
                                    end

                                    return r";
            var redisKeys = string.Join("@", keys);
            var prepared = LuaScript.Prepare(luaScript);
            var vals = await prepared.EvaluateAsync(ReadDatabase, new {keys = redisKeys});
            var valuePairs = new List<KeyValuePair<string, KeyValuePair<string, long>[]>>();
            if (!vals.IsNull)
            {
                var results = (RedisResult[]) vals;

                var j = 0;
                foreach (var redisResult in results)
                {
                    var valuePairsByKey = new List<KeyValuePair<string, long>>();
                    if (redisResult.IsNull) continue;
                    var resultsByKey = (RedisResult[]) redisResult;
                    for (var i = 0; i < resultsByKey.Length; i += 2)
                    {
                        var key = resultsByKey[i];
                        var value = resultsByKey[i + 1];
                        if (value != null)
                            valuePairsByKey.Add(new KeyValuePair<string, long>((string) key, (long) value));
                        else
                            valuePairsByKey.Add(new KeyValuePair<string, long>((string) key, 0));
                    }

                    valuePairs.Add(
                        new KeyValuePair<string, KeyValuePair<string, long>[]>(keys[j], valuePairsByKey.ToArray()));
                    j++;
                }
            }

            return valuePairs.ToArray();
        }

        public async Task<T[]> HashGetAll<T>(string key)
        {
            var hashEntries = await ReadDatabase.HashGetAllAsync(key);
            if (hashEntries.Length > 0)
            {
                var results = new T[hashEntries.Length];
                var i = 0;
                foreach (var hashEntry in hashEntries)
                {
                    results[i] = ConvertOutput<T>(hashEntry.Value);
                    i++;
                }

                return results;
            }

            return default;
        }

        public async Task<T> HashGet<T>(string key, string field)
        {
            var redisValue = await ReadDatabase.HashGetAsync(key, field);
            return ConvertOutput<T>(redisValue);
        }

        public async Task<T[]> HashGet<T>(string key, string[] fields)
        {
            var redisValues = fields.Select(p => (RedisValue) p).ToArray();
            var values = await ReadDatabase.HashGetAsync(key, redisValues);
            var results = new T[values.Length];
            var i = 0;
            foreach (var redisValue in values)
            {
                if (redisValue.HasValue)
                {
                    var obj = ConvertOutput<T>(redisValue);
                    results[i] = obj;
                }

                i++;
            }

            return results;
        }

        public async Task<bool> HashDelete(string key, string field)
        {
            return await WriteDatabase.HashDeleteAsync(key, field);
        }

        public Task<bool> KeyExist(string key)
        {
            return Task.FromResult(ReadDatabase.KeyExists(key));
        }

        public async Task<bool> HashExists(string key, string field)
        {
            return await WriteDatabase.HashExistsAsync(key, field);
        }

        public Task HashScan<T>(string key, Action<string, T> iteratorAction, int pageSize = 10)
        {
            var response = ReadDatabase.HashScan(key, pageSize: pageSize);

            foreach (var item in response)
            {
                var tValue = ConvertOutput<T>(item.Value);

                if (iteratorAction == null) throw new ArgumentNullException(nameof(iteratorAction));

                iteratorAction.Invoke(item.Name, tValue);
            }

            return Task.CompletedTask;
        }

        public Task HashScan<T>(string key, Func<string, T, bool> iteratorFunc, int pageSize = 10)
        {
            var response = ReadDatabase.HashScan(key, pageSize: pageSize);

            foreach (var item in response)
            {
                var tValue = ConvertOutput<T>(item.Value);

                if (iteratorFunc == null) throw new ArgumentNullException(nameof(iteratorFunc));

                var isContinue = iteratorFunc.Invoke(item.Name, tValue);

                if (!isContinue) break;
            }

            return Task.CompletedTask;
        }

        private byte[] ConvertInput(object value)
        {
            return Serialize.ProtoBufSerialize(value);
        }

        private T ConvertOutput<T>(RedisValue redisValue)
        {
            if (redisValue.HasValue) return Serialize.ProtoBufDeserialize<T>(redisValue);
            return default;
        }
    }
}