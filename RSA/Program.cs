﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace RSA
{
    class Program
    {
        private static Dictionary<char, byte> alphabet = new Dictionary<char, byte>();

        static void Main(string[] args)
        {
            // Генерируем алфавит
            Console.WriteLine("Choice  alphabet: l - latin; c - cyrillic; any key - quit");
            var alphabetMode = Console.ReadKey(true).KeyChar;

            if (alphabetMode == 'l' || alphabetMode == 'д')
            {
                for (int i = 0; i < 26; i++)
                {
                    alphabet.Add(
                        Convert.ToChar(i + Convert.ToInt32('a')),
                        Convert.ToByte(10 + i));
                }
            }
            else if (alphabetMode == 'c' || alphabetMode == 'c')
            {
                for (int i = 0; i < 33; i++)
                {
                    alphabet.Add(
                        Convert.ToChar(i + Convert.ToInt32('а')),
                        Convert.ToByte(10 + i));
                }
            }
            else
            {
                Console.WriteLine("Для продолжения нажмите любую клавишу...");
                Console.ReadKey();
                return;
            }

            alphabet.Add(' ', 66);

            do
            {
                Console.WriteLine("Choice program mode: e - encrypt; d - decrypt; g - generate key; any key - quit");
                var programMode = Console.ReadKey(true).KeyChar;
                if (programMode == 'e' || programMode == 'у')
                    Encrypt();
                else if (programMode == 'd' || programMode == 'в')
                    Decrypt();
                else if (programMode == 'g' || programMode == 'п')
                    GenerateKey();
                else
                    break;
            } while (true);

            Console.WriteLine("Для продолжения нажмите любую клавишу...");
            Console.ReadKey();
        }

        private static void Encrypt()
        {
            RSA encoder = new RSA();

            // Открываем файл публичного ключа
            using (var publicKeyFile = new FileStream("PublicKey.txt", FileMode.Open))
            {
                if (publicKeyFile == null)
                {
                    Console.WriteLine("Ошибка. Файл ключа для шифрования не может быть открыт");
                    return;
                }

                byte[] key = new byte[publicKeyFile.Length];
                publicKeyFile.Read(key, 0, key.Length);
                encoder.ExportPublicKey(Encoding.UTF8.GetString(key));
            }

            byte[] inputText;
            byte[] inputTextCode;
            byte[] outputText = null;

            // Открываем файл InputText.txt
            // Если такого нет, вводим текст с клавиатуры и сохраняем в данный файл
            using (var inputFile = new FileStream("InputText.txt", FileMode.OpenOrCreate))
            {
                // Если открытый файл пустой
                if (inputFile.Length == 0)
                {
                    // Вводим текст с клавиатуры
                    Console.WriteLine("Enter text");
                    inputText = Encoding.UTF8.GetBytes(Console.ReadLine().ToLower());
                    inputFile.Write(inputText, 0, inputText.Length);
                }
                else
                {
                    // Считываем текст с файла
                    var array = new byte[inputFile.Length];
                    inputFile.Read(array, 0, array.Length);
                    inputText = Encoding.UTF8.GetBytes(Encoding.UTF8.GetString(array).ToLower());
                }
            }

            inputTextCode = new byte[inputText.Length];

            for (int i = 0; i < inputTextCode.Length; i++)
            {
                inputTextCode[i] = alphabet[Convert.ToChar(inputText[i])];
            }

            outputText = encoder.Encrypt(inputTextCode);

            // Записываем зашифрованный текст в файл EnryptedText.txt
            using (FileStream outputFile = new FileStream("EnryptedText.txt", FileMode.Create))
            {
                if (outputFile != null)
                {
                    outputFile.Write(outputText, 0, outputText.Length);
                }
            }

            Console.WriteLine("Данные успешно зашифрованы\n" + 
                              Encoding.UTF8.GetString(outputText));
        }

        private static void Decrypt()
        {
            byte[] inputText;
            byte[] outputTextCode = null;
            byte[] outputText = null;

            RSA encoder = new RSA();

            // Открываем файл приватного ключа
            using (var privateKeyFile = new FileStream("PrivateKey.txt", FileMode.Open))
            {
                if (privateKeyFile == null)
                {
                    Console.WriteLine("Ошибка. Файл ключа для дешифрования не может быть открыт");
                    return;
                }

                byte[] key = new byte[privateKeyFile.Length];
                privateKeyFile.Read(key, 0, key.Length);
                encoder.ExportPrivateKey(Encoding.Default.GetString(key));
            }

            // Открываем файл с зашифрованным текстом EnryptedText.txt
            using (FileStream inputFile = new FileStream("EnryptedText.txt", FileMode.Open))
            {
                // Если файл пустой, то выводим сообщение об ошибке и завершаем программу
                if (inputFile.Length == 0)
                {
                    Console.WriteLine("Ощибка. Файл EnryptedText.txt пуст");
                    return;
                }

                inputText = new byte[inputFile.Length];
                inputFile.Read(inputText, 0, inputText.Length);
            }

            outputTextCode = encoder.Decrypt(inputText);
            outputText = new byte[outputTextCode.Length];

            for (int i = 0; i < outputTextCode.Length; i++)
            {
                outputText[i] = Convert.ToByte(alphabet.FirstOrDefault(x => x.Value == outputTextCode[i]).Key);
            }

            // Запись результатов в файл DeryptedText.txt
            using (FileStream outputFile = new FileStream("DeryptedText.txt", FileMode.Create))
            {
                outputFile.Write(outputText, 0, outputText.Length);
            }

            Console.WriteLine("Данные успешно расшифрованы\n" +
                              Encoding.UTF8.GetString(outputText));
        }

        private static void GenerateKey()
        {

            RSA encoder = new RSA();
            encoder.GenerateKey();

            // Создаем файл публичного ключа
            using (FileStream publicFileKey = new FileStream("PublicKey.txt", FileMode.Create)) //запись результата в  файл
            {
                if (publicFileKey != null)
                {

                    string publicKey = encoder.ImportPublicKey();
                    byte[] array = Encoding.UTF8.GetBytes(publicKey);
                    publicFileKey.Write(array, 0, array.Length);
                    Console.WriteLine("Файл публичного ключа создан");
                }
            }

            // Создаем файл приватного ключа
            using (FileStream privateFileKey = new FileStream("PrivateKey.txt", FileMode.Create)) //запись результата в  файл
            {
                if (privateFileKey != null)
                {

                    string privateKey = encoder.ImportPrivateKey();
                    byte[] array = Encoding.UTF8.GetBytes(privateKey);
                    privateFileKey.Write(array, 0, array.Length);
                    Console.WriteLine("Файл секретного ключа создан");
                }
            }
        }
    }
}
