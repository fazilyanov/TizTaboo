using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace TizTaboo
{
    /// <summary>
    /// Хранит и обрабатывает список ссылок
    /// </summary>
    internal class Links
    {
        /// <summary>
        /// Список ссылок
        /// </summary>
        public List<Link> LinkList { get; set; }

        /// <summary>
        /// Путь до файла с алиасами рядом с приложением
        /// </summary>
        public string DataFilePath { get; set; }

        /// <summary>
        /// Количество ссылок
        /// </summary>
        public int Count
        {
            get { return LinkList.Count; }
        }

        /// <summary>
        /// Хранит и обрабатывает список ссылок
        /// </summary>
        /// <param name="filePath">Путь для загрузки и сохранения</param>
        public Links(string filePath)
        {
            LinkList = new List<Link>();
            DataFilePath = filePath;
        }

        /// <summary>
        /// Добавляет новую ссылку
        /// </summary>
        /// <param name="link"></param>
        public void Add(Link link)
        {
            LinkList.Add(link);
        }

        /// <summary>
        /// Удаляет из списка ссылок по алиасу
        /// </summary>
        /// <param name="alias">Алиас для поиска</param>
        /// <returns></returns>
        public bool DeleteByAlias(string alias)
        {
            Link link = null;
            link = LinkList.Find(item => item.Alias == alias);
            if (link != null)
            {
                LinkList.Remove(link);
                return true;
            }
            else
            {
                Log.Error("#201706021415: Ссылка не найдена");
                return false;
            }
        }

        /// <summary>
        /// Возвращает экземпляр ссылки по алиасу
        /// </summary>
        /// <param name="alias">Алиас для поиска</param>
        /// <returns></returns>
        public Link GetByAlias(string alias)
        {
            return LinkList.Find(item => item.Alias.Trim() == alias);
        }

        /// <summary>
        /// Загружает данные из файла данных
        /// </summary>
        /// <returns></returns>
        public bool Load()
        {
            string[] arrParam = null;
            string[] arrNotes = null;
            string toLoad = null;
            try
            {
                toLoad = File.ReadAllText(DataFilePath);

                if (!string.IsNullOrEmpty(toLoad))
                {
                    arrNotes = toLoad.Split(new string[] { "*#*\n" }, StringSplitOptions.None);
                    if (arrNotes.Length > 0)
                    {
                        // Генерим список
                        foreach (string note in arrNotes)
                        {
                            if (!string.IsNullOrEmpty(note))
                            {
                                arrParam = note.Split(new string[] { "*|*" }, StringSplitOptions.None);
                                if (arrParam.Length > 0)
                                {
                                    LinkList.Add(new Link()
                                    {
                                        Alias = arrParam[0],
                                        Name = arrParam[1],
                                        Command = arrParam[2],
                                        Param = arrParam[3],
                                        Confirm = arrParam[4] == "1",
                                        LastEditDate = DateTime.ParseExact(arrParam[5], "dd.MM.yyyy HH:mm:ss", new CultureInfo("ru-RU")),
                                        Type = arrParam[6].ParseEnum<LinkType>(),
                                        RunCount = arrParam[7].ToULong()
                                    });
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("#201706021414: " + ex.Message);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Записываем данные в файл
        /// </summary>
        public bool Save()
        {
            try
            {
                string toSaveText = string.Empty;
                foreach (Link item in LinkList)
                {
                    toSaveText += item.Alias + "*|*";
                    toSaveText += item.Name + "*|*";
                    toSaveText += item.Command + "*|*";
                    toSaveText += item.Param + "*|*";
                    toSaveText += item.Confirm ? "1" : "0" + "*|*";
                    toSaveText += item.LastEditDate.ToString("dd.MM.yyyy HH:mm:ss") + "*|*";
                    toSaveText += item.Type.ToString() + "*|*";
                    toSaveText += item.RunCount + "*#*\n";
                }

                File.WriteAllText(DataFilePath, toSaveText);
            }
            catch (Exception ex)
            {
                Log.Error("#201706021426: " + ex.Message);
                return false;
            }
            return true;
        }

        public bool Sync()
        {
            // Путь для скаченного из облака файла
            string cloudDataFilePath = Application.StartupPath + "\\data_cloud";

            // Подключаемся к облаку
            Drive drive = new Drive()
            {
                fileId = Properties.Settings.Default.gFileId,
            };

            // Если запуск первый, или слетели настройки, нужно найти файлы в облаке
            if (drive.fileId == "0")
            {
                drive.fileId = drive.Find("TizTabooDataFile");
            }
            

            // Если файла в облаке есть, работаем с ним дальше
            if (drive.FileExists())
            {
                // Пытаемся скачать файл
                if (!drive.DownloadFile(cloudDataFilePath)) return false;

                // Загружаем в класс
                Links cloudLinks = new Links(cloudDataFilePath);
                cloudLinks.Load();

                // Узнаем когда последний раз синхронизировались
                DateTime lastSyncDate = Properties.Settings.Default.LastSyncDate;

                // Теперь их надо сравнить..
                // Для начала добавим новые записи и обновим измененные
                foreach (Link cl in cloudLinks.LinkList)
                {
                    // Если дата изменения облачной версии больше чем дата последней синхронизации
                    if (cl.LastEditDate > lastSyncDate)
                    {
                        // Если запись локальная есть - удаляем ее
                        if (Program.Links.GetByAlias(cl.Alias) != null)
                        {
                            Program.Links.DeleteByAlias(cl.Alias);
                        }
                        Program.Links.Add(cl);
                    }
                }

                // Удалим удаленные
                List<string> alias4del = new List<string>();
                foreach (Link ll in Program.Links.LinkList)
                {
                    if (cloudLinks.GetByAlias(ll.Alias) == null && ll.LastEditDate < lastSyncDate)
                    {
                        alias4del.Add(ll.Alias);
                    }
                }
                foreach (string  item in alias4del)
                {
                    Program.Links.DeleteByAlias(item);
                }
            }

            // Заливаем обновленный файл обратно
            if (drive.UploadFile(DataFilePath))
            {
                Properties.Settings.Default.gFileId = drive.fileId;
                Properties.Settings.Default.LastSyncDate = DateTime.Now;
                Properties.Settings.Default.Save();
            }

            return true;
        }

        /// <summary>
        /// Поиск ссылки
        /// </summary>
        /// <param name="query">Введенный текст</param>
        /// <returns></returns>
        public List<Link> Seek(string query)
        {
            List<Link> found = new List<Link>();
            string query_rus = query.EngToRus();
            string query_eng = query.RusToEng();
            found = LinkList.FindAll(
                delegate (Link n)
                {
                    return n.Name.ToLower().Contains(query_rus) || n.Name.ToLower().Contains(query_eng) || n.Alias.ToLower().Contains(query_rus) || n.Alias.ToLower().Contains(query_eng);
                });
            found.Sort((a, b) => b.RunCount.CompareTo(a.RunCount));
            return found;
        }
    }
}