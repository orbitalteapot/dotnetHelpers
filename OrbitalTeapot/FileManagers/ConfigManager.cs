using System;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;

namespace OrbitalTeapot.FileManagers
{
    public interface IConfigManager<T>
    {
        /// <summary>
        /// Read Configuration from File
        /// </summary>
        /// <param name="filename"></param>
        /// <returns> <see cref="T"/></returns>
        Task<T> ReadConfig(string filename);

        /// <summary>
        /// Write Configuration to File
        /// </summary>
        /// <param name="config"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        Task WriteConfig(T config, string filename);
    }

    /// <summary>
    /// Create config manager for passed in type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ConfigManager<T> : IConfigManager<T>
    {

        /// <summary>
        /// Read Configuration from File
        /// </summary>
        /// <param name="filename"></param>
        /// <returns> <see cref="T"/></returns>
        public async Task<T> ReadConfig(string filename)
        {
            try
            {
                await using var openStream = File.OpenRead(filename);
                var result = await JsonSerializer.DeserializeAsync<T>(openStream);
                await openStream.DisposeAsync();
                return result ?? throw new Exception("Unable to read configuration file");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }

        /// <summary>
        /// Write Configuration to File
        /// </summary>
        /// <param name="config"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public async Task WriteConfig(T config, string filename)
        {
            try
            {
                await using var createStream = File.Create(filename);
                await JsonSerializer.SerializeAsync(createStream, config, new JsonSerializerOptions { WriteIndented = true });
                await createStream.DisposeAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message,ex);
            }
            
        }
    }
}
