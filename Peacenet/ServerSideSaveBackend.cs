﻿using Plex.Engine.Saves;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plex.Engine;
using System.IO;
using Newtonsoft.Json;
using Peacenet.Server;

namespace Peacenet
{
    /// <summary>
    /// Provides an implementation of <see cref="ISaveBackend"/> capable of interacting with a Peacenet server's save files. 
    /// </summary>
    public class ServerSideSaveBackend : ISaveBackend
    {
        [Dependency]
        private AsyncServerManager _server = null;

        /// <inheritdoc/>
        public string CreateSnapshot()
        {
            string result = "";
            _server.SendMessage(Plex.Objects.ServerMessageType.SAVE_TAKESNAPSHOT, null, (res, reader) =>
            {
                if (res == Plex.Objects.ServerResponseType.REQ_SUCCESS)
                    result = reader.ReadString();
                else
                    result = null;
            }).Wait();
            return result;
        }

        /// <inheritdoc/>
        public T GetValue<T>(string key, T defaultValue)
        {
            if (_server.Connected == false)
                throw new InvalidOperationException("You are not connected to a server, so this functionality is not available.");

            object result = null;
            Exception err = null;

            byte[] defBody = null;
            using(var memstr = new MemoryStream())
            {
                using(var writer = new BinaryWriter(memstr, Encoding.UTF8))
                {
                    writer.Write(key);
                    writer.Write(JsonConvert.SerializeObject(defaultValue));

                    defBody = memstr.ToArray();
                }
            }

            _server.SendMessage(Plex.Objects.ServerMessageType.SAVE_GETVAL, defBody, (res, reader) =>
            {
                try
                {
                    if(res == Plex.Objects.ServerResponseType.REQ_SUCCESS)
                    {
                        string json = reader.ReadString();
                        result = JsonConvert.DeserializeObject<T>(json);
                    }
                    else
                    {
                        throw new Exception($"Server returned error code {res} while fetching save value \"{key}\".");
                    }
                }
                catch (Exception ex)
                {
                    err = ex;
                }
            }).Wait();

            if (err != null)
                throw err;

            return (T)result;
        }

        /// <inheritdoc/>
        public void RestoreSnapshot(string id)
        {
            byte[] body = null;
            using (var memstr = new MemoryStream())
            {
                using(var writer = new BinaryWriter(memstr))
                {
                    writer.Write(id);
                    body = memstr.ToArray();
                }
            }

            _server.SendMessage(Plex.Objects.ServerMessageType.SAVE_RESTORESNAPSHOT, body, (res, reader) =>
            {

            }).Wait();
        }

        /// <inheritdoc/>
        public void SetValue<T>(string key, T value)
        {
            if (_server.Connected == false)
                throw new InvalidOperationException("You are not connected to a server, so this functionality is not available.");
            Exception err = null;

            byte[] defBody = null;
            using (var memstr = new MemoryStream())
            {
                using (var writer = new BinaryWriter(memstr, Encoding.UTF8))
                {
                    writer.Write(key);
                    writer.Write(JsonConvert.SerializeObject(value));

                    defBody = memstr.ToArray();
                }
            }

            _server.SendMessage(Plex.Objects.ServerMessageType.SAVE_SETVAL, defBody, (res, reader) =>
            {
                try
                {
                    if (res != Plex.Objects.ServerResponseType.REQ_SUCCESS)
                    {
                        throw new Exception($"Server returned error code {res} while setting save value \"{key}\".");
                    }
                }
                catch (Exception ex)
                {
                    err = ex;
                }
            }).Wait();

            if (err != null)
                throw err;
        }
    }
}
