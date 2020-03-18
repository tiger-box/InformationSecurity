// Solution: InformationSecurity
// Project: RC4
// Program.cs
// Author: Nazar Lyamzin

using System;
using System.IO;
using System.Text;

namespace RC4
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Choice program mode: e - encrypt; d - decrypt; any key - quit");
            var programMode = Console.ReadKey(true).KeyChar;
            if (programMode == 'e' || programMode == 'у')
                Encrypt();
            else if (programMode == 'd' || programMode == 'в') 
                Decrypt();

            Console.ReadKey();
        }

        private static void Encrypt()
        {
            string inputText;
            string outputText = null;
            string key;

            // Открываем файл InputText.txt
            // Если такого нет, вводим текст с клавиатуры и сохраняем в данный файл
            using (var inputFile = new FileStream("InputText.txt", FileMode.OpenOrCreate))
            {
                // Если открытый файл пустой
                if (inputFile.Length == 0)
                {
                    // Вводим текст с клавиатуры
                    Console.WriteLine("Enter text");
                    inputText = Console.ReadLine();
                    var array = Encoding.Default.GetBytes(inputText);
                    inputFile.Write(array, 0, array.Length);
                }
                else
                {
                    // Считываем текст с клавиатуры
                    var array = new byte[inputFile.Length];
                    inputFile.Read(array, 0, array.Length);
                    inputText = Encoding.Default.GetString(array);
                }
            }


            // Вводим ключ, удаляем в нем пробелы
            Console.WriteLine("Enter key");
            key = Console.ReadLine().Replace(" ", "");

            // Экземпляр класса для шифрования
            RC4 encoder = new RC4(Encoding.Default.GetBytes(key));
            outputText = Encoding.Default.GetString(
                encoder.Code(
                    Encoding.Default.GetBytes(inputText)));

            Console.WriteLine(outputText);

            // Записываем зашифрованный текст в файл EnryptedText.txt
            using (FileStream outputFile = new FileStream("EnryptedText.txt", FileMode.Create)) //запись результата в  файл
            {
                if (outputFile != null)
                {
                    byte[] array = Encoding.Default.GetBytes(outputText);
                    outputFile.Write(array, 0, array.Length);
                }
            }
        }

        private static void Decrypt()
        {
            string inputText;
            string outputText = null;
            string key;

            // Открываем файл с зашифрованным текстом EnryptedText.txt
            using (FileStream inputFile = new FileStream("EnryptedText.txt", FileMode.OpenOrCreate))
            {
                // Если файл пустой, то выводим сообщение об ошибке и завершаем программу
                if (inputFile.Length == 0)
                {
                    Console.WriteLine("Error. File is empty.");
                    return;
                }

                byte[] array = new byte[inputFile.Length];
                inputFile.Read(array, 0, array.Length);
                inputText = System.Text.Encoding.Default.GetString(array);
            }

            // Вводим ключ и удаляем пробелы в нем
            Console.WriteLine("Enter key");
            key = Console.ReadLine().Replace(" ", "");

            // Экземпляр класса для дешифровки
            RC4 encoder = new RC4(Encoding.Default.GetBytes(key));
            outputText = Encoding.Default.GetString(
                encoder.Code(
                    Encoding.Default.GetBytes(inputText)));

            Console.WriteLine(outputText);

            // Запись результатов в файл DeryptedText.txt
            using (FileStream outputFile = new FileStream("DeryptedText.txt", FileMode.Create))
            {
                byte[] array = System.Text.Encoding.Default.GetBytes(outputText);
                outputFile.Write(array, 0, array.Length);
            }
        }
    }
}