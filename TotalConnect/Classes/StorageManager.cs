
namespace TotalConnect.Classes
{
    using System;
    using System.IO.IsolatedStorage;

    public class StorageManager
    {
        /// <summary>
        /// Thread safe lock object.
        /// </summary>
        private static readonly object lockObject = new object();

        /// <summary>
        /// Attempts to read the given file name back to the object type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static T ReadObject<T>(string filename)
        {
            T readObject = default(T);

            lock (lockObject)
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains(filename))
                {
                    readObject = (T)IsolatedStorageSettings.ApplicationSettings[filename];
                }
            }

            return readObject;
        }

        /// <summary>
        /// Attempts to write the given file data to the given file name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="writeObject"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static bool WriteObject<T>(T writeObject, string filename)
        {
            try
            {
                lock (lockObject)
                {
                    // Delete file if exists..
                    if (IsolatedStorageSettings.ApplicationSettings.Contains(filename))
                        IsolatedStorageSettings.ApplicationSettings.Remove(filename);

                    // Add and save the file..
                    IsolatedStorageSettings.ApplicationSettings.Add(filename, writeObject);
                    IsolatedStorageSettings.ApplicationSettings.Save();
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
