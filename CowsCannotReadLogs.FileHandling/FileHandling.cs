using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Xml.Linq;

namespace CowsCannotReadLogs.FileHandling
{
    public interface IFileHandling
    {
        //TODO:Move Data out of FileHandling.
        FileHandling.Data ReadData(string pathFile);
        void Save(string dataPathFile, IEnumerable<FileHandling.Data.Item> items);
    }

    public class FileHandling : IFileHandling
    {

        #region Object state.

        private readonly IFileSystem _fileSystem;

        #endregion

        #region Constructors.

        //TODO:Create static constructors instead.

        /// <summary>This is the default constructor used for production.
        /// </summary>
        public FileHandling()
            :this(new FileSystem())
        {
        }

        /// <summary>This constructor is only used in automatic testing.
        /// </summary>
        /// <param name="fileSystem"></param>
        internal FileHandling(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        #endregion

        #region Public methods.

        /// <summary>This method reads a (log) file, line by line.
        /// </summary>
        /// <param name="pathfile"></param>
        /// <returns></returns>
        public IEnumerable<string> Read(string pathfile)
        {
            string line;
            using (var reader = _fileSystem.File.OpenText(pathfile))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    yield return line;
                }
            }
        }

        /// <summary>This method returns some configuration data.
        /// </summary>
        /// <param name="pathFile"></param>
        /// <returns></returns>
        public Data ReadData(string pathFile)
        {
            using (var fileStream = _fileSystem.File.Open(pathFile, System.IO.FileMode.Open))
            {
                var xdoc = XDocument.Load(fileStream);
                var persistData = ToPersistData(xdoc);
                var ret = Data.Create(
                    persistData.Version,
                    persistData.Items.Select(pi => Data.Item.Create(
                        pi.Name, 
                        pi.RecogniserFunction, 
                        pi.ParserFunction)));
                return ret;
            }
        }

        /// <summary>This method persists some configuration data in a file system (<see cref="IFileSystem"/>).
        /// </summary>
        /// <param name="pathFile"></param>
        /// <param name="items"></param>
        public void Save(string pathFile, IEnumerable<Data.Item> items)
        {
            var version = ReadData(_fileSystem, pathFile).Version;

            var xml = ToXml(items, version);

            // We cannot use `xml.Save(pathFile);` as I have found no way to inject the file system into `XDocument`.
            _fileSystem.File.WriteAllText(pathFile, xml.ToString());
        }

        #endregion

        #region Helper methods.

        private static PersistData ReadData(IFileSystem fileSystem, string pathFile)
        {
            CreateDirectoriesForPathFileIfTheDirectoryDoesNotExist(fileSystem, pathFile);
            
            if (fileSystem.File.Exists(pathFile))
            {
                using (var fileStream = fileSystem.File.Open(pathFile, System.IO.FileMode.OpenOrCreate))
                {
                    var xdoc = XDocument.Load(fileStream);
                    var persistData = ToPersistData(xdoc);
                    return persistData;
                }
            }
            else
            {
                return PersistData.Create(1, new PersistData.PersistItem[0]);
            }
        }

        private static void CreateDirectoriesForPathFileIfTheDirectoryDoesNotExist(IFileSystem fileSystem, string pathFile)
        {
            fileSystem.Directory.CreateDirectory(fileSystem.Path.GetDirectoryName(pathFile));
        }

        private static PersistData ToPersistData(XDocument xdoc)
        {
            var version = int.Parse(xdoc.Root.Attribute("Version").Value);
            var items = xdoc.Root.Descendants("Item")
                .Select(i => PersistData.PersistItem.Create(
                    i.Element("Name").Value, 
                    i.Element("RecogniserFunction").Value, 
                    i.Element("ParserFunction").Value));
            return PersistData.Create(version, items.ToArray());
        }

        private static XDocument ToXml(IEnumerable<Data.Item> items, int version)
        {
            var xdoc = new XDocument();
            var root = new XElement("Data", new XAttribute("Version", version));
            xdoc.Add(root);
            var itemsElement = new XElement("Items");
            root.Add(itemsElement);
            foreach (var item in items)
            {
                var itemElement = new XElement("Item");
                itemElement.Add(new XElement("Name", item.Name));
                itemElement.Add(new XElement("RecogniserFunction", item.RecogniserFunction));
                itemElement.Add(new XElement("ParserFunction", item.ParserFunction));
                itemsElement.Add(itemElement);
            }

            return xdoc;
        }

        #endregion

        /// <summary>This class is part of the public interface.
        /// </summary>
        public struct Data
        {
            public readonly IEnumerable<Item> Items;

            public int Version { get; }

            public static Data Create(int version, params Item[] items)
            {
                return new Data(version, items);
            }

            internal static Data Create(int version, IEnumerable<Item> items)
            {
                return Create(version, items.ToArray());
            }

            private Data(int version, Item[] items)
            {
                Items = new List<Item>(items.Length);
                foreach (var item in items)
                {
                    ((List<Item>)Items).Add(item);
                }

                Version = version;
            }

            public struct Item
            {
                public string Name { get; }
                public string RecogniserFunction { get; }
                public string ParserFunction { get; }

                public static Item Create(string name, string recogniserFunction, string parserFunction)
                {
                    return new Item(name, recogniserFunction, parserFunction);
                }
                private Item(string name, string recogniserFunction, string parserFunction)
                {
                    Name = name;
                    RecogniserFunction = recogniserFunction;
                    ParserFunction = parserFunction;
                }
            }
        }

        /// <summary>This class is what is persisted.
        /// It is not part of a public interface but needs to be internal to make automatic testing work.
        /// </summary>
        internal class PersistData
        {
            internal int Version { get; set; }
            internal PersistItem[] Items { get; set; }
            internal static PersistData Create(int version, params PersistItem[] items)
            {
                var ret = new PersistData
                {
                    Version = version,
                    Items = items
                };
                return ret;
            }
            internal class PersistItem
            {
                internal string Name { get; set; }
                internal string RecogniserFunction { get; set; }
                internal string ParserFunction { get; set; }
                internal static PersistItem Create(string name, string recogniserFunction, string parserFunction)
                {
                    var ret = new PersistItem
                    {
                        Name = name,
                        RecogniserFunction = recogniserFunction,
                        ParserFunction = parserFunction
                    };
                    return ret;
                }
            }
        }
    }
}
