using System;

namespace TizTaboo
{
    /// <summary>
    /// Описывает параметры ссылки
    /// </summary>
    internal class Link
    {
        /// <summary>
        /// Тип ссылки
        /// </summary>
        public faType Type { get; set; }

        /// <summary>
        /// Алиас, уникальный ключ для списка
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// Имя или описание ссылки
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Ссылка или команда
        /// </summary>
        public string Command { get; set; }

        /// <summary>
        /// Параметры запуска
        /// </summary>
        public string Param { get; set; }

        /// <summary>
        /// Время последнего запуска
        /// </summary>
        public DateTime LastExec { get; set; } = DateTime.Now;

        /// <summary>
        /// Количество запусков
        /// </summary>
        public int RunCount { get; set; } = 0;

        /// <summary>
        /// Нужно ли подтверждение для запуска
        /// </summary>
        public bool Confirm { get; set; } = false;

        public Link()
        {
        }
    }
}