using System;
using System.Text;

namespace TizTaboo
{
    public static class StringExt
    {
        private const string
            ENG = "qwertyuiop[]asdfghjkl;'zxcvbnm,.",
            RUS = "йцукенгшщзхъфывапролджэячсмитьбю";

        /// <summary>
        /// Переводит текст написанный на английской раскладке в текст в русской расскладке
        /// </summary>
        public static string EngToRus(this string input)
        {
            var result = new StringBuilder(input.Length);
            int index;
            foreach (var symbol in input.ToLower())
                result.Append((index = ENG.IndexOf(symbol)) != -1 ? RUS[index] : symbol);
            return result.ToString();
        }

        /// <summary>
        /// Переводит текст написанный на русской раскладке в текст в английской расскладке
        /// </summary>
        public static string RusToEng(this string input)
        {
            var result = new StringBuilder(input.Length);
            int index;
            foreach (var symbol in input.ToLower())
                result.Append((index = RUS.IndexOf(symbol)) != -1 ? ENG[index] : symbol);
            return result.ToString();
        }

        /// <summary>
        /// Возвращает элемент перечисления по имени
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T ParseEnum<T>(this string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        /// <summary>
        /// Конвертирует в integer
        /// </summary>
        /// <param name="input"></param>
        /// <param name="throwExceptionIfFailed"></param>
        /// <returns></returns>
        public static int ToInt(this string input, bool throwExceptionIfFailed = false)
        {
            int result;
            var valid = int.TryParse(input, out result);
            if (!valid)
                if (throwExceptionIfFailed)
                    throw new FormatException(string.Format("'{0}' cannot be converted as int", input));
            return result;
        }

       
    }
}