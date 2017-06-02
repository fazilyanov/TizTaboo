using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;

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
        /// Дата последнего редактирования, нужна для синхронизации
        /// </summary>
        public DateTime LastEditDateTime { get; set; }

        /// <summary>
        /// Путь для хранения на диске
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
                        // В первой строке хранится дата последнего редактирования списка ссылок
                        LastEditDateTime = DateTime.ParseExact(arrNotes[0], "dd.MM.yyyy HH:mm:ss", new CultureInfo("ru-RU"));

                        foreach (string note in arrNotes.Skip(1))
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
                                        LastExec = DateTime.ParseExact(arrParam[5], "dd.MM.yyyy HH:mm:ss", new CultureInfo("ru-RU")),
                                        Type = arrParam[6].ParseEnum<faType>(),
                                        RunCount = arrParam[7].ToInt()
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
                string toSaveText = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss") + "*#*\n";
                foreach (Link item in LinkList)
                {
                    toSaveText += item.Alias + "*|*";
                    toSaveText += item.Name + "*|*";
                    toSaveText += item.Command + "*|*";
                    toSaveText += item.Param + "*|*";
                    toSaveText += item.Confirm ? "1" : "0" + "*|*";
                    toSaveText += item.LastExec.ToString("dd.MM.yyyy HH:mm:ss") + "*|*";
                    toSaveText += item.Type.ToString() + "*|*";
                    toSaveText += item.RunCount + "*#*\n";
                }

                File.WriteAllText(DataFilePath, toSaveText);
                GDrive("write");
            }
            catch (Exception ex)
            {
                Log.Error("#201706021426: " + ex.Message);
                return false;
            }
            return true;
        }

        private string GDrive(string act)
        {
            string result = string.Empty;
            string[] Scopes = { DriveService.Scope.Drive };
            try
            {
                FileStream fs = new FileStream("client_id.json", FileMode.Open, FileAccess.Read);
                using (FileStream stream = new FileStream("client_id.json", FileMode.Open, FileAccess.Read))
                {
                    string credPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

                    credPath = Path.Combine(credPath, ".credentials/drive-dotnet-quickstart.json");

                    UserCredential credential =
                        GoogleWebAuthorizationBroker.AuthorizeAsync(
                            GoogleClientSecrets.Load(stream).Secrets,
                            Scopes, GoogleClientSecrets.Load(fs).Secrets.ClientId,
                            CancellationToken.None,
                            new FileDataStore(credPath, true)
                        ).Result;

                    var service = new DriveService(
                        new BaseClientService.Initializer()
                        {
                            HttpClientInitializer = credential,
                            ApplicationName = "TizTaboo",
                        }
                    );
                    fs.Close();

                    switch (act)
                    {
                        case "write":
                            Google.Apis.Drive.v3.Data.File body = new Google.Apis.Drive.v3.Data.File()
                            {
                                Name = Path.GetFileName(DataFilePath),
                                MimeType = "application/octet-stream",
                                Description = "Описание"
                            };
                            byte[] byteArray = File.ReadAllBytes(DataFilePath);
                            MemoryStream mStream = new MemoryStream(byteArray);
                            FilesResource.CreateMediaUpload createRequest;
                            FilesResource.UpdateMediaUpload updateRequest;

                            !!! Узнать существует ли файл, если есть обновить его, если нет создать!!!
                            //    !!! FilesResource.GetRequest getRequest = service.Files.Get(Properties.Settings.Default.gFileId);
                            //   getRequest.Execute();

                            if (Properties.Settings.Default.gFileId != "0")
                            {
                                updateRequest = service.Files.Update(body, Properties.Settings.Default.gFileId, mStream, body.MimeType);
                            }
                            else
                            {
                                request= service.Files.Create(body, mStream, body.MimeType);
                            }
                            if (request.Upload().Exception != null)
                            {
                                result = request.Upload().Exception.Message;
                            }
                            else
                            {
                                if (Properties.Settings.Default.gFileId != request.ResponseBody.Id)
                                {
                                    Properties.Settings.Default.gFileId = request.ResponseBody.Id;
                                    Properties.Settings.Default.Save();
                                }
                            }
                            break;

                        case "read":
                            FilesResource.GetRequest getRequest = service.Files.Get(Properties.Settings.Default.gFileId);
                            getRequest.Execute();
                            FileStream fsDownload = new FileStream("aa", FileMode.Open, FileAccess.Write);
                            getRequest.Download(fsDownload);
                            break;

                        default:
                            break;
                    }

                    //FilesResource.ListRequest listRequest = service.Files.List();
                    ////

                    //listRequest.PageSize = 400;
                    //listRequest.Fields = "nextPageToken, files(id, webViewLink, webContentLink, name, size, mimeType)";//требуемые свойства загружаемых файлов(можно убрать лишнее или добавить требуемое)
                    //                                                                                                   //  files.
                    //IList<Google.Apis.Drive.v3.Data.File> files = listRequest.Execute().Files;
                    //files.Clear();
                }
            }
            catch (Exception ex)
            {
                Log.Error("#201706021625: " + ex.Message);
            }
            return result;
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